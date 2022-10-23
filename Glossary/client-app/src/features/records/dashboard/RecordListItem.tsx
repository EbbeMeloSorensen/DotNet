import React, { SyntheticEvent, useState } from 'react';
import { Link } from "react-router-dom";
import { Button, Item, List, Segment } from "semantic-ui-react";
import { Record } from "../../../app/models/record";
import { useStore } from '../../../app/stores/store';

interface Props {
    record: Record
}

export default function RecordListItem({record}: Props) {
    const {recordStore} = useStore();
    const {deleteRecord, loading} = recordStore;
    const [target, setTarget] = useState('');

    function handleRecordDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
        setTarget(e.currentTarget.name);
        deleteRecord(id);
    }

    return (
        <List.Item>
            <List.Content>
                <List.Header as={Link} to={`/records/${record.id}`}>
                    {record.firstName}
                </List.Header>
            </List.Content>
        </List.Item>
    )
}