import { observer } from 'mobx-react-lite';
import React from 'react'
import { Segment, Grid } from 'semantic-ui-react'
import { Record } from "../../../app/models/record";
import { format } from 'date-fns';

interface Props {
    record: Record
}

export default observer(function RecordDetailedInfo({record}: Props) {
    return (
        <Segment.Group>
            <Segment>
                <Grid attached='top'>
                    <Grid.Column width={2}>
                        Nickname
                    </Grid.Column>
                    <Grid.Column width={14}>
                        <p>{record.nickname}</p>
                    </Grid.Column>
                </Grid>
            </Segment>
            <Segment>
                <Grid>
                    <Grid.Column width={2}>
                        Address
                    </Grid.Column>
                    <Grid.Column width={14}>
                        <p>{record.address}</p>
                    </Grid.Column>
                </Grid>
            </Segment>
            <Segment>
                <Grid>
                    <Grid.Column width={2}>
                        Zip Code
                    </Grid.Column>
                    <Grid.Column width={14}>
                        <p>{record.zipCode}</p>
                    </Grid.Column>
                </Grid>
            </Segment>
            <Segment>
                <Grid>
                    <Grid.Column width={2}>
                        City
                    </Grid.Column>
                    <Grid.Column width={14}>
                        <p>{record.city}</p>
                    </Grid.Column>
                </Grid>
            </Segment>
            <Segment>
                <Grid>
                    <Grid.Column width={2}>
                        Birthday
                    </Grid.Column>
                    <Grid.Column width={14}>
                        <span>
                            {record.birthday === null ? '' : format(record.birthday, 'MMMM d, yyyy')}
                        </span>
                    </Grid.Column>
                </Grid>
            </Segment>
            <Segment>
                <Grid>
                    <Grid.Column width={2}>
                        Category
                    </Grid.Column>
                    <Grid.Column width={14}>
                        <p>{record.category}</p>
                    </Grid.Column>
                </Grid>
            </Segment>
            <Segment>
                <Grid>
                    <Grid.Column width={2}>
                        Description
                    </Grid.Column>
                    <Grid.Column width={14}>
                        <p>{record.description}</p>
                    </Grid.Column>
                </Grid>
            </Segment>
            <Segment attached>
                <Grid>
                    <Grid.Column width={2}>
                        Dead
                    </Grid.Column>
                    <Grid.Column width={14}>
                        <span>
                            { record.dead === null 
                                ? null 
                                : record.dead === false
                                    ? "No"
                                    : "Yes" }
                        </span>
                    </Grid.Column>
                </Grid>
            </Segment>
        </Segment.Group>
    )
})
