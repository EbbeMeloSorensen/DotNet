using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Application
{
    public abstract class UIDataProviderBase : IUIDataProvider
    {
        protected ILogger _logger;

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
    }
}
