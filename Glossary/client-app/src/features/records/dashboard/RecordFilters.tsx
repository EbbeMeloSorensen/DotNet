import { observer } from "mobx-react-lite";
import React, { useState } from "react";
import { Button, Checkbox, Form, Header, Label, Radio } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";

export default observer(function RecordsFilters() {
    const {recordStore: {setPredicate, sorting, setSorting}} = useStore();

    // Nogle states, vi gerne vil sende til recordStore, når man klikker på Search-knappen
    const [nameFilter, setNameFilter] = useState('');
    const [categoryFilter, setCategoryFilter] = useState('');
    const [sortingLocal, setSortingLocal] = useState(sorting);

    function handleClick() {
        setSorting(sortingLocal);
        setPredicate(nameFilter, categoryFilter);
    }

    return (
        <>
            <Header icon='filter' attached color='teal' content='Filters' />

            <Header>Name</Header>
            <Label>Name contains</Label>
                <input value={nameFilter} onChange={e => setNameFilter(e.target.value)}
            />

            <Header>Category</Header>
            <Label>Category contains</Label>
                <input value={categoryFilter} onChange={e => setCategoryFilter(e.target.value)}
            />

            <Header>Sorting</Header>
            <Form>
                <Form.Field>
                    <Radio
                        label='Alphabetical'
                        name='radioGroup'
                        value='this'
                        checked={sortingLocal === 'name'}
                        onChange={() => setSortingLocal('name')}
                    />
                </Form.Field>
                <Form.Field>
                    <Radio
                        label='Entry Time'
                        name='radioGroup'
                        value='that'
                        checked={sortingLocal === 'created'}
                        onChange={() => setSortingLocal('created')}
                    />
                </Form.Field>
            </Form>

            <Button
                floated="right"
                content='Search'
                onClick={() => handleClick()}
            />
        </>
    )
})