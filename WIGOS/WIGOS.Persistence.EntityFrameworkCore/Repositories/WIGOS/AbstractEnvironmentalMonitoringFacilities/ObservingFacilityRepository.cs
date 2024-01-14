using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using WIGOS.Persistence.Repositories.WIGOS;
using Craft.Persistence;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.WIGOS.AbstractEnvironmentalMonitoringFacilities
{
    public class ObservingFacilityRepository : Repository<ObservingFacility>, IObservingFacilityRepository
    {
        private WIGOSDbContextBase DbContext => Context as WIGOSDbContextBase;

        public ObservingFacilityRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public ObservingFacility Get(
            Guid id)
        {
            return DbContext.ObservingFacilities.SingleOrDefault(_ => _.Id == id) ?? throw new InvalidOperationException();
        }

        public Tuple<ObservingFacility, List<GeospatialLocation>> GetIncludingGeospatialLocations(
            Guid id,
            IList<Expression<Func<GeospatialLocation, bool>>> geospatialLocationPredicates)
        {
            var observingFacility = Get(id);

            geospatialLocationPredicates.Add(_ => _.AbstractEnvironmentalMonitoringFacilityObjectId == observingFacility.ObjectId);

            var geospatialLocationPredicate = geospatialLocationPredicates
                .Aggregate((c, n) => c.And(n));

            var geospatialLocations = DbContext.GeospatialLocations
                .Where(geospatialLocationPredicate)
                .ToList();

            return new Tuple<ObservingFacility, List<GeospatialLocation>>(observingFacility, geospatialLocations);
        }

        public Dictionary<ObservingFacility, List<GeospatialLocation>> FindIncludingGeospatialLocations(
            Expression<Func<ObservingFacility, bool>> observingFacilityPredicate,
            Expression<Func<GeospatialLocation, bool>> geospatialLocationPredicate)
        {
            var observingFacilityPredicates = new List<Expression<Func<ObservingFacility, bool>>>
        {
            observingFacilityPredicate
        };

            var geospatialLocationPredicates = new List<Expression<Func<GeospatialLocation, bool>>>
        {
            geospatialLocationPredicate
        };

            return FindIncludingGeospatialLocations(
                observingFacilityPredicates,
                geospatialLocationPredicates);
        }

        public Dictionary<ObservingFacility, List<GeospatialLocation>> FindIncludingGeospatialLocations(
            IList<Expression<Func<ObservingFacility, bool>>> observingFacilityPredicates,
            IList<Expression<Func<GeospatialLocation, bool>>> geospatialLocationPredicates)
        {
            var observingFacilities = observingFacilityPredicates.Any()
                ? DbContext.ObservingFacilities
                    .Where(observingFacilityPredicates.Aggregate((c, n) => c.And(n)))
                    .ToList()
                : DbContext.ObservingFacilities
                    .ToList();

            var observingFacilityObjectIds = observingFacilities
                .Select(_ => _.ObjectId)
                .ToList();

            geospatialLocationPredicates.Add(_ =>
                observingFacilityObjectIds.Contains(_.AbstractEnvironmentalMonitoringFacilityObjectId));

            //var databaseTimeOfInterest = new DateTime(2023, 12, 11, 19, 3, 0, DateTimeKind.Utc);

            var geospatialLocationPredicate = geospatialLocationPredicates.Aggregate((c, n) => c.And(n));

            var geospatialLocations = DbContext.GeospatialLocations
                .Where(geospatialLocationPredicate)
                .ToList();

            var geospatialLocationGroups = geospatialLocations.
                GroupBy(_ => _.AbstractEnvironmentalMonitoringFacilityObjectId);

            var result = observingFacilities.ToDictionary(
                of => of,
                of =>
                {
                    return geospatialLocationGroups
                        .Single(glg => glg.Key == of.ObjectId)
                        .ToList();
                });

            return result;
        }

        public override void Update(
            ObservingFacility observingFacility)
        {
            // Todo: Test this
            var observingFacilityFromRepository = Get(observingFacility.Id);

            observingFacilityFromRepository.Name = observingFacility.Name;
            observingFacilityFromRepository.DateEstablished = observingFacility.DateEstablished;
            observingFacilityFromRepository.DateClosed = observingFacility.DateClosed;
        }

        public override void UpdateRange(
            IEnumerable<ObservingFacility> observingFacilities)
        {
            var updatedObservingFacilities = observingFacilities.ToList();
            var ids = updatedObservingFacilities.Select(p => p.Id);
            var observingFacilitiesFromRepository = Find(p => ids.Contains(p.Id)).ToList();

            observingFacilitiesFromRepository.ForEach(ofRepo =>
            {
                var updatedObservingFacility = updatedObservingFacilities.Single(ofUpd => ofUpd.Id == ofRepo.Id);

                ofRepo.Name = updatedObservingFacility.Name;
                ofRepo.DateEstablished = updatedObservingFacility.DateEstablished;
                ofRepo.DateClosed = updatedObservingFacility.DateClosed;
            });
        }
    }
}