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

### Setting Up the Frontend

Se, hvordan det gøres i readme filen for Reactivities projektet under ASP.NET

### Preparing the Frontend after cloning from GitHub Repository

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
   4) I VS code: Åbn projekt-folderen og et terminalvindue. I root folderen for solutionen, dvs PR, dvs DotNet: Eksekver: heroku login og klik derefter på Enter. Så popper der en web page op, hvor man skal klikke på en "login"-knap.
   5) I VS code: Naviger hen til folderen for API-projektet, dvs PR.Web.API, og eksekver `heroku git:remote -a prebsi`
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

## Noter vedr deployering til Heroku

Denne sekvens brugte jeg, da jeg første gang lykkedes med at deployere til Heroku:

cd .\PR.Web.API
heroku git:remote -a prubsi
git subtree push --prefix PR.Web.API heroku main     (nej)
git subtree push --prefix PR/PR.Web.API heroku main  (nej)
git subtree push --prefix PR heroku main
cd PR
heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack




H: I terminalvinduet skal du stå i folderen PR.Web.API, når du kalder heroku git:remote -a prebsi
Bemærk, at du IKKE skal bruge nogen af Craft-bibliotekerne, så når den brokker sig over, at den ikke kan finde dem, skyldes det muligvis, at man har stået et forkert sted, når man har eksekveret heroku git:remote

Dette er den sidste kommando man skal eksekvere, og man skal stå i DotNet-folderen
git subtree push --prefix PR heroku main

## Flere noter vedr deployering til Heroku

23-12-2022

Jeg var opmærksom på at Heroku overgik til at kræve moderat betaling for at hoste ens applikationer ved udgangen af november 2022, så derfor var jeg inde på Heroku websitet og ændre databaserne fra Free til Mini, hvilket, så vidt jeg forstår, indebærer, at der skal betales 5 dollars om måneden. Applikationen virkede også efter ændringen, men ca ved månedsskiftet skete der alligevel det, at web applikationen meldte fejl, på trods af at databasen virkede fint, således at jeg stadig kunne bruge den wpf-baserede desktop applikation. I går, dvs den 22. december, prøvede jeg så at redeployere applikationen ved at gennemløbe den procedure, som jeg har skrevet i dette dokument. Det lykkedes ikke - jeg kunne af en eller anden årsag ikke komme uden om nogle fejlbeskeder, der mindede om dem, jeg havde set tidligere, hvor den brokkede sig over, at den ikke kunne finde Craft-projekterne, selv om de jo altså ikke bliver brugt af web applikationen. I sin tid løste jeg det som beskrevet ovenfor ved at basere mig på git subtree, men det hjalp ikke denne gang.

Jeg lykkedes imidlertid med denne procedure:

1) Opret en ny applikation hos Heroku som beskrevet ovenfor, men bemærk, at man nu tilsyneladende skal angive en applikation, når man tilføjer en buildpack, ved at eksekvere:

   ```
   heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack -a prebsi
   ```

2) Kopier indholdet af `PR`-folderen fra `Dotnet`-folderen (som jo altså kommer fra et Git repository med samme navn) hen i en ny folder, f.eks. ved navn `MyTempFolder`

3) Fjern fra solutionen alle projekter pånær PR.Domain samt dem,  der starter med PR.Web og slet også de pågældende projektfoldere.

4) Opret et lokalt git repo ved at eksekvere `git init` i `MyTempFolder`

5) Eksekver `git add *` og derefter `git commit -m "InitialCommit"`

6) Knyt dette lokale git repo til det remote git repo, der ligger hos Heroku efter oprettelse af en applikation der, ved at eksekvere: `heroku git:remote -a prebsi`

7) Publicer til Heroku ved at eksekvere:

   ```
   git push heroku master
   ```

   Bemærk, at jeg før skrev `main `i stedet for `master`, men det kunne jeg af en eller anden årsag ikke nu - muligvis fordi jeg mod sædvane startede med at lave et lokalt repo frem for at klone fra GitHub. Jeg googlede mig til et tip om at eksekvere `git show-ref`for at finde ud af, hvilket navn man skulle bruge.