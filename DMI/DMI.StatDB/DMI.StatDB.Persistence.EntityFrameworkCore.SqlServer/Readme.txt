Steps:
1) Install the nuget package Microsoft.EntityFrameworkCore.SqlServer
2) Install the nuget package Microsoft.EntityFrameworkCore.Tools
3) Create the StationConfiguration class
4) Create the ConnectionStringProvider class (home made)
5) Create the StatDbContext class
6) Set the project: DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer as the startup project
7) Open the Package Manager Console window, and set the default project to DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer
8) In the Package Manager Console window: Execute this line at the prompt:
     add-migration InitialMigration
9) In the Package Manager Console window: Execute this line at the prompt:
     update-database
10) Use Sql Server Management Studio to verify that the database has been created

Notice:
* Apparently you dont need the DbConfigurationType construct you used for the EntityFramework (i.e. not Core) project
* You dont need to execute enable-migrations like you did before