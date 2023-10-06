Done
* S�rg for at DateEstablished og DateClosed f�lger henholdsvis From og To, n�r man opretter en ny observing facility
* Sorter observing facilities alfabetisk i list viewet (ellers sorteres de tilsyneladende efter, hvilke der sidst er rettet)
* Man skal ikke kunne v�lge et "historisk" tidspunkt i fremtiden
* Name filter skal kunne sl�s til og fra og ikke vises pr default
* S�rg for at man ikke har lov til at �ndre p� aspect ratio for map viewet
* Man skal kunne g� ud af retroaktivt mode ved at klikke p� de der knapper i filteret
* I list viewet skal man kun vise de observing facilities, hvor der g�lder, at time of interest ligger mellem hvis establishing date og closing date
* S�rg for at g�re tydeligt opm�rksom p�, n�r man kigger p� historisk tid - evt med farver - alla falmede kort

In progress:
* Der skal vises noget tekst, hvor det fremg�r, n�r man har at g�re med en historisk situation, gerne i en statusbar
* S�rg for at vedligeholde hj�lpestreger for de database tidslinien
  - En for Now
  - En for time of interest
* S�rg for at vedligeholde hj�lpestreger for de history tidslinien
  - En for Now
  - En for time of interest
* Introducer det mode, hvor Find-knappen ikke vises, men kaldes hver gang der sker en �ndring i filteret
* P� de 2 tidsakser skal man kunne se en linie, der angiver nuv�rende tidspunkt - gerne med l�bende opdatering

Todo:
* Details view skal vises p�nere - i h�jre side i portr�t format lige som i sms
* Du skal have et view alla det i PR for associations, som viser lokationer
* Man skal ikke kunne lave nye stationer i history mode
* Man skal ikke kunne slette noget i history mode
* Man skal ikke kunne opdatere noget i history mode
* Der skal v�re et visuelt cue om at man har skruet tilbage p� database tid
* I Tidsserie viewet skal man med nogle pileknapper kunne g� hen til n�ste linie
* Der skal v�re et mode for et tidsserieview, hvos det f�lger med current time.
* N�r man tegner streger, skal den tegne streger for et interval, der er 3 gange bredere end World Window, s� man disse streger ind, n�r man panner
* stregerne skal tegnes med en farve, der angiver, om man kaldte create, update eller delete
* N�r man klikker p� Now-knappen (ud for historisk tid) efter at have v�ret tilbage i transaktionstid, s� skal man nulstille historisk tid OG databasetid 
* Listen m� ogs� gerne f� en baggrundsfarve, der er falmet*
* Du skal kunne �ndre en observing facility ved at s�tte dens Closed Date til en dato, der ligger f�r i tiden.
  N�r det sker, skal den ogs� s�tte to time for den seneste geospatielle lokation til samme tidspunkt.
  ... Det er kun i "almindeligt" mode at man kan tilf�je eller �ndre data". I det mode ser man ogs� kun de aktive stationer, og s� skal man 
  have mulighed for at "nedl�gge" dem, dvs s�tte deres DateClosed til en dato. Det skal s� udvirke, at den seneste Geospatielle lokation ogs�
  s�ttes til samme dato. Det skal valideres, at man ikke s�tter datoen til noget i fremtiden, og ogs�, at man ikke s�tter det til noget, der
  er �ldre end From for den seneste geospatielle lokation.
* DateEstablished og DateClosed skal ikke vises i brugergr�nsefladen, men f�lge From og To for de underliggende Geospatial Locations.
  Det g�lder b�de Create-dialogen og detalje viewet
* Hvis man opretter en ny, som i �vrigt er nedlagt (og derfor ikke vises), s� skal man cleare selection
* Pr default skal den bare v�lge en dato i historisk tid - med Shift kan man s� angive, at det skal v�re mere granuleret

(Herunder roder det lidt rigeligt)
Todo:
* CRUD for ObservingFacilities supporteret af API
	- List (OK)
	- Create (OK)
	- Delete (OK)
	- Update (OK)
	- 
* CRUD for GeospatialLocations supporteret af API
	- List (OK)
	- Create
	- Delete
	- Update
	- 
