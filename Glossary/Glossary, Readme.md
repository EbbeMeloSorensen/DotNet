# Deployment to Heroku

## Establishing a temporary local Git repository

1) Make a new clone of the Git repository: https://github.com/EbbeMeloSorensen/DotNet such as by opening a command prompt, navigating to e.g. C:\Git\GitHub and executing:

   `git clone https://github.com/EbbeMeloSorensen/DotNet`

   Notice that you should avoid using an existing clone of the Git repository since it might very well include a number of build outputs that will make it cumbersome to copy relevant contents of the repository, which you will do later in this proces.

2) Copy the 'Glossary' folder into a new temporary folder such as C: \Temp\heroku03032024

3) In the temporary folder, delete all folders except the following 6:

   * client-app
   * Glossary.Domain
   * Glossary.Web.Persistence
   * Glossary.Web.Infrastructure
   * Glossary.Web.Application
   * Glossary.Web.API

4) Open the solution: Glossary.sln from the temporary folder in Visual Studio 2022 - it will state that a number of projects are missing, so remove them from the solution, and then build the solution. Notice that the client-app is not a part of the solution - that project is handled later in VS Code.

5) At the command prompt, navigate to the folder 'Glossary' in the temporary folder, and create a local Git repository by executing: `git init`. Then commit all the contents of the temporary folder to the local Git repository by executing `git add *` and then `git commit -m "InitialCommmit"`.

6) Close Visual Studio

7) Launch VS Code, and open the 'Glossary' folder from the temporary folder.

## Optional: Verify that the API runs locally

8. In VS Code, open the file: 'appsettings.json' from the folder: 'Glossary.Web.API' and verify that the DefaultConnection is set correctly, i.e. so that it specifies a connection string for a local RDBMS such as postgres.

   For the computer: 'MELO-BASEMENT' I usually spin up a postgres server in a Docker container where the user is 'root' and the password is also 'root'. For Melo-Home, where I dont run Docker, I connect to a local installation of a postgres server, where the user is 'postgres' and the password is 'L1on8Zebra'.

9. Open the file: 'ApplicationServiceExtensions.cs', comment in the block with the comment: 'This section is for running locally', and comment out the block with the comment 'This section is for deploying to Heroku'

10. Make sure that the RDBMS is running and that there is no database named 'Glossary_Web' (it should be created later when launching the API)

11. In VS Code, open a terminal, navigate to the folder: 'Glossary.Web.API', and execute:

    `dotnet watch run`

    Here, you should see a web page with Swagger opening in your default browser.

    If you refresh the list of databases in your local RDBMS, you should see a database with the name 'Glossary_Web', and here you can also verify that the database has been seeded with some data in the 'Records' table.

    You can also launch Postman and verify that you can call the API, such as by logging in as the user Bob.

## Optional: Verify that the client runs locally and connects to the local API

12. In VS Code, open an extra terminal, navigate to the folder: 'client-app', and execute:

    `npm install`

    in order to install the required React packages

13. In VS Code, make sure you are still in the client application folder and execute:

    `npm start`

    in order to launch the client application. Make sure you can log in as user: 'bob@test.com' with password: 'Pa$$w0rd' and inspect, create and delete records.

14. Stop the client and the API. Also make sure to discard the changes in the 'ApplicationServiceExtensions.cs' file.

## Make a production build and commit to the local Git repository

15. Delete the contents of the folder: 'wwwroot'

16. In VS Code, navigate to the folder: 'client-app' execute:

    `npm install`

    (you might have already done this when verifying that the client worked). Then execute:

     `npm run build`

    This creates a production build of the client application. Due to a postbuild construct in the package.json file, the application is also copied to the wwwroot folder. Notice that for some reason, the production build may be placed in a a subfolder of wwwroot named 'build' - this will result in a FAILED deployment. The wwwroot folder needs to contain the 2 subfolders 'assets' and 'static' as well as a number of other files such as index.html. If a build folder is generated the copy its contents to the wwwroot folder and delete the now empty build folder.

