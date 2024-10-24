using System;
using System.Collections.Generic;
using System.Text;

namespace PR.Persistence
{
    public interface IUnitOfWorkFactoryVersioned : IUnitOfWorkFactory
    {
        DateTime? DatabaseTime { get; set; }
    }
}
