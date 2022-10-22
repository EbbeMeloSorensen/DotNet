# Backend

### Making a new migration after having changed the Domain and Persistence projects

`dotnet ef migrations add InitialMigration -p Persistence -s API`

(In the main folder, i.e. not the API folder. Remember to recreate the database, e.g. when using Sqlite afterwards)

### Running the backend

`dotnet watch run`

(in the 'API' folder)

### Just compiling the backend

`dotnet build`

(in the 'API' folder)

# Frontend

### Preparing the Frontend

`npm install`

(In the client-app folder)

### Running the Frontend

`npm start`

(In the client-app folder)

# Deployment to Heroku

This is also relevant after having updated the migration

1) Log ind på https://www.heroku.com/
2) Slet en evt tidligere web-applikation, f.eks. "prebsi"
   1) Klik på applikationen, så på settings og scroll så ned og klik på "Delete app"-knappen
   2) Indtast applikationens navn som bekræftelse og klik på Delete
3) Lav en ny web applikation
   1) På Dashboardet, hvor der er en liste over ens applikationer: Klik på "New" og derefter på "Create new app"
   2) Giv den et unikt navn, f.eks. "prebsi", vælg en region, gerne Europe og klik på "Create app"
   3) Installer evt Heroku Git (det skal ikke gøres hver gang man deployerer). Det gøres ved at downloade og eksekvere 64-bit installeren for Windows fra https://devcenter.heroku.com/articles/heroku-cli
   4) I VS code: Åbn projekt-folderen og et terminalvindue. I root folderen for Git repositoryet, dvs DotNet: Eksekver: heroku login og klik derefter på Enter. Så popper der en web page op, hvor man skal klikke på en "login"-knap.
   5) I VS code: Eksekver `heroku git:remote -a prebsi`
   6) Klik på "Resources"-tabben (for web applikationen) i Heroku. Skriv postgre i "Add-ons"-feltet og vælg derefter "Heroku Postgres" i listen. Vælg et "Plan name", gerne "Hobby Dev - Free" og klik så på "Submit Order Form". Så skulle der gerne poppe et "Heroku Postgres"-item op i listen under Resources, som man kan klikke på, f.eks. for at inspicere data (under Settings) til brug for en connection string. Bemærk, at man også her har mulighed for at resette eller destroye databasen.
   7) Naviger tilbage til Settings-page for selve web applikationen og klik på "Reveal Config Vars"-knappen. Bemærk, at der allerede er en variabel for DATABASE_URL.
   8) Tilføj en variabel ved navn "TokenKey" og brug en random password generator, f.eks. LastPass til at lave en værdi for den.
   9) Tilføj også en variabel ved navn "ASPNETCORE_ENVIRONMENT" og sæt dens værdi til "Production".
   10) Nu skal vi så tilføje en såkaldt build pack. Bemærk, at der er en "Add buildpack"-knap under Config Vars - den skal vi imidlertid *ikke* bruge. I stedet eksekverer vi følgende i VS Code-terminal-vinduet: `heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack` (det er kopieret fra ".NET Core edge"-sektionen af siden "https://github.com/jincod/dotnetcore-buildpack"). Det skulle gerne udvirke, at der så vises en build pack på Heroku-web pagen (efter at man har refreshet siden).
   11) Sørg for at have committet alle ændringer til GitHub
   12) Slet folderen "wwwroot" under API og commit også dette til GitHub
   13) I VS Code: Åbn et terminalvindue, naviger til client-app folderen, og eksekver `npm run build`. Bemærk, at den kopierer resultatet hen i wwwroot-folderen pga den "postbuild" setting, som vi har lavet i package.json-filen.
   14) Commit igen til GitHub, så vi får det nye production build med.
   15) Eksekver følgende ved root-folderen (for git repositoryet, dvs ikke for solutionen) i VS Code terminalvinduet: `git subtree push --prefix PR heroku main`. (hvis man har et repository, der kun indeholder ens web applikation, skriver man sædvanligvis git push heroku main, men i dette tilfælde indeholder repositoryet en del andre ting - derfor er vi nødt til at bruge subtree. Bemærk i øvrigt at stien er PR og ikke PR.Web.API, da den skal kunne bygge hele solutionen)
   16) Verificer, at applikationen er deployeret succesfuldt ved at indtaste https://prebsi.herokuapp.com i en browser. Man skulle gerne kunne logge ind som bob@test.com osv.
4) I den lokale pgadmin-klient - opdater en evt forbindelse til Heroku databasen
5) På Heroku-webpagen find credentials for databasen og indtast i konfigurationsfilen (PR.UI.WPF.dll.config) for desktop-applikationen. Opdater også credentials for forbindelsen i den lokale pgadmin-klient.

## Opdatering af eksisterende applikation uden at man har ændret databasen

1. Sikr, at alle ændringer af projektet (backend og frontend) er comittet til GitHub
2. I VS Code: I API-folderen - slet folderen wwwroot og commit igen til GitHub
3. I VS Code: Åbn et terminalvindue, naviger til client-app folderen, og eksekver `npm run build`. Bemærk, at den kopierer resultatet hen i wwwroot-folderen pga den "postbuild" setting, som vi har lavet i package.json-filen.
4. Commit igen til GitHub, så vi får det nye production build med.
5. I VS code: Åbn projekt-folderen og et terminalvindue. I root folderen (f.eks. ReactSlim - ikke hverken API eller client-app): Eksekver: heroku login og klik derefter på Enter. Så popper der en web page op, hvor man skal klikke på en "login"-knap.
6. Eksekver følgende ved root-folderen i VS Code terminalvinduet: `git push heroku main`.

**NB: Starting November 28th, 2022, free Heroku Dynos, free Heroku Postgres, and free Heroku Data for Redis® will no longer be available.**

