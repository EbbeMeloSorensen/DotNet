Done
* S�rg for at DateEstablished og DateClosed f�lger henholdsvis From og To, n�r man opretter en ny observing facility
* Sorter observing facilities alfabetisk i list viewet (ellers sorteres de tilsyneladende efter, hvilke der sidst er rettet)
* Man skal ikke kunne v�lge et "historisk" tidspunkt i fremtiden
* Name filter skal kunne sl�s til og fra og ikke vises pr default
* S�rg for at man ikke har lov til at �ndre p� aspect ratio for map viewet
* Man skal kunne g� ud af retroaktivt mode ved at klikke p� de der knapper i filteret
* I list viewet skal man kun vise de observing facilities, hvor der g�lder, at time of interest ligger mellem hvis establishing date og closing date
* S�rg for at g�re tydeligt opm�rksom p�, n�r man kigger p� historisk tid - evt med farver - alla falmede kort
* Der skal vises noget tekst, hvor det fremg�r, n�r man har at g�re med en historisk situation, gerne i en statusbar
* Ret fejlen med at det crasher, hvis man selecter flere observing facilities, der er opstillet p� forskellige dage
* Introducer det mode, hvor Find-knappen ikke vises, men kaldes hver gang der sker en �ndring i filteret
* Details view skal vises p�nere - i h�jre side i portr�t format lige som i sms
* selection control som for person list view i PR
* N�r man v�lger en observing facility, skal den vise hele dens collection af steder, hvor den har st�et
* Lav et event, som ObservingFacilitiesDetailsViewModel kan bruge til at signalere til main view model, at brugeren har
  klikket p� New, s� main kan tage sig af det (ved at bede brugeren om at klikke i map viewet)
* For en station skal den kun vise den position, der passer med time of interest
* Hvis der kun er �n Geospatial location for en observing facility, s� skal man ikke kunne slette den
* Man m� ikke kunne slette alle geospatielle lokationer
* Man skal kunne tilf�je en ekstra lokation for en eksisterende tidsserie
* S�rg for at vedligeholde hj�lpestreger for database tidslinien
  - �n for Now
  - �n for selected time
* N�r man trykker p� find skal den s� vidt muligt bibeholde selection, dvs de observing facilities, der var selected f�r, skal ogs� v�re selected efter
* Det at slette, tilf�je, eller �ndre en Geospatiel Location kan godt �ndre p� map viewet og endda master listen, s� det skal refreshes
* Der skal tilf�jes streger til samlingen af database write times, n�r man arbejder med lokationer - create, update delete
* Geospatial locations for en observing facility skal sorteres kronologisk
* N�r man laver en ny observing facility og i den forbindelse specificerer from date, s� skal den bruges b�de for
  observing facility og geospatial location
* Man skal kunne frems�ge closed observing facilities uden at hoppe tilbage i tid - s� man kan �ndre p� deres geospatial locations
* DateEstablished og DateClosed skal opdateres, n�r man arbejder med geospatial locations
  - n�r man �ndrer en
  - n�r man laver en ny geospatiel lokation
  - n�r man sletter en
* Man skal kunne �ndre en ToDate fra en given dato til null, dvs angive, at der ikke er nogen slutdato
* Fix fejl: Man ser ikke noget, hvis man beder om et historical view, der er 10 seks gammelt
* Man m� godt lave �ndringer, n�r man ser p� historik for en database, men IKKE n�r man ser p� en tidligere version af databasen
* N�r man v�lger en Database Write Time og den s� automatisk s�tter en historical time, s� skal man kunne se stregen i "historical time" viewet
* Man skal ikke kunne s�tte historical time of interest senere end database time of interest
* Den skal vise g�ldende tidspunkt i en tekstbox i brugergr�nsefladen

In progress:
* IMPORT AF SMS-STATIONER
* Selection skal ogs� vises p� map viewet
* Der skal v�re mulighed for at v�lge stationer p� map viewet
* N�r man v�lger en geospatiel location, skal det vises:
  - p� map viewet - f.eks. som en tynd streg
  - p� "historical time"-viewet som en bj�lke
* Der skal vises et ur et passende sted i brugergr�nsefladen
  - det skal opdateres, n�r man kigger p� nuv�rende situation og st� stille, n�r man kigger p� en historisk situation
* N�r man klikker til h�jre for Now-linien i et time view, skal det svare til at man klikker p� Now- og Latest-knapperne* Der skal tegnes linier der hvor en placering �ndrer sig i historical view - og det skal opdateres ved database�ndringer
* N�r man v�lger et antal geospatial locations i details viewet, skal der tegnes bj�lker til repr�sentation af dem i historical view
* De f�rn�vnte bj�lker skal kunne overlejres med smalle bj�lker, der viser, hvor der er foretaget m�linger - i stil med den gamle applikation - bare bedre fordi man kan zoome
* Man skal kunne se v�rdierne for en tidsserie
* Man skal ikke kunne �ndre p� noget i et history mode, hvor historical time of interest er sat
* Input validering, n�r man tilf�jer eller �ndrer en Geospatiel location, s� man sikrer:
  - at den ikke overlapper med de eksisterende
  - at den ikke starter eller slutter i fremtiden
* Forskellige farver for streger i tids-linealerne
* Import af SMS - ogs� alt skraldet
* Man skal kunne v�lge virkningstid og registreringstid i de der felter
* Det skal fremg� tydeligt af detaljeviewet, om man har valgt �n eller flere observing facilities
* Hvis man v�lger FLERE observing facilities, skal den IKKE vise sektionen med geospatial locations
* 2d view med tidsakser p� begge leder
* Man skal ikke kunne steppe frem, hvis det indeb�rer at den lilla "now" line tr�kkes ind
* S�rg for at den foresl�r nogle rimelige muligheder, hvis man pr�ver at �ndre From Date eller To Date
* Lav en "unleash service technician" funktion, s� man kan placere en figur, der g�r rundt efter en drunkards walk og placerer stationer
* Lav noget, s� man kan se pile mellem forskellige lokationer for en station, f.eks. n�r man v�lger dem i detalje viewet
  - man kunne f.eks. forsyne punkterne med tidsangivelser
* Brugeren skal ikke kunne �ndre DateEstablished og DateClosed - de skal altid svare til de GeospatialLocations, der h�rer til en observing facility
* S�rg for at vedligeholde hj�lpestreger for de history tidslinien
  - �n for Now
  - �n for selected time
* P� de 2 tidsakser skal man kunne se en linie, der angiver nuv�rende tidspunkt - gerne med l�bende opdatering

Todo:
* Man skal kunne importere sms data
* Man skal kunne vise tidsserier (som i DMI.Data.Studio)
* Man skal kunne vise Gant-diagrammer for forskellige tidsperioder (som i DMI.Data.Studio)
* Man skal kunne vise perioder, hvor der er foretaget m�linger (som i DMI.Data.Studio)
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
