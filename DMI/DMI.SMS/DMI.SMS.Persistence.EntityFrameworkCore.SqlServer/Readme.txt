Steps:
1) Install the nuget package Microsoft.EntityFrameworkCore.SqlServer
2) Install the nuget package Microsoft.EntityFrameworkCore.Tools
3) Create the StationInformationConfiguration class
4) Create the ConnectionStringProvider class (home made)
5) Create the SmsDbContext class
6) Open the Package Manager Console window
7) In the Package Manager Console window: Execute this line at the prompt:
     add-migration InitialMigration
8) In the Package Manager Console window: Execute this line at the prompt:
     update-database
9) Use Sql Server Management Studio to verify that the database has been created

Notice:
* Apparently you dont need the DbConfigurationType construct you used for the EntityFramework (i.e. not Core) project
* You dont need to execute enable-migrations like you did before