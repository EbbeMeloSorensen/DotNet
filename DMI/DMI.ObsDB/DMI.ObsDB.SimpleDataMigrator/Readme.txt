Denne konsolapplikation læser observationer fra et filrepository eller obsdb-databasen i nanoq-miljøet
- bemærk, at det hardkodes i Application.cs om den gør det ene eller det andet
- filrepositoryet laves i øvrigt med applikationen DMI.DAL.ObsDB.UI.Console

Den skriver så til et repository, hvor der benyttes dependency injection til et plugin, som baserer sig på Entity Framework Core,
og som kan være en postgres database, en sqlite database, eller en ms sql server database