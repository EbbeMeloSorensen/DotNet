using System;
using Craft.Persistence;

namespace DMI.SMS.Persistence
{
    public interface IRepositoryType2<TEntity> : IRepository<TEntity> where TEntity : class
    {
        void RemoveLogically(
            TEntity entity,
            DateTime transactionTime);

        void Supersede(
            TEntity entity,
            DateTime transactionTime,
            string user);
    }
}
