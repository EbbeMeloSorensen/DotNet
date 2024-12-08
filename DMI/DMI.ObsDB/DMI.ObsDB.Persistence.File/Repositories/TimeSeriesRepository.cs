using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.File.Repositories
{
    public class TimeSeriesRepository : ITimeSeriesRepository
    {
        private static int _nextId;
        private static Dictionary<int, int> _idMap;
        private static Dictionary<int, Tuple<int, string>> _stationIdAndParamIdMap;

        static TimeSeriesRepository()
        {
            _nextId = 1;
            _idMap = new Dictionary<int, int>();
            _stationIdAndParamIdMap = new Dictionary<int, Tuple<int, string>>();
        }

        public Task Add(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<TimeSeries, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<TimeSeries, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TimeSeries>> Find(Expression<Func<TimeSeries, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TimeSeries>> Find(IList<Expression<Func<TimeSeries, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public TimeSeries Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TimeSeries>> GetAll()
        {
            throw new NotImplementedException();
        }

        public TimeSeries GetIncludingObservations(
            int id)
        {
            if (!_stationIdAndParamIdMap.TryGetValue(id, out var statIdAndParamId))
            {
                throw new InvalidOperationException();
            }

            var statId = statIdAndParamId.Item1;
            var paramId = statIdAndParamId.Item2;
            var result = GenerateTimeSeries(statId, paramId);

            var rootDirectory = new DirectoryInfo(@"C:\Data\Observations");

            if (rootDirectory.Exists)
            {
                var observations = new List<Observation>();
                var yearDirectories = rootDirectory.GetDirectories();

                foreach (var yearDirectory in yearDirectories)
                {
                    var year = int.Parse(yearDirectory.Name);

                    var stationDirectory = yearDirectory
                        .GetDirectories(statId.ToString())
                        .SingleOrDefault();

                    if (stationDirectory != null)
                    {
                        var paramDirectory = stationDirectory
                            .GetDirectories(paramId)
                            .SingleOrDefault();

                        if (paramDirectory != null)
                        {
                            var fileName = $"{statId}_{paramId}_{yearDirectory.Name}.txt";

                            var file = paramDirectory
                                .GetFiles(fileName)
                                .SingleOrDefault();

                            if (file == null)
                            {
                                continue;
                            }

                            var observationTimesForCurrentYear =
                                ReadObservationsFromFile(file.FullName);

                            observationTimesForCurrentYear.ForEach(_ =>
                            {
                                _.TimeSeriesId = id;
                            });

                            observations.AddRange(observationTimesForCurrentYear);
                        }
                    }
                }

                result.Observations = observations;
            }

            return result;
        }

        public TimeSeries GetIncludingObservations(
            int id,
            DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public TimeSeries GetIncludingObservations(
            int id,
            DateTime startTime,
            DateTime endTime)
        {
            if (!_stationIdAndParamIdMap.TryGetValue(id, out var statIdAndParamId))
            {
                throw new InvalidOperationException();
            }

            var statId = statIdAndParamId.Item1;
            var paramId = statIdAndParamId.Item2;
            var result = GenerateTimeSeries(statId, paramId);

            var rootDirectory = new DirectoryInfo(@"C:\Data\Observations");

            if (rootDirectory.Exists)
            {
                var observations = new List<Observation>();
                var yearDirectories = rootDirectory.GetDirectories();

                foreach (var yearDirectory in yearDirectories)
                {
                    var year = int.Parse(yearDirectory.Name);
                    
                    if (year < startTime.Year ||
                        year > endTime.Year)
                    {
                        continue;
                    }

                    var stationDirectory = yearDirectory
                        .GetDirectories(statId.ToString())
                        .SingleOrDefault();

                    if (stationDirectory != null)
                    {
                        var fileName = $"{statId}_{paramId}_{yearDirectory.Name}.txt";

                        var file = stationDirectory
                            .GetFiles(fileName)
                            .SingleOrDefault();

                        if (file == null)
                        {
                            continue;
                        }

                        var observationTimesForCurrentYear =
                            ReadObservationsFromFile(file.FullName);

                        if (year == startTime.Year)
                        {
                            observationTimesForCurrentYear =
                                observationTimesForCurrentYear
                                    .Where(_ => _.Time >= startTime)
                                    .ToList();
                        }

                        if (year == endTime.Year)
                        {
                            observationTimesForCurrentYear =
                                observationTimesForCurrentYear
                                    .Where(_ => _.Time < endTime)
                                    .ToList();
                        }

                        observationTimesForCurrentYear.ForEach(_ =>
                        {
                            _.TimeSeriesId = id;
                        });

                        observations.AddRange(observationTimesForCurrentYear);
                        
                    }
                }

                result.Observations = observations;
            }

            return result;
        }

        public void Load(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public Task Remove(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public TimeSeries SingleOrDefault(Expression<Func<TimeSeries, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task Update(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public static TimeSeries GenerateTimeSeries(int statId, string paramId)
        {
            var hashCode = new Tuple<int, string>(statId, paramId).GetHashCode();

            if (!_idMap.TryGetValue(hashCode, out int id))
            {
                id = _nextId++;
                _idMap.Add(hashCode, id);
                _stationIdAndParamIdMap.Add(id, new Tuple<int, string>(statId, paramId));
            }

            return new TimeSeries { Id = id, ParamId = paramId };
        }

        private static List<Observation> ReadObservationsFromFile(
            string fileName)
        {
            var result = new List<Observation>();

            using (var streamReader = new StreamReader(fileName))
            {
                string line;

                var dateOfObservation = DateTime.MinValue;

                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line == "No observations")
                    {
                        return result;
                    }

                    if (line.Length <= 10)
                    {
                        var elements = line.Split('-');
                        var year = int.Parse(elements[0]);
                        var month = int.Parse(elements[1]);
                        var day = int.Parse(elements[2]);

                        dateOfObservation = new DateTime(year, month, day);

                        continue;
                    }

                    var lineElements = line.Split();
                    var observationTimeAsString = lineElements[1];
                    var observationValueAsString = lineElements[2];
                    var hour = int.Parse(observationTimeAsString.Substring(0, 2));
                    var minute = int.Parse(observationTimeAsString.Substring(3, 2));
                    var second = int.Parse(observationTimeAsString.Substring(6, 2));
                    var value = double.Parse(observationValueAsString, NumberStyles.Number, CultureInfo.InvariantCulture);

                    result.Add(new Observation
                    {
                        Time = new DateTime(
                            dateOfObservation.Year,
                            dateOfObservation.Month,
                            dateOfObservation.Day,
                            hour,
                            minute,
                            second,
                            DateTimeKind.Utc),
                        Value = value
                    });
                }
            }

            return result;
        }
    }
}
