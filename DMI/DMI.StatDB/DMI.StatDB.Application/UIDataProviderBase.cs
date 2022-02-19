using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.IO;

namespace DMI.StatDB.Application
{
    public abstract class UIDataProviderBase : IUIDataProvider
    {
        protected ILogger _logger;
        private readonly IDataIOHandler _dataIOHandler;

        protected UIDataProviderBase(
            IDataIOHandler dataIOHandler)
        {
            _dataIOHandler = dataIOHandler;
        }

        public virtual void Initialize(
            ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task<bool> CheckConnection();

        public abstract int CountAllStations();

        public abstract int CountStations(
            Expression<Func<Station, bool>> predicate);

        public abstract int CountStations(
            IList<Expression<Func<Station, bool>>> predicates);

        public abstract IList<Station> GetAllStations();

        public abstract IList<Station> FindStations(
            IList<Expression<Func<Station, bool>>> predicates);

        public abstract IList<Station> FindStationsWithPositions(
            Expression<Func<Station, bool>> predicate);

        public abstract IList<Station> FindStationsWithPositions(
            IList<Expression<Func<Station, bool>>> predicates);

        public void ExportData(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException();
            }

            _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving all stationinformation records from repository..");
            var allStations = GetAllStations();
            _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieved {allStations.Count} stationinformation records");

            switch (extension)
            {
                //case ".xml":
                //{
                //    _dataIOHandler.ExportDataToXML(allStationInformations, fileName);
                //    _logger?.WriteLine(LogMessageCategory.Information,
                //        $"  Exported {allStationInformations.Count} stationinformation records to xml file");
                //    break;
                //}
                case ".json":
                {
                    _dataIOHandler.ExportDataToJson(allStations, fileName);
                    _logger?.WriteLine(LogMessageCategory.Information,
                        $"  Exported {allStations.Count} stationinformation records to json file");
                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }
        }
    }
}
