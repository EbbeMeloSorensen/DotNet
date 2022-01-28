using DMI.SMS.Persistence.Npgsql;

UnitOfWorkFactory _unitOfWorkFactory = new UnitOfWorkFactory();

void CountStationInformationsWithOTANISKInTheName()
{
    var host = "localhost";
    var port = 5432;
    var schema = "public";
    var database = "SMS";
    var user = "postgres";
    var password = "L1on8Zebra";

    //var host = "172.25.7.23";
    //var port = 5432;
    //var schema = "sde";
    //var database = "sms_prod";
    //var user = "ebs";
    //var password = "Vm6PAkPh";
    ConnectionStringProvider.Initialize(host, port, database, schema, user, password);

    //Hej med dig steg!!!!!!
    var _nameFilterInUppercase = "Esbjerg";

    using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
    {

        //var stationInformations = unitOfWork.StationInformations
        //    .Find(s => s.StationName.ToUpper().Contains(_nameFilterInUppercase))
        //    .ToList();

        Console.WriteLine($"Count: {unitOfWork.StationInformations.CountAll()}");
    }
}

Console.WriteLine("Hello, World!");

Console.WriteLine("Fun with the DD.Persistence.Npgsql library");
CountStationInformationsWithOTANISKInTheName();
Console.WriteLine("Done");
