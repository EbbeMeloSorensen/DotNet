import { observer } from 'mobx-react-lite';
import React from 'react'
import { Segment, Grid } from 'semantic-ui-react'
import { Record } from "../../../app/models/record";

interface Props {
    record: Record
}

export default observer(function RecordDetailedInfo({record}: Props) {
    return (
        <Segment.Group>
            <Segment>
                <Grid>
                    <Grid.Column width={2}>
                        Source
                    </Grid.Column>
                    <Grid.Column width={14}>
                        <p>{record.source}</p>
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
        </Segment.Group>
    )
})
