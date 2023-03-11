using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMI.SMS.Persistence.EntityFrameworkCore.PostgreSQL.Repositories;
using DMI.SMS.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DMI.SMS.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SMSDbContext _context;

        public IStationInformationRepository StationInformations { get; }
        public IContactPersonRepository ContactPersons { get; }
        public ILegalOwnerRepository LegalOwners { get; }
        public ISensorLocationRepository SensorLocations { get; }
        public ISensorInformationRepository SensorInformations { get; }
        public IImagesOfSensorLocationRepository ImagesOfSensorLocations { get; }

        public UnitOfWork(SMSDbContext context)
        {
            _context = context;
            StationInformations = new StationInformationRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