17. Commit the changes to the local Git repo.

## Prepare an application at the Cloud Service Provider Heroku

18. Open a Web Browser and navigate to Heroku://www.heroku.com. Notice that you have to use the Salesforce authenticator app on your phone to do so.

19. Click the 'Create New App' button

20. Enter a unique app name such as 'Glossary', select an appropriate region such as Europe and click the 'Create app' button.

21. Click on the 'Resources' tab and enter 'post..' in the 'Add-ons' text box. Then select 'Heroku Postgres' in the drop down list and make sure to select the plan name 'Mini', before you click the 'Submit Order Form' button.

22. Click the 'Settings' tab for the application and then click the 'Reveal Config Vars' button. Notice that there already is a DATABASE_URL variable.

23. Add a variable named 'TokenKey' and generate a strong password for it e.g. by using LastPass. Also add a variable named 'ASPNETCORE_ENVIRONMENT' with the value 'Production'.

24. In a terminal in VS Code, navigate to the 'Glossary' folder, and execute:

    `heroku login`

    and then press Enter. Now a browser opens with a 'Log In' button that you need to click and again use the Salesforce app for authentication. Notice the message in the VS Code terminal saying that login succeeded.

25. In the settings tab on Heroku notice that there is so far no buildpack assigned to the application. Don't click the 'Add buildpack' button here, but instead execute the following in the terminal of VS Code:

    `heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack -a glossary`

    Now you should see a 'Buildpack set' message in the VS Code terminal. Also notice that if you refresh the settings page for the application on the Heroku web site, you can now see a build package.

## Link the local temporary Git repository to the new Heroku application and publish

26. In the terminal of VS Code, make sure you have commited everything by executing: `git status`. Then navigate to the 'Glossary.Web.API' folder and execute:

    `heroku git:remote -a glossary`

27. Finally, in the terminal of VS Code, make sure you are still in the 'Glossary.Web.API' folder, and then execute the following:

    `git push heroku master`

    The log should look as illustrated in Appendix A.

28. Open the url listed at the bottom of the log in a browser. Log in as user bob@test.com and password Pa$$w0rd. Make sure you can inspect, create and delete records.

## Connecting to the heroku hosted database from a local WPF application

29. Build the project: Glossary.UI.WPF from the solution Glossary.sln from the original clone of the Git repository: https://github.com/EbbeMeloSorensen/DotNet, i.e. not the trimmed version in the temporary folder.

30. Deploy the WPF application locally by copying the contents of the .\Glossary.UI.WPF\bin\Debug\net6.0-windows folder to a local folder such as C:\Progs\Glossary\GUI. Open the configuration file: Glossary.UI.WPF.dll.config and overwrite the credentials with the ones from the database on Heroku. It should look like illustrated below: 

    ```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
    	<appSettings>
    		<add key="Host" value="ec2-52-215-68-14.eu-west-1.compute.amazonaws.com" />
    		<add key="Port" value="5432" />
    		<add key="Database" value="daokgg90bn0ab0" />
    		<add key="Schema" value="public" />
    		<add key="User" value="luzvprwuxtkiez" />
    		<add key="Password" value="72271326f8bab09fafb94722a3a5c996cc2abaa849c41012ab5b9d3b34f6841d" />
    	</appSettings>
    </configuration>
    ```

31. Launch the WPF application: Glossary.UI.WPF.exe. When you click the Find button, you should see the same contents as in the web application.

32. Clear the database, import the latest version of the dataset and verify that it is accessible in the web application.

## Appendix A: Log for a succesful deployment to Heroku

