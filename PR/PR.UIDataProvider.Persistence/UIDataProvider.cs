using PR.Persistence;
using PR.IO;
using PR.Application;

namespace PR.UIDataProvider.Persistence
{
    public class UIDataProvider : UIDataProviderBase
    {
        public override IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
        }
    }
}
