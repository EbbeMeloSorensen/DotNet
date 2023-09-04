import React, { useEffect } from "react";
import { observer } from "mobx-react-lite";
import { Grid, Header, Segment } from "semantic-ui-react";
import MapComponent from "./Map";

export default observer(function MapView() {
    return (
        <div className="mapViewContainer" style={{ height: '700px' }}>
            <MapComponent zoomSize={7.8} />
        </div>
    )
})
