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