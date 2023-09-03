import { useState, useEffect, useRef } from "react";
import { Map, View } from "ol";
import { OSM } from "ol/source";
import "ol/ol.css";
import { Tile as TileLayer } from "ol/layer";
import proj4 from "proj4";
import { register } from "ol/proj/proj4";
import { useGeographic, get as getProjection } from "ol/proj";
import "./Map.css";

function MapComponent({ zoomSize }): JSX.Element {
  proj4.defs(
    "EPSG:25832",
    "+proj=utm +zone=32 +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs"
  );
  register(proj4);
  const EPSG25832 = getProjection("EPSG:25832");
  const [map, setMap] = useState<Map>(
    new Map({
      view: new View({
        center: [11.7, 56.2],
        zoom: 7.8,
        projection: EPSG25832,
      }),
    })
  );

  const mapElement = useRef();
  const mapRef = useRef<Map>();

  mapRef.current = map;
  useGeographic();

  var tileLayer = new TileLayer({
    source: new OSM(),
  });

  const options = {
    view: new View({
      center: [11, 55.8],
      zoom: zoomSize,
      projection: EPSG25832,
    }),
    layers: [tileLayer],
  };

  useEffect(() => {
    const initialMap = new Map(options);
    initialMap.setTarget(mapElement.current);
    setMap(initialMap);
    return () => initialMap.setTarget(undefined);
  }, []);

  return (
    <>
      <div className="mapContainer" ref={mapElement}></div>
    </>
  );
}

export default MapComponent;
