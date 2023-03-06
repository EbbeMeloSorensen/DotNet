﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Craft.Persistence.EntityFrameworkCore
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        protected Repository(
            DbContext context)
        {
            Context = context;
        }

        public int CountAll()
        {
            return Context.Set<TEntity>().Count();
        }

        public int Count(
            Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Count(predicate);
        }

        public int Count(
            IList<Expression<Func<TEntity, bool>>> predicates)
        {
            var predicate = predicates.Aggregate((c, n) => c.And(n));
            return Context.Set<TEntity>().Count(predicate);
        }

        //public TEntity Get(
        //    decimal id)
        //{
        //    return Context.Set<TEntity>().Find(id);
        //}

        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> Find(
            Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public IEnumerable<TEntity> Find(
            IList<Expression<Func<TEntity, bool>>> predicates)
        {
            var predicate = predicates.Aggregate((c, n) => c.And(n));
            return Context.Set<TEntity>().Where(predicate);
        }

        public TEntity SingleOrDefault(
            Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public void Add(
            TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void AddRange(
            IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
        }

        public void Remove(
            TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public abstract void Clear();

        public abstract void Update(
            TEntity entity);

        public abstract void UpdateRange(
            IEnumerable<TEntity> entities);

        public void RemoveRange(
            IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }

        public void Load(
            IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
            Context.SaveChanges();

            /*
            // Notice that we use Z.EntityFramework.Extensions for this ..... 
            // (Dette gjorde jeg for lang tid siden for at addressere problemet med at man ikke uden videre kan sætte værdien
            // for identity attributes. Det er imidlertid ikke uden implikationer, f.eks. derved at Z.EntityFramework er et
            // kommercielt tool, der tilsyneladende kun virker i en begrænset periode efter at man har bygget sin applikation

            var skip = 0;
            var bufferCapacity = 10;
            var bufferSize = 0;

            do
            {
                var buffer = entities
                    .Skip(skip)
                    .Take(bufferCapacity)
                    .ToList();

                bufferSize = buffer.Count();

                if (bufferSize > 0)
                {
                    Context.BulkInsert(
                        buffer,
                        options => options.InsertKeepIdentity = true);
                }

                skip += bufferCapacity;

                Console.Write(".");
            } while (bufferSize > 0);

            // Original
            //Context.BulkInsert(entities, options => options.InsertKeepIdentity = true);
            */
        }
    }
}