```
PS C:\Temp3\HerokuSjovIgen\Glossary\Glossary.Web.API> git push heroku master
Enumerating objects: 436, done.
Counting objects: 100% (436/436), done.
Delta compression using up to 4 threads
Compressing objects: 100% (369/369), done.
Writing objects: 100% (436/436), 17.69 MiB | 4.24 MiB/s, done.
Total 436 (delta 78), reused 0 (delta 0)
remote: Resolving deltas: 100% (78/78), done.
remote: Updated 353 paths from 6518acc
remote: Compressing source files... done.
remote: Building source:
remote: 
remote: -----> Building on the Heroku-22 stack
remote: -----> Using buildpack: https://github.com/jincod/dotnetcore-buildpack
remote: -----> ASP.NET Core app detected
remote: > Installing dotnet
remote: -----> Removing old cached .NET version
remote: -----> Fetching .NET SDK
remote: -----> Fetching .NET Runtime
remote: -----> Export dotnet to Path
remote: -----> Project File
remote: > /tmp/build_d494be88/Glossary.Web.API/Glossary.Web.API.csproj
remote: -----> Project Name
remote: > Glossary.Web.API
remote: > publish /tmp/build_d494be88/Glossary.Web.API/Glossary.Web.API.csproj for Release on heroku_output
remote: 
remote: Welcome to .NET 8.0!
remote: ---------------------
remote: SDK Version: 8.0.201
remote: 
remote: ----------------
remote: Installed an ASP.NET Core HTTPS development certificate.
remote: To trust the certificate, view the instructions: https://aka.ms/dotnet-https-linux
remote:
remote: ----------------
remote: Write your first app: https://aka.ms/dotnet-hello-world
remote: Find out what's new: https://aka.ms/dotnet-whats-new
remote: Explore documentation: https://aka.ms/dotnet-docs
remote: Report issues and find source on GitHub: https://github.com/dotnet/core
remote: Use 'dotnet --help' to see available commands or visit: https://aka.ms/dotnet-cli
remote: --------------------------------------------------------------------------------------
remote: MSBuild version 17.9.4+90725d08d for .NET
remote:   Determining projects to restore...
remote:   Restored /tmp/build_d494be88/Glossary.Domain/Glossary.Domain.csproj (in 130 ms).
remote:   Restored /tmp/build_d494be88/Glossary.Web.Persistence/Glossary.Web.Persistence.csproj (in 10.86 sec).
remote:   Restored /tmp/build_d494be88/Glossary.Web.Infrastructure/Glossary.Web.Infrastructure.csproj (in 51 ms).
remote:   Restored /tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj (in 56 ms).
remote: /tmp/build_d494be88/Glossary.Web.API/Glossary.Web.API.csproj : warning NU1902: Package 'System.IdentityModel.Tokens.Jwt' 6.24.0 has a known moderate severity vulnerability, https://github.com/advisories/GHSA-59j7-ghrg-fj52
remote: /tmp/build_d494be88/Glossary.Web.API/Glossary.Web.API.csproj : warning NU1902: Package 'System.IdentityModel.Tokens.Jwt' 6.24.0 has a known moderate severity vulnerability, https://github.com/advisories/GHSA-8g9c-28fc-mcx2
remote:   Restored /tmp/build_d494be88/Glossary.Web.API/Glossary.Web.API.csproj (in 11.34 sec).
remote: /tmp/build_d494be88/Glossary.Web.API/Glossary.Web.API.csproj : warning NU1902: Package 'System.IdentityModel.Tokens.Jwt' 6.24.0 has a known moderate severity vulnerability, https://github.com/advisories/GHSA-59j7-ghrg-fj52
remote: /tmp/build_d494be88/Glossary.Web.API/Glossary.Web.API.csproj : warning NU1902: Package 'System.IdentityModel.Tokens.Jwt' 6.24.0 has a known moderate severity vulnerability, https://github.com/advisories/GHSA-8g9c-28fc-mcx2
remote: /tmp/build_d494be88/Glossary.Domain/Entities/RecordAssociation.cs(13,23): warning CS8618: Non-nullable property 'SubjectRecord' must contain a non-null value when exiting constructor. Consider declaring the property as nullable. [/tmp/build_d494be88/Glossary.Domain/Glossary.Domain.csproj]
remote: /tmp/build_d494be88/Glossary.Domain/Entities/RecordAssociation.cs(15,23): warning CS8618: Non-nullable property 'ObjectRecord' must contain a non-null value when exiting constructor. Consider declaring the property as nullable. [/tmp/build_d494be88/Glossary.Domain/Glossary.Domain.csproj]
remote: /tmp/build_d494be88/Glossary.Domain/RecordAssociationExtensions.cs(62,13): warning CS8602: Dereference of a possibly null reference. [/tmp/build_d494be88/Glossary.Domain/Glossary.Domain.csproj]
remote: /tmp/build_d494be88/Glossary.Domain/RecordAssociationExtensions.cs(68,13): warning CS8602: Dereference of a possibly null reference. [/tmp/build_d494be88/Glossary.Domain/Glossary.Domain.csproj]
remote:   Glossary.Domain -> /tmp/build_d494be88/Glossary.Domain/bin/Release/netstandard2.1/Glossary.Domain.dll
remote: /tmp/build_d494be88/Glossary.Web.Persistence/DataContext.cs(9,16): warning CS8618: Non-nullable property 'Records' must contain a non-null 
value when exiting constructor. Consider declaring the property as nullable. [/tmp/build_d494be88/Glossary.Web.Persistence/Glossary.Web.Persistence.csproj]
remote: /tmp/build_d494be88/Glossary.Web.Persistence/DataContext.cs(9,16): warning CS8618: Non-nullable property 'RecordAssociations' must contain 
a non-null value when exiting constructor. Consider declaring the property as nullable. [/tmp/build_d494be88/Glossary.Web.Persistence/Glossary.Web.Persistence.csproj]
remote:   Glossary.Web.Persistence -> /tmp/build_d494be88/Glossary.Web.Persistence/bin/Release/net6.0/Glossary.Web.Persistence.dll
remote: /tmp/build_d494be88/Glossary.Web.Application/Core/AppException.cs(5,74): warning CS8625: Cannot convert null literal to non-nullable reference type. [/tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj]
remote: /tmp/build_d494be88/Glossary.Web.Application/Core/Result.cs(6,14): warning CS8618: Non-nullable property 'Value' must contain a non-null value when exiting constructor. Consider declaring the property as nullable. [/tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj]
remote: /tmp/build_d494be88/Glossary.Web.Application/Core/Result.cs(7,19): warning CS8618: Non-nullable property 'Error' must contain a non-null value when exiting constructor. Consider declaring the property as nullable. [/tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj]
remote: /tmp/build_d494be88/Glossary.Web.Application/Records/Delete.cs(27,13): warning CS8634: The type 'Glossary.Domain.Entities.Record?' cannot be used as type parameter 'TEntity' in the generic type or method 'DbContext.Remove<TEntity>(TEntity)'. Nullability of type argument 'Glossary.Domain.Entities.Record?' doesn't match 'class' constraint. [/tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj]
remote: /tmp/build_d494be88/Glossary.Web.Application/Records/Details.cs(39,46): warning CS8604: Possible null reference argument for parameter 'value' in 'Result<RecordDto> Result<RecordDto>.Success(RecordDto value)'. [/tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj]
remote: /tmp/build_d494be88/Glossary.Web.Application/Records/Create.cs(15,23): warning CS8618: Non-nullable property 'Record' must contain a non-null value when exiting constructor. Consider declaring the property as nullable. [/tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj]
remote: /tmp/build_d494be88/Glossary.Web.Application/Records/Edit.cs(40,40): warning CS8603: Possible null reference return. [/tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj]
remote: /tmp/build_d494be88/Glossary.Web.Application/Records/Edit.cs(14,23): warning CS8618: Non-nullable property 'Record' must contain a non-null value when exiting constructor. Consider declaring the property as nullable. [/tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj]
remote: /tmp/build_d494be88/Glossary.Web.Application/Records/List.cs(14,29): warning CS8618: Non-nullable property 'Params' must contain a non-null value when exiting constructor. Consider declaring the property as nullable. [/tmp/build_d494be88/Glossary.Web.Application/Glossary.Web.Application.csproj]
remote:   Glossary.Web.Application -> /tmp/build_d494be88/Glossary.Web.Application/bin/Release/net6.0/Glossary.Web.Application.dll
remote: /tmp/build_d494be88/Glossary.Web.Infrastructure/Security/UserAccessor.cs(18,16): warning CS8602: Dereference of a possibly null reference. 
[/tmp/build_d494be88/Glossary.Web.Infrastructure/Glossary.Web.Infrastructure.csproj]
remote:   Glossary.Web.Infrastructure -> /tmp/build_d494be88/Glossary.Web.Infrastructure/bin/Release/net6.0/Glossary.Web.Infrastructure.dll        
remote: /tmp/build_d494be88/Glossary.Web.API/Startup.cs(29,17): warning CS0618: 'FluentValidationMvcConfiguration.RegisterValidatorsFromAssemblyContaining<T>(Func<AssemblyScanner.AssemblyScanResult, bool>, ServiceLifetime, bool)' is obsolete: 'RegisterValidatorsFromAssemblyContaining is deprecated. Call services.AddValidatorsFromAssemblyContaining<T> instead, which has the same effect. See https://github.com/FluentValidation/FluentValidation/issues/1963' [/tmp/build_d494be88/Glossary.Web.API/Glossary.Web.API.csproj]
remote: /tmp/build_d494be88/Glossary.Web.API/Startup.cs(22,9): warning CS0618: 'FluentValidationMvcExtensions.AddFluentValidation(IMvcBuilder, Action<FluentValidationMvcConfiguration>)' is obsolete: 'Calling AddFluentValidation() is deprecated. Call services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters() instead, which has the same effect. For details see https://github.com/FluentValidation/FluentValidation/issues/1965' [/tmp/build_d494be88/Glossary.Web.API/Glossary.Web.API.csproj]
remote:   Glossary.Web.API -> /tmp/build_d494be88/Glossary.Web.API/bin/Release/net6.0/linux-x64/Glossary.Web.API.dll
remote:   Glossary.Web.API -> /tmp/build_d494be88/heroku_output/
remote:        Add web process to Procfile
remote: -----> Discovering process types
remote:        Procfile declares types -> web
remote:
remote: -----> Compressing...
remote:        Done: 141.4M
remote: -----> Launching...
remote:        Released v7
remote:        https://glossary-f08a43d7cf4b.herokuapp.com/ deployed to Heroku
remote:
remote: Verifying deploy... done.
To https://git.heroku.com/glossary.git
 * [new branch]      master -> master
PS C:\Temp3\HerokuSjovIgen\Glossary\Glossary.Web.API>
```

## Appendix B: Troubleshooting tips

When I started writing this guide the 3rd of March 2024 I had just succeeded in deploying the Glossary web application to Heroku, using the MELO_HOME computer. I then deleted the application and set out to do a clean deployment from the MELO_BASEMENT computer, while documenting each step along the way. At the final step I got an error about something being rejected, and I couldn't figure out what was wrong. I suspected that it had something to do with being in the wrong folder when executing git init, heroku git:remote, or git push heroku master. I tried again the 4th of March from the MELO_HOME computer and succeeded. The command git init should be called from the 'Glossary' folder, and I am quite sure I called the latter to commands from the Glossary.Web.API folder, but I am not sure if it can be called from the 'Glossary' folder as well.

Update 5th of March 2024. I tried and failed to install the PR application. I just got an internal server error with error code 500 and no details. When investigating, I realized that it was because the production build of the client application was placed in a folder named 'build' instead of the wwwroot folder, so this may be a cause of an internal server error.

If you get a [remote rejected] error when trying to push to heroku, it may be because you forgot to specify a build pack for the application or because you entered the path for the build pack incorrectly.
