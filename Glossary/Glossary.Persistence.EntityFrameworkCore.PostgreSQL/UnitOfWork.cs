﻿using Glossary.Persistence.Repositories;
using Glossary.Persistence.EntityFrameworkCore.PostgreSQL.Repositories;

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PRDbContext _context;

        public IPersonRepository People { get; }
        public IPersonAssociationRepository PersonAssociations { get; }

        public UnitOfWork(PRDbContext context)
        {
            _context = context;
            People = new PersonRepository(_context);
            PersonAssociations = new PersonAssociationRepository(_context);
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