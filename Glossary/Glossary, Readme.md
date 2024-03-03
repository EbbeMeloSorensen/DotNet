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

    This creates a production build of the client application. Due to a postbuild construct in the package.json file, the application is also copied to the wwwroot folder

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

    `heroku buildpacks:set https://github.comjincod/dotnetcore-buildpack -a glossary`

    Now you should see a 'Buildpack set' message in the VS Code terminal. Also notice that if you refresh the settings page for the application on the Heroku web site, you can now see a build package.

## Link the local temporary Git repository to the new Heroku application and publish

26. In the terminal of VS Code, make sure you have commited everything by executing: `git status`. Then navigate to the 'Glossary.Web.API' folder and execute:

    `heroku git:remote -a glossary`

27. Finally, execute the following at the terminal in VS Code:

    `git push heroku master`

28. ..så gumler den lidt, og så får jeg kraftedeme en "remote rejected" fejl!!??? Det virkede ellers tidligere på MELO-HOME. Hvad fanden i helvede er forskellen?!





