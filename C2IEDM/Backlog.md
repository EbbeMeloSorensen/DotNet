Done
* Sørg for at DateEstablished og DateClosed følger henholdsvis From og To, når man opretter en ny observing facility

Todo:
* DateEstablished og DateClosed skal ikke vises i brugergrænsefladen, men følge From og To for de underliggende Geospatial Locations.
  Det gælder både Create-dialogen og detalje viewet
* I list viewet skal man kun vise de observing facilities, hvor der gælder at time of interest ligger mellem hvis establishing date 
  og evt closing

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

Når man laver en ny observing Facility, skal den også have en lokation. Brugeroplevelsen kan være, at man vælger New og så får en modeless besked om at man 
skal klikke på kortet
Todo: Lav noget, der svarer til ListLines, blot i C2IEDM.Persistence
Todo: Kig på ListLines som inspiration til at lave noget tilsvarende for List AbstractEnvironmentalMonitoringFacilities - check det med Postman
Todo: Introducer et mode, hvor der ikke er en Find knap
Todo: Mulighed for at maintaine aspect ratio under navigation i kort
Todo: Introducer en mulighed for at importere sms stationer
Todo: Når man har klikket et sted, så skal der vises et punkt
Todo: Koordinater skal med ind i dialogboksen
Todo: Når man klikker ok efter at have angivet egenskaber for en observing facility, så skal der oprettes et punkt (dvs en geospatiel lokation) i databasen
Todo: Hiv seneste lokation op for ObservingFacilities for at populere map viewet
Todo: Indfør et mode, hvor find knappen skjules, og hvor listen opdateres, lige så snart man ændrer noget i filteret (som om man altid bare klikker i vildskab på den)
Overvej lige om ikke en geospatiel lokation bør være en mange til mange relation i stedet for en simpel child af AbstractEnvironmentalMonitoringFacility
Hvordan er det lige i dmi.data.studio? der vises alle de stationer vist, som også er i listen, i map viewet. Sørg lige for at det er ordentligt - gerne som i den
gamle BMS application. Også fint hvis man kan operere med at ting er selected 
