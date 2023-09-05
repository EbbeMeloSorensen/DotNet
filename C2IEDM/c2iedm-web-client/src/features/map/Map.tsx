import { useState, useEffect, useRef } from "react";
import { observer } from "mobx-react-lite";
import { Feature, Map, View } from "ol";
import { OSM, Vector as VectorSource } from "ol/source";
import { Point } from "ol/geom";
import { Circle, Fill, Stroke, Style } from "ol/style";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import proj4 from "proj4";
import { register } from "ol/proj/proj4";
import { useGeographic, get as getProjection } from "ol/proj";
import { useStore } from "../../app/stores/store";
import "ol/ol.css";
import "./Map.css";

interface Props {
  zoomSize: number;
}

export default observer(function MapComponent({ zoomSize }: Props): JSX.Element {
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

  const { absolutePointStore } = useStore();
  const { sortedAbsolutePoints } = absolutePointStore;

  const mapElement = useRef();
  const mapRef = useRef<Map>();

  mapRef.current = map;
  useGeographic();

  var tileLayer = new TileLayer({
    source: new OSM(),
  });

  const stroke = new Stroke({
    color: "rgba(108, 122, 137)", //grey
  });

  const circleColor = "rgba(237, 81, 81, 1)";
    
  const style1 = new Style({
    image: new Circle({
      radius: 5,
      fill: new Fill({ color: circleColor }),
      stroke: stroke,
    }),
  })

  const markers = sortedAbsolutePoints.map((absPoint) => {
    const feature = new Feature(
      new Point([absPoint.latitudeCoordinate, absPoint.longitudeCoordinate])
    );
    feature.setStyle(style1);
    return feature;
  });

  var markerLayer = new VectorLayer({
    source: new VectorSource({
      features: markers,
    }),
  });

  const options = {
    view: new View({
      center: [11, 55.8],
      zoom: zoomSize,
      projection: EPSG25832,
    }),
    layers: [tileLayer, markerLayer],
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
});

