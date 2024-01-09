Done
* Sørg for at DateEstablished og DateClosed følger henholdsvis From og To, når man opretter en ny observing facility
* Sorter observing facilities alfabetisk i list viewet (ellers sorteres de tilsyneladende efter, hvilke der sidst er rettet)
* Man skal ikke kunne vælge et "historisk" tidspunkt i fremtiden
* Name filter skal kunne slås til og fra og ikke vises pr default
* Sørg for at man ikke har lov til at ændre på aspect ratio for map viewet
* Man skal kunne gå ud af retroaktivt mode ved at klikke på de der knapper i filteret
* I list viewet skal man kun vise de observing facilities, hvor der gælder, at time of interest ligger mellem hvis establishing date og closing date
* Sørg for at gøre tydeligt opmærksom på, når man kigger på historisk tid - evt med farver - alla falmede kort
* Der skal vises noget tekst, hvor det fremgår, når man har at gøre med en historisk situation, gerne i en statusbar
* Ret fejlen med at det crasher, hvis man selecter flere observing facilities, der er opstillet på forskellige dage
* Introducer det mode, hvor Find-knappen ikke vises, men kaldes hver gang der sker en ændring i filteret
* Details view skal vises pænere - i højre side i portræt format lige som i sms
* selection control som for person list view i PR
* Når man vælger en observing facility, skal den vise hele dens collection af steder, hvor den har stået
* Lav et event, som ObservingFacilitiesDetailsViewModel kan bruge til at signalere til main view model, at brugeren har
  klikket på New, så main kan tage sig af det (ved at bede brugeren om at klikke i map viewet)
* For en station skal den kun vise den position, der passer med time of interest
* Hvis der kun er én Geospatial location for en observing facility, så skal man ikke kunne slette den
* Man må ikke kunne slette alle geospatielle lokationer
* Man skal kunne tilføje en ekstra lokation for en eksisterende tidsserie
* Sørg for at vedligeholde hjælpestreger for database tidslinien
  - Én for Now
  - Én for selected time
* Når man trykker på find skal den så vidt muligt bibeholde selection, dvs de observing facilities, der var selected før, skal også være selected efter
* Det at slette, tilføje, eller ændre en Geospatiel Location kan godt ændre på map viewet og endda master listen, så det skal refreshes
* Der skal tilføjes streger til samlingen af database write times, når man arbejder med lokationer - create, update delete
* Geospatial locations for en observing facility skal sorteres kronologisk
* Når man laver en ny observing facility og i den forbindelse specificerer from date, så skal den bruges både for
  observing facility og geospatial location
* Man skal kunne fremsøge closed observing facilities uden at hoppe tilbage i tid - så man kan ændre på deres geospatial locations
* DateEstablished og DateClosed skal opdateres, når man arbejder med geospatial locations
  - når man ændrer en
  - når man laver en ny geospatiel lokation
  - når man sletter en
* Man skal kunne ændre en ToDate fra en given dato til null, dvs angive, at der ikke er nogen slutdato
* Fix fejl: Man ser ikke noget, hvis man beder om et historical view, der er 10 seks gammelt
* Man må godt lave ændringer, når man ser på historik for en database, men IKKE når man ser på en tidligere version af databasen
* Når man vælger en Database Write Time og den så automatisk sætter en historical time, så skal man kunne se stregen i "historical time" viewet
* Man skal ikke kunne sætte historical time of interest senere end database time of interest
* Den skal vise gældende tidspunkt i en tekstbox i brugergrænsefladen

In progress:
* IMPORT AF SMS-STATIONER
* Selection skal også vises på map viewet
* Der skal være mulighed for at vælge stationer på map viewet
* Når man vælger en geospatiel location, skal det vises:
  - på map viewet - f.eks. som en tynd streg
  - på "historical time"-viewet som en bjælke
* Der skal vises et ur et passende sted i brugergrænsefladen
  - det skal opdateres, når man kigger på nuværende situation og stå stille, når man kigger på en historisk situation
