using System.Linq;
using DD.Domain;

namespace DD.Persistence.Npgsql.UI.Console
{
    class Program
    {
        private static UnitOfWorkFactory _unitOfWorkFactory;

        static void Main(string[] args)
        {
            System.Console.WriteLine("Fun with the DD.Persistence.Npgsql library");

            _unitOfWorkFactory = new UnitOfWorkFactory();

            //CountAllCreatureTypes();
            ListAllCreatureTypes();

            System.Console.WriteLine("Done");
        }

        static void CreateCreatureType(
            string name)
        {
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var creatureType = new CreatureType(name, 8, 7, 17, 0, 6, null);

                unitOfWork.CreatureTypes.Add(creatureType);
                unitOfWork.Complete();
            }
        }

        static void CountAllCreatureTypes()
        {
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                System.Console.WriteLine($"Creatures in repository: {unitOfWork.CreatureTypes.CountAll()}");
            }
        }

        static void ListAllCreatureTypes()
        {
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.CreatureTypes.GetAll()
                    .ToList()
                    .ForEach(c => System.Console.WriteLine(c.Name));
            }
        }
    }
}
