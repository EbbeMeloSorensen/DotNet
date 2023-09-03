import { observer } from "mobx-react-lite";
import React, { useState } from "react";
import {
  Button,
  Checkbox,
  Form,
  Header,
  Label,
  Radio,
} from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";

export default observer(function PeopleFilters() {
  const {
    personStore: { setPredicate, sorting, setSorting },
  } = useStore();

  // Nogle states, vi gerne vil sende til personStore, når man klikker på Search-knappen
  const [nameFilter, setNameFilter] = useState("");
  const [categoryFilter, setCategoryFilter] = useState("");
  const [dead, setDead] = useState(false);
  const [notDead, setNotDead] = useState(false);
  const [deadUnspecified, setDeadUnspecified] = useState(false);
  const [sortingLocal, setSortingLocal] = useState(sorting);

  function handleClick() {
    setSorting(sortingLocal);
    setPredicate(nameFilter, categoryFilter, dead, notDead, deadUnspecified);
  }

  return (
    <>
      <Button floated="right" content="Search" onClick={() => handleClick()} />
    </>
  );
});