* Når man klikker til højre for Now-linien i et time view, skal det svare til at man klikker på Now- og Latest-knapperne* Der skal tegnes linier der hvor en placering ændrer sig i historical view - og det skal opdateres ved databaseændringer
* Når man vælger et antal geospatial locations i details viewet, skal der tegnes bjælker til repræsentation af dem i historical view
* De førnævnte bjælker skal kunne overlejres med smalle bjælker, der viser, hvor der er foretaget målinger - i stil med den gamle applikation - bare bedre fordi man kan zoome
* Man skal kunne se værdierne for en tidsserie
* Man skal ikke kunne ændre på noget i et history mode, hvor historical time of interest er sat
* Input validering, når man tilføjer eller ændrer en Geospatiel location, så man sikrer:
  - at den ikke overlapper med de eksisterende
  - at den ikke starter eller slutter i fremtiden
* Forskellige farver for streger i tids-linealerne
* Import af SMS - også alt skraldet
* Man skal kunne vælge virkningstid og registreringstid i de der felter
* Det skal fremgå tydeligt af detaljeviewet, om man har valgt én eller flere observing facilities
* Hvis man vælger FLERE observing facilities, skal den IKKE vise sektionen med geospatial locations
* 2d view med tidsakser på begge leder
* Man skal ikke kunne steppe frem, hvis det indebærer at den lilla "now" line trækkes ind
* Sørg for at den foreslår nogle rimelige muligheder, hvis man prøver at ændre From Date eller To Date
* Lav en "unleash service technician" funktion, så man kan placere en figur, der går rundt efter en drunkards walk og placerer stationer
* Lav noget, så man kan se pile mellem forskellige lokationer for en station, f.eks. når man vælger dem i detalje viewet
  - man kunne f.eks. forsyne punkterne med tidsangivelser
* Brugeren skal ikke kunne ændre DateEstablished og DateClosed - de skal altid svare til de GeospatialLocations, der hører til en observing facility
* Sørg for at vedligeholde hjælpestreger for de history tidslinien
  - Én for Now
  - Én for selected time
* På de 2 tidsakser skal man kunne se en linie, der angiver nuværende tidspunkt - gerne med løbende opdatering

Todo:
* Man skal kunne importere sms data
* Man skal kunne vise tidsserier (som i DMI.Data.Studio)
* Man skal kunne vise Gant-diagrammer for forskellige tidsperioder (som i DMI.Data.Studio)
* Man skal kunne vise perioder, hvor der er foretaget målinger (som i DMI.Data.Studio)
* Du skal have et view alla det i PR for associations, som viser lokationer
* Man skal ikke kunne lave nye stationer i history mode
* Man skal ikke kunne slette noget i history mode
* Man skal ikke kunne opdatere noget i history mode
* Der skal være et visuelt cue om at man har skruet tilbage på database tid
* I Tidsserie viewet skal man med nogle pileknapper kunne gå hen til næste linie
* Der skal være et mode for et tidsserieview, hvos det følger med current time.
* Når man tegner streger, skal den tegne streger for et interval, der er 3 gange bredere end World Window, så man disse streger ind, når man panner
* stregerne skal tegnes med en farve, der angiver, om man kaldte create, update eller delete
* Når man klikker på Now-knappen (ud for historisk tid) efter at have været tilbage i transaktionstid, så skal man nulstille historisk tid OG databasetid 
* Listen må også gerne få en baggrundsfarve, der er falmet*
* Du skal kunne ændre en observing facility ved at sætte dens Closed Date til en dato, der ligger før i tiden.
  Når det sker, skal den også sætte to time for den seneste geospatielle lokation til samme tidspunkt.
  ... Det er kun i "almindeligt" mode at man kan tilføje eller ændre data". I det mode ser man også kun de aktive stationer, og så skal man 
  have mulighed for at "nedlægge" dem, dvs sætte deres DateClosed til en dato. Det skal så udvirke, at den seneste Geospatielle lokation også
  sættes til samme dato. Det skal valideres, at man ikke sætter datoen til noget i fremtiden, og også, at man ikke sætter det til noget, der
  er ældre end From for den seneste geospatielle lokation.
* DateEstablished og DateClosed skal ikke vises i brugergrænsefladen, men følge From og To for de underliggende Geospatial Locations.
  Det gælder både Create-dialogen og detalje viewet
* Hvis man opretter en ny, som i øvrigt er nedlagt (og derfor ikke vises), så skal man cleare selection
* Pr default skal den bare vælge en dato i historisk tid - med Shift kan man så angive, at det skal være mere granuleret

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
