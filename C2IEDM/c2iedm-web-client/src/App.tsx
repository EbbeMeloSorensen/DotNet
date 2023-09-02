import React, { useState, useEffect } from "react";
import "./App.css";
import { Header, List, ListItem } from "semantic-ui-react";

function App() {
  // This is a useState React hook, that allows us to store state (in variables) inside a component
  // This useState hook consists of the variable people that we initialize as an empty array,
  // and the function setVertices that we can use for setting the state.
  const [vertices, setVertices] = useState<any[]>([]);

  const vertexData = [
    {
      id: 1,
      name: "Vertex A",
      x: 1,
      y: 1,
    },
    {
      id: 2,
      name: "Vertex B",
      x: 2,
      y: 2,
    },
    {
      id: 3,
      name: "Vertex C",
      x: 3,
      y: 3,
    },
  ];

  // This is a useEffect React hook
  // The empty array at the end indicates dependencies. By including an empty set of dependencies like this
  // we avoid the useEffect hook to be called infinitely
  // Neil Cummings gør sådan her, hvor useEffecten ikke har noget navn
  // useEffect(() => {
  //   setVertices(vertexData)
  // }, [])

  // Andre anbefaler at navngive ens userEffect som her.
  // Det opfører sig ligesådan, men er lettere at dokumentere og vedligeholde
  useEffect(function populatePeopleListAfterMounting() {
    setVertices(vertexData);
  }, []);

  return (
    <div>
      <h2>Vertices</h2>
      <ul>
        {/* Her bruger vi javascript til at populere en unordered list (generelt indlejrer man javascript i jsx ved
            at omkranse det med curly brackets). Vi looper over items i people-listen, som jo er state for denne component.
            Bemærk, at items skal have et id, når de tilføjes til en list. Bemærk også, at vi angiver any som type
            for person. Det er for at undgå compiler fejl */}
        {vertices.map((vertex: any) => (
          <li key={vertex.id}>{vertex.name}</li>
        ))}
      </ul>

      {/* Her gør vi det samme som ovenfor, blot hvor vi bruger React-elementer i stedet for html elementer */}
      <Header as="h2" icon="users" content="Vertices" />
      <List>
        {vertices.map((vertex: any) => (
          <ListItem key={vertex.id}>{vertex.name}</ListItem>
        ))}
      </List>
    </div>
  );
}

export default App;
