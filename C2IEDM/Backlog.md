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
- Create Observing Facility
- Delete
- Update
- Show historical situation (m�ske en del af filteret?.. eller m�ske en del af en mere fancy kontrol, hvor man ogs� kan se, 
- hvorn�r der er lavet �ndringer..)
