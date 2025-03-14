﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Threading.Tasks;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.File.Repositories
{
    public class ObservingFacilityRepository : IObservingFacilityRepository
    {
        private const string _rootDirectory = @"C:\Data\Observations";
        //private const string _rootDirectory = "/Data";

        private static int _nextId;
        private static Dictionary<int, int> _idMap;
        private static Dictionary<int, int> _stationIdMap;

        static ObservingFacilityRepository()
        {
            _nextId = 1;
            _idMap = new Dictionary<int, int>();
            _stationIdMap = new Dictionary<int, int>();
        }

        public Task Add(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRange(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<ObservingFacility, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<ObservingFacility, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ObservingFacility>> Find(
            Expression<Func<ObservingFacility, bool>> predicate)
        {
            return await Task.Run(() => {
                var temp = predicate.Analyze() as Predicate;

                if (temp.Field == "StatId" &&
                    temp.Operator == Operator.Equal)
                {
                    var statId = temp.Value.ToString();
                    var result = new List<ObservingFacility>();

                    var rootDirectory = new DirectoryInfo(_rootDirectory);

                    if (rootDirectory.Exists)
                    {
                        var yearDirectories = rootDirectory.GetDirectories();

                        foreach (var yearDirectory in yearDirectories)
                        {
                            var stationDirectories = yearDirectory.GetDirectories();

                            if (stationDirectories
                                .Select(_ => _.Name)
                                .Contains(statId))
                            {
                                result.Add(GenerateObservingFacility(int.Parse(statId)));
                                break;
                            }
                        }
                    }

                    return result;
                }

                // Todo: Denne skal kunne bruges, hvis man bruger et bestemt predicate
                throw new InvalidOperationException();
            });
        }

        public Task<IEnumerable<ObservingFacility>> Find(
            IList<Expression<Func<ObservingFacility, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public ObservingFacility Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ObservingFacility>> GetAll()
        {
            var rootDirectory = new DirectoryInfo(_rootDirectory);
            var stationIds = new HashSet<int>();

            if (rootDirectory.Exists)
            {
                var yearDirectories = rootDirectory.GetDirectories();

                foreach (var yearDirectory in yearDirectories)
                {
                    var stationDirectories = yearDirectory.GetDirectories();

                    foreach (var stationDirectory in stationDirectories)
                    {
                        stationIds.Add(int.Parse(stationDirectory.Name));
                    }
                }
            }

            return stationIds.Select(_ => GenerateObservingFacility(_));
        }

        public ObservingFacility GetIncludingTimeSeries(int id)
        {
            if(!_stationIdMap.TryGetValue(id, out var statId))
            {
                throw new InvalidOperationException();
            }

            var result = GenerateObservingFacility(statId);

            var rootDirectory = new DirectoryInfo(_rootDirectory);

            if (rootDirectory.Exists)
            {
                var paramIds = new HashSet<string>();
                var yearDirectories = rootDirectory.GetDirectories();

                foreach (var yearDirectory in yearDirectories)
                {
                    var stationDirectory = yearDirectory
                        .GetDirectories(statId.ToString())
                        .SingleOrDefault();

                    if (stationDirectory != null)
                    {
                        var files = stationDirectory.GetFiles();

                        foreach (var file in files)
                        {
                            var paramId = file.Name.Substring(file.Name.IndexOf('_') + 1);
                            paramId = paramId.Substring(0, paramId.Length - 9);
                            paramIds.Add(paramId);
                        }
                    }
                }

                result.TimeSeries = paramIds.Select(_ =>
                {
                    var timeSeries = TimeSeriesRepository.GenerateTimeSeries(statId, _);
                    timeSeries.ObservingFacilityId = id;
                    timeSeries.ObservingFacility = result;
                    return timeSeries;
                }).ToList();
            }

            return result;
        }

        public void Load(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        public Task Remove(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        public ObservingFacility SingleOrDefault(Expression<Func<ObservingFacility, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task Update(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRange(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        // Denne helper funktion genererer en Observing Facility og tildeler den et
        // id. Hvis en tidligere query har produceret samme station, genbruges det
        // pågældende id
        private ObservingFacility GenerateObservingFacility(int statId)
        {
            if (!_idMap.TryGetValue(statId, out int id))
            {
                id = _nextId++;
                _idMap.Add(statId, id);
                _stationIdMap.Add(id, statId);
            }

            return new ObservingFacility { Id = id, StatId = statId };
        }
    }
}
