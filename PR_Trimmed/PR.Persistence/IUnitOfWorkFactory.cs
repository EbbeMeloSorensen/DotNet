using Craft.Logging;

namespace PR.Persistence
{
    public interface IUnitOfWorkFactory
    {
        ILogger Logger { get; set; }

        void Initialize(
            bool versioned);

        IUnitOfWork GenerateUnitOfWork();

        void Reseed();
    }
}
