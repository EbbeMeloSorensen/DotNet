using Craft.Logging;
using System;
using System.Threading.Tasks;
using DMI.Utils;
using Npgsql;

namespace DMI.DAL.ClimaDB
{
    public class DataProvider : DataProviderBase
    {
        public DataProvider(
            ILogger logger) : base(logger)
        {
        }

        // Denne bruges til at checke konsistens for grønlandske manuelle nedbørsstationer
        public async Task<long> CountAllManualPrecipitationObservationsForStation(
            string host,
            string database,
            string climaDBUser,
            string climaDBPassword,
            string stationId,
            DateTime startTime,
            DateTime endTime)
        {
            return await Task.Run(() =>
            {
                long result = 0;

                using (var conn = new NpgsqlConnection(ConnectionString(host, database, climaDBUser, climaDBPassword)))
                {
                    conn.Open();

                    var statid = int.Parse($"{stationId}50");

                    var query = string.Concat(
                        "SELECT COUNT(id) FROM basis_daily_na ",
                        $"WHERE statid = {statid} ",
                        $"AND the_date >= '{startTime.AsDateTimeString(false)}'",
                        $"AND the_date < '{endTime.AsDateTimeString(false)}'");

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        result = (long)cmd.ExecuteScalar();
                    }
                }

                return result;
            });
        }

        // Denne bruges til at lave en matrix for stationer vs klima parametre
        public async Task<short?> IdentifyYearForFirstObservation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string stationId,
            int elem_no,
            short? minYearDK,
            short? minYearNA)
        {
            short? result = null;

            if (stationId.Substring(0, 2) == "04" ||
                stationId.Substring(0, 2) == "34")
            {
                // (det er en grønlandsk station)

                if (elem_no == 113 || // "max_temp_12h"
                    elem_no == 123 || // "min_temperature_12h"
                    elem_no == 603 || // "acc_precip_past12h"
                    elem_no == 609 || // "acc_precip_past24h"
                    elem_no == 365)   // "mean_wind_dir_min0"
                {
                    // (parameteren er tilgængelig som hourly)

                    result = await IdentifyYearForFirstObservation_Hourly_or_Daily(
                        host,
                        database,
                        obsDBUser,
                        obsDBPassword,
                        stationId,
                        elem_no,
                        "view_basis_hourly_na");
                }
                else
                {
                    // (parameteren er tilgængelig som yearly)

                    result = await IdentifyYearForFirstObservation_Yearly(
                        host,
                        database,
                        obsDBUser,
                        obsDBPassword,
                        stationId,
                        elem_no,
                        "basis_yearly_na");
                }

                if (result.HasValue && minYearNA.HasValue && result.Value < minYearNA)
                {
                    result = minYearNA;
                }
            }
            else
            {
                // (det er en dansk station)

                if (stationId.Substring(0, 2) == "05" &&
                    (elem_no == 301 || // "mean_wind_speed"
                     elem_no == 302 || // "max_wind_speed_10min"
                     elem_no == 311 || // "no_windy_days"
                     elem_no == 321 || // "no_stormy_days"
                     elem_no == 326 || // "no_days_w_storm"
                     elem_no == 331))  // "no_days_w_hurricane"
                {
                    // (vi har at gøre med en pluvio station samt en vind-parameter -
                    //  for disse har vi observeret målinger i databasen, som må være forkerte,
                    //  og derfor nulstiller vi dem)
                    result = null;
                }
                else if (
                    elem_no == 906 || // "snow_depth"
                    elem_no == 910)   // "snow_cover"
                {
                    // (parameteren er en sneparameter)

                    result = await IdentifyYearForFirstObservation_Hourly_or_Daily(
                        host,
                        database,
                        obsDBUser,
                        obsDBPassword,
                        stationId,
                        elem_no,
                        "basis_daily_dk");
                }
                else if (
                    elem_no == 213 || // "leaf_moisture"
                    elem_no == 106 || // "temp_grass"
                    elem_no == 107 || // "temp_soil_10"
                    elem_no == 108)   // "temp_soil_30" 
                {
                    // (parameteren er en "additional" landbrugsparameter)

                    result = await IdentifyYearForFirstObservation_Hourly_or_Daily(
                        host,
                        database,
                        obsDBUser,
                        obsDBPassword,
                        stationId,
                        elem_no,
                        "add_basis_daily_dk");
                }
                else if (elem_no == 251) // "vapour_pressure_deficit_mean"
                {
                    // (parameteren er en landbrugsparameter, der er tilgængelig som hourly)

                    result = await IdentifyYearForFirstObservation_Hourly_or_Daily(
                        host,
                        database,
                        obsDBUser,
                        obsDBPassword,
                        stationId,
                        elem_no,
                        "view_basis_hourly_dk");
                }
                else
                {
                    // (parameteren er tilgængelig som yearly)

                    result = await IdentifyYearForFirstObservation_Yearly(
                        host,
                        database,
                        obsDBUser,
                        obsDBPassword,
                        stationId,
                        elem_no,
                        "basis_yearly_dk");
                }

                if (result.HasValue && minYearDK.HasValue && result.Value < minYearDK)
                {
                    result = minYearDK;
                }
            }

            return result;
        }

        private async Task<short?> IdentifyYearForFirstObservation_Yearly(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string stationId,
            int elem_no,
            string table)
        {
            return await Task.Run(() =>
            {
                short? result = null;

                using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
                {
                    conn.Open();

                    var statid = int.Parse($"{stationId}");

                    var query = string.Concat(
                        $"SELECT MIN(yyyy) FROM {table} ",
                        $"WHERE statid = {statid} ",
                        $"AND elem_no = {elem_no}");

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var temp = cmd.ExecuteScalar();

                        if (!(temp is DBNull))
                        {
                            result = (short)temp;
                        }
                    }
                }

                return result;
            });
        }

        private async Task<short?> IdentifyYearForFirstObservation_Hourly_or_Daily(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string stationId,
            int elem_no,
            string table)
        {
            return await Task.Run(() =>
            {
                short? result = null;

                using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
                {
                    conn.Open();

                    var statid = int.Parse($"{stationId}");

                    var query = string.Concat(
                        $"SELECT MIN(the_date) FROM {table} ",
                        $"WHERE statid = {statid} ",
                        $"AND elem_no = {elem_no}");

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var temp = cmd.ExecuteScalar();

                        if (!(temp is DBNull))
                        {
                            result = (short)((DateTime)temp).Year;
                        }
                    }
                }

                return result;
            });
        }
    }
}
