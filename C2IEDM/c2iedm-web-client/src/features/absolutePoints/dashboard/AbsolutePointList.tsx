import { observer } from "mobx-react-lite";
import React from "react";
import { List } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import AbsolutePointListItem from "./AbsolutePointListItem";

export default observer(function AbsolutePointList() {
  const { absolutePointStore } = useStore();
  const { sortedAbsolutePoints } = absolutePointStore;

  return (
    <List divided>
      {sortedAbsolutePoints.map((absolutePoint) => (
        <AbsolutePointListItem
          key={absolutePoint.id}
          absolutePoint={absolutePoint}
        />
      ))}
    </List>
  );
});
