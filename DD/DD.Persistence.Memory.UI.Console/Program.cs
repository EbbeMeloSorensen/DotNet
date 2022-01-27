// See https://aka.ms/new-console-template for more information

using System;
using System.Linq;
using DD.Domain;
using DD.Persistence.Memory;

UnitOfWorkFactory _unitOfWorkFactory = new UnitOfWorkFactory();

void CreateCreatureType(
    string name)
{
    using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
    {
        var creatureType = new CreatureType(name, 8, 7, 17, 0, 6, null);

        unitOfWork.CreatureTypes.Add(creatureType);
        unitOfWork.Complete();
    }
}

void CountAllCreatureTypes()
{
    using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
    {
        System.Console.WriteLine($"Creatures in repository: {unitOfWork.CreatureTypes.CountAll()}");
    }
}

void ListAllCreatureTypes()
{
    using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
    {
        unitOfWork.CreatureTypes.GetAll()
            .ToList()
            .ForEach(c => System.Console.WriteLine(c.Name));
    }
}


Console.WriteLine("Fun with the DD.Persistence.Memory library");
CreateCreatureType("Zombie");
CountAllCreatureTypes();
ListAllCreatureTypes();
Console.WriteLine("Done");

