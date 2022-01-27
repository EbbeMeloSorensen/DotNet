// See https://aka.ms/new-console-template for more information

using System;
using System.Linq;
using DD.Domain;
using DD.Persistence.Npgsql;

UnitOfWorkFactory _unitOfWorkFactory = new UnitOfWorkFactory();

void ListAllCreatureTypes()
{
    using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
    {
        unitOfWork.CreatureTypes.GetAll()
            .ToList()
            .ForEach(c => System.Console.WriteLine(c.Name));
    }
}

Console.WriteLine("Fun with the DD.Persistence.Npgsql library");
ListAllCreatureTypes();
Console.WriteLine("Done");
