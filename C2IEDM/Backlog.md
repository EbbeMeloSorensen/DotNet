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
* En WPF baseret applikation, hvor man kan vedligeholde en samling observing facilities i en liste. I første omgang uden geospatial locations.
* Prøv at lave det som du plejer, dvs med forskellige repository plugins, men prøv også gerne at lave en, der trækket på APIet. Du har engang
* gjort noget tilsvarende, hvor du trak på Frie Data servicen
Overvej at bruge en decorator for at slippe for at få de supersedede med
Egentlig skal du jo bare have den samme simple where klausul på, som du også bruger i Web.Application-projektet.
Og mht at hive children op skal du jo alligevel have dedikerede methods på banen lige som du allerede har i PR med GetPeopleIncludingAssociations
* 
WPF applikation
- Create Observing Facility (OK)
- Delete  (OK)
- Vis timeStamps i TimeSeriesViewModel, dem, der er i databasen ved opstart (OK)
- Updater timeStamps-samlingen, når der laves nye observing facilities (OK)
- Updater timeStamps-samlingen, når der slettes eksisterende observing facilities (OK)
- Der skal være 2 modes:
	1. "Normalt mode", hvor man vedligeholder sit datasæt
	2. "Historical inspection", hvor man inspicerer tidligere versioner af databasens datasæt (du har overvejet, om historical inspection skal 
	1. være en del af filteret - det ved jeg sgu ikke rigtigt, om det skal)
	1. 
- Tidsakse-viewet skal vise NUVÆRENDE tidspunkt som en linie med en given farve
- Tidsakse-viewet skal vise historisk tidspunkt som en linie med en given farve
- Der skal være en fornuftig brugeroplevelse, når man vælger et tidspunkt i tidsakse-viewet
- En funktion, der kan kaldes for at opdatere timeseries viewet, dvs trække det frem til current
- Show historical situation (måske en del af filteret?.. eller måske en del af en mere fancy kontrol, hvor man også kan se, 
- hvornår der er lavet ændringer..)

Det kunne være fint, hvis brugregrænsefladen somehow havde 2 "tidsmaskiner" - en, hvor man kunne se, hvordan ting så ud på et givet tidspunkt
før i tiden, og en, hvor man kunne se, hvordan databasen PÅ ET TIDLIGERE TIDDSPUNKT mente, at virkeligheden så ud
Det kunne jo passende være to tidspunkter, der blev vist i hver deres felt i brugergrænsefladen. Skal man så vise det i samme view? nej det er 
nok ikke smart, men lige som det kunne være smart at have mulighed for at vælge tidspunkter i time series viewet, så kunne det være sjovt at kunne 
vælge det historiske tidspunkt, man var interesseret i, i et time series view.