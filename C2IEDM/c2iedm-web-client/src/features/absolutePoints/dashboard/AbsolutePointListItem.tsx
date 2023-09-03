import React, { SyntheticEvent, useState } from "react";
import { Link } from "react-router-dom";
import { Button, Item, List, Segment } from "semantic-ui-react";
import { AbsolutePoint } from "../../../app/models/absolutepoint";
import { useStore } from "../../../app/stores/store";

interface Props {
  absolutePoint: AbsolutePoint;
}

export default function AbsolutePointListItem({ absolutePoint }: Props) {
  const { absolutePointStore } = useStore();
  const { loading } = absolutePointStore;
  const [target, setTarget] = useState("");

  return (
    <List.Item>
      <List.Content>
        <List.Header as={Link} to={`/people/${absolutePoint.id}`}>
          {absolutePoint.lat} {absolutePoint.long}
        </List.Header>
      </List.Content>
    </List.Item>
  );
}
