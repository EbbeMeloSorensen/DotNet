import { observer } from 'mobx-react-lite';
import React, { SyntheticEvent } from 'react'
import { Link, useHistory } from 'react-router-dom';
import { Button, Header, Segment } from 'semantic-ui-react'
import { Record } from "../../../app/models/record";
import { useStore } from '../../../app/stores/store';

interface Props {
    record: Record
}

export default observer (function RecordDetailedHeader({record}: Props) {
    const history = useHistory();
    const {recordStore} = useStore();
    const {deleteRecord, loading} = recordStore;

    function handleRecordDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
        if (window.confirm("Do you want to delete this record?") == true) {
            deleteRecord(id).then(() => history.push(`/records`));
        }
    }

    return (
        <Segment.Group>
            <Segment clearing attached='bottom'>
                <Header>{record.term}</Header>
                <Button.Group floated='right'>
                    <Button
                        loading={loading}
                        onClick={(e) => handleRecordDelete(e, record.id)} 
                        color='red'
                        type='button'>
                        Delete
                    </Button>
                    <Button as={Link} 
                        to={`/manage/${record.id}`}
                        color='orange'
                        floated='right'
                        type='button'>
                        Edit
                    </Button>
                </Button.Group>
            </Segment>
        </Segment.Group>
    )
})
