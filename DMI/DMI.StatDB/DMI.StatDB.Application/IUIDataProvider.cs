using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Application
{
    public interface IUIDataProvider
    {
        void Initialize(ILogger logger);

        Task<bool> CheckConnection();

        int CountAllStations();

        int CountStations(
            Expression<Func<Station, bool>> predicate);

        int CountStations(
            IList<Expression<Func<Station, bool>>> predicates);

        Task<IList<Station>> GetAllStations();

        Task<IList<Position>> GetAllPositions();

        Task<IList<Station>> FindStations(
            IList<Expression<Func<Station, bool>>> predicates);

        Task<IList<Station>> FindStationsWithPositions(
            Expression<Func<Station, bool>> predicate);

        Task<IList<Station>> FindStationsWithPositions(
            IList<Expression<Func<Station, bool>>> predicates);

        Task ExportData(
            string fileName);

        void ImportData(
            string fileName);
    }
}