* En WPF baseret applikation, hvor man kan vedligeholde en samling observing facilities i en liste. I f�rste omgang uden geospatial locations.
* Pr�v at lave det som du plejer, dvs med forskellige repository plugins, men pr�v ogs� gerne at lave en, der tr�kket p� APIet. Du har engang
* gjort noget tilsvarende, hvor du trak p� Frie Data servicen
Overvej at bruge en decorator for at slippe for at f� de supersedede med
Egentlig skal du jo bare have den samme simple where klausul p�, som du ogs� bruger i Web.Application-projektet.
Og mht at hive children op skal du jo alligevel have dedikerede methods p� banen lige som du allerede har i PR med GetPeopleIncludingAssociations
* 
WPF applikation
- Create Observing Facility (OK)
- Delete  (OK)
- Vis timeStamps i TimeSeriesViewModel, dem, der er i databasen ved opstart (OK)
- Updater timeStamps-samlingen, n�r der laves nye observing facilities (OK)
- Updater timeStamps-samlingen, n�r der slettes eksisterende observing facilities (OK)
- Der skal v�re 2 modes:
	1. "Normalt mode", hvor man vedligeholder sit datas�t
	2. "Historical inspection", hvor man inspicerer tidligere versioner af databasens datas�t (du har overvejet, om historical inspection skal 
	1. v�re en del af filteret - det ved jeg sgu ikke rigtigt, om det skal)
	1. 
- Tidsakse-viewet skal vise NUV�RENDE tidspunkt som en linie med en given farve
- Tidsakse-viewet skal vise historisk tidspunkt som en linie med en given farve
- Der skal v�re en fornuftig brugeroplevelse, n�r man v�lger et tidspunkt i tidsakse-viewet
- En funktion, der kan kaldes for at opdatere timeseries viewet, dvs tr�kke det frem til current
- Show historical situation (m�ske en del af filteret?.. eller m�ske en del af en mere fancy kontrol, hvor man ogs� kan se, 
- hvorn�r der er lavet �ndringer..)

Det kunne v�re fint, hvis brugregr�nsefladen somehow havde 2 "tidsmaskiner" - en, hvor man kunne se, hvordan ting s� ud p� et givet tidspunkt
f�r i tiden, og en, hvor man kunne se, hvordan databasen P� ET TIDLIGERE TIDDSPUNKT mente, at virkeligheden s� ud
Det kunne jo passende v�re to tidspunkter, der blev vist i hver deres felt i brugergr�nsefladen. Skal man s� vise det i samme view? nej det er 
nok ikke smart, men lige som det kunne v�re smart at have mulighed for at v�lge tidspunkter i time series viewet, s� kunne det v�re sjovt at kunne 
v�lge det historiske tidspunkt, man var interesseret i, i et time series view.

N�r man laver en ny observing Facility, skal den ogs� have en lokation. Brugeroplevelsen kan v�re, at man v�lger New og s� f�r en modeless besked om at man 
skal klikke p� kortet
Todo: Lav noget, der svarer til ListLines, blot i C2IEDM.Persistence
Todo: Kig p� ListLines som inspiration til at lave noget tilsvarende for List AbstractEnvironmentalMonitoringFacilities - check det med Postman
Todo: Introducer et mode, hvor der ikke er en Find knap
Todo: Mulighed for at maintaine aspect ratio under navigation i kort
Todo: Introducer en mulighed for at importere sms stationer
Todo: N�r man har klikket et sted, s� skal der vises et punkt
Todo: Koordinater skal med ind i dialogboksen
Todo: N�r man klikker ok efter at have angivet egenskaber for en observing facility, s� skal der oprettes et punkt (dvs en geospatiel lokation) i databasen
Todo: Hiv seneste lokation op for ObservingFacilities for at populere map viewet
Todo: Indf�r et mode, hvor find knappen skjules, og hvor listen opdateres, lige s� snart man �ndrer noget i filteret (som om man altid bare klikker i vildskab p� den)
Overvej lige om ikke en geospatiel lokation b�r v�re en mange til mange relation i stedet for en simpel child af AbstractEnvironmentalMonitoringFacility
Hvordan er det lige i dmi.data.studio? der vises alle de stationer vist, som ogs� er i listen, i map viewet. S�rg lige for at det er ordentligt - gerne som i den
gamle BMS application. Ogs� fint hvis man kan operere med at ting er selected 
