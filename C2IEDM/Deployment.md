## Deployering af C2IEDM til Heroku

1) Sikr, at det virker lokalt med en opsætning til postgres, dvs kør API'en med dotnet watch run og verificer, at den genererer databasen. Sikr, at man kan kalde APIen med postman. Sikr også, at man kan køre client applikationen med npm start
2) Slet folderen wwwroot under C2IEDM.Web.API
3) I VS Code: Åbn et terminalvindue, naviger til client-app folderen, og eksekver `npm run build`. Bemærk, at den kopierer resultatet hen i wwwroot-folderen pga den "postbuild" setting, som vi har lavet i package.json-filen.
4) Kopier indholdet af folderen Dotnet\C2IEDM hen til en midlertidig folder, f.eks. C:\Temp\heroku. Kopier også de 2 foldere: Craft.Persistence og Craft.Persistence.EntityFrameworkCore samme sted hen. Ret afhænggheder til, så det bygger og slet de foldere, der ikke skal bruges. Sikr gerne igen at det stadig virker.
5) I C2IEDM.Web.API: Åbn filen ApplicationServiceExtensions og udkommenter her blokken med kommentaren "This section is for running locally", og indkommenter blokken med kommentaren: "This section is for deploying to Heroku"
6) Log ind på Heroku hjemmesiden
7) På Heroku: Under Personal (hvor man kan se sine applikationer) klik på New-knappen og vælg Create new app. Vælg et navn alla c2iedm, vælg Europe som region og klik på 'Create app'-knappen.
8) Installer evt Heroku Git (det skal ikke gøres hver gang man deployerer). Det gøres ved at downloade og eksekvere 64-bit installeren for Windows fra https://devcenter.heroku.com/articles/heroku-cli
9) Launch VS code med den valgte folder (f.eks. C:\Temp\heroku) som aktiv folder. Åbn et terminalvindue og eksekver `heroku login`. Bemærk, at man skal brugen den der SalesForce app på telefonen. Så skulle det gerne fremgå, at man er logget ind på Heroku.
10) I VS Codes terminalvindue: Sikr, at man står i root folderen (f.eks. C:\Temp\heroku). Opret nu et lokalt git repo ved at eksekvere `git init`.
11) Eksekver `git add *` og derefter `git commit -m "InitialCommit"` for at comitte projektet til det lokale git repo.
12) Eksekver `heroku git:remote -a c2iedm` for at associere det lokale git repository med det, der ligger på Heroku.
13) Klik på "Resources"-tabben (for web applikationen) i Heroku. Skriv postgre i "Add-ons"-feltet og vælg derefter "Heroku Postgres" i listen. Vælg et "Plan name", gerne "Mini - max 5 dollars monthly" og klik så på "Submit Order Form". Så skulle der gerne poppe et "Heroku Postgres"-item op i listen under Resources, som man kan klikke på, f.eks. for at inspicere data (under Settings) til brug for en connection string. Bemærk, at man også her har mulighed for at resette eller destroye databasen.
14) Naviger tilbage til Settings-page for selve web applikationen og klik på "Reveal Config Vars"-knappen. Bemærk, at der allerede er en variabel for DATABASE_URL.
15) Tilføj en variabel ved navn "TokenKey" og brug en random password generator, f.eks. LastPass til at lave en værdi for den - sæt den f.eks. til 32 karakterer.
16) Tilføj også en variabel ved navn "ASPNETCORE_ENVIRONMENT" og sæt dens værdi til "Production"
17) Nu skal vi så tilføje en såkaldt build pack. Bemærk, at der er en "Add buildpack"-knap under Config Vars - den skal vi imidlertid *ikke* bruge. I stedet eksekverer vi følgende i VS Code-terminal-vinduet: `heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack -a c2iedm` (det er kopieret fra ".NET Core edge"-sektionen af siden "https://github.com/jincod/dotnetcore-buildpack"). Det skulle gerne udvirke, at der så vises en build pack på Heroku-web pagen (efter at man har refreshet siden).
18) Publicer til Heroku ved at eksekvere: `git push heroku master`

Nu skulle den så gerne skrive en langs smøre, der vidner om at den bygger applikationen og som ender med "deployed to Heroku". Åbn den url, som den viser, i en browser - så skulle applikationen gerne virke.

