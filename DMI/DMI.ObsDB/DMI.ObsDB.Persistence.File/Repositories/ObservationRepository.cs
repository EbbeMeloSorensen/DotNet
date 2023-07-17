using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.IO;
using System.Globalization;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.File.Repositories
{
    public class ObservationRepository : IObservationRepository
    {
        public void Add(Observation entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<Observation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<Observation, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Observation> Find(Expression<Func<Observation, bool>> predicate)
        {
            var temp = predicate.Analyze();

            throw new NotImplementedException();
        }

        public IEnumerable<Observation> Find(IList<Expression<Func<Observation, bool>>> predicates)
        {
            throw new NotImplementedException();

            //var temp = predicates
            //    .Select(p => p.Analyze() as Predicate)
            //    .ToList();

            //var statId = temp.Single(p => p.Field == "StatId").Value.ToString();
            //var paramId = (string)temp.Single(p => p.Field == "ParamId").Value;
            //var t1 = (DateTime)temp.Single(p => p.Field == "Time" && p.Operator == Operator.GreaterThanOrEqual).Value;
            //var t2 = (DateTime)temp.Single(p => p.Field == "Time" && p.Operator == Operator.LessThan).Value;

            //var firstYear = t1.Year;
            //var lastYear = t2.Year;
            //var years = Enumerable.Range(firstYear, lastYear - firstYear + 1);

            //var result = new List<Observation>();

            //foreach (var year in years)
            //{
            //    var directory = new DirectoryInfo(Path.Combine(@"C:\Data\Observations", $"{year}", $"{statId}", $"{paramId}"));

            //    if (!directory.Exists)
            //    {
            //        continue;
            //    }

            //    var fileName = $"{statId}_{paramId}_{year}.txt";
            //    var file = directory.GetFiles(fileName).SingleOrDefault();

            //    if (file == null)
            //    {
            //        continue;
            //    }

            //    var observationTimesForCurrentYear = ReadObservationsForStationFromFile(file.FullName);
            //    result.AddRange(observationTimesForCurrentYear);
            //}

            //result = result
            //    .Where(o => o.Time >= t1)
            //    .Where(o => o.Time <= t2)
            //    .OrderBy(o => o.Time)
            //    .ToList();

            //return result;
        }

        public IEnumerable<Observation> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(Observation entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        public Observation SingleOrDefault(Expression<Func<Observation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Update(Observation entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<Observation> entities)
        {
            throw new NotImplementedException();
        }

        //private List<Observation> ReadObservationsForStationFromFile(
        //    string fileName)
        //{
        //    var result = new List<Observation>();

        //    using (var streamReader = new StreamReader(fileName))
        //    {
        //        string line;

        //        var dateOfObservation = DateTime.MinValue;

        //        while ((line = streamReader.ReadLine()) != null)
        //        {
        //            if (line == "No observations")
        //            {
        //                return result;
        //            }

        //            if (line.Length <= 10)
        //            {
        //                var elements = line.Split('-');
        //                var year = int.Parse(elements[0]);
        //                var month = int.Parse(elements[1]);
        //                var day = int.Parse(elements[2]);

        //                dateOfObservation = new DateTime(year, month, day);

        //                continue;
        //            }

        //            var lineElements = line.Split();
        //            var observationTimeAsString = lineElements[1];
        //            var observationValueAsString = lineElements[2];
        //            var hour = int.Parse(observationTimeAsString.Substring(0, 2));
        //            var minute = int.Parse(observationTimeAsString.Substring(3, 2));
        //            var second = int.Parse(observationTimeAsString.Substring(6, 2));
        //            var value = double.Parse(observationValueAsString, NumberStyles.Number, CultureInfo.InvariantCulture);

        //            result.Add(new Observation
        //            {
        //                Time = new DateTime(
        //                    dateOfObservation.Year,
        //                    dateOfObservation.Month,
        //                    dateOfObservation.Day,
        //                    hour,
        //                    minute,
        //                    second),
        //                Value = value
        //            });
        //        }
        //    }

        //    return result;
        //}
    }
}