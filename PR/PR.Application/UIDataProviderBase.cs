using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Craft.Logging;
using PR.Domain;
using PR.IO;
using PR.Persistence;
using Person = PR.Domain.Entities.Person;
using PersonAssociation = PR.Domain.Entities.PersonAssociation;

namespace PR.Application
{
    public abstract class UIDataProviderBase : IUIDataProvider
    {
    }
}
