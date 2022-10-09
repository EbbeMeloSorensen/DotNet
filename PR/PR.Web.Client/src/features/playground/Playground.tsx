import _ from 'lodash'
import React, { Fragment } from "react";
import { Image, Button, Grid, Header, Label, Icon } from "semantic-ui-react";

export default function Playground() {
    const columns = _.times(16, (i) => (
        <Grid.Column key={i}>
          <Image src='/assets/placeholder.png' />
        </Grid.Column>
      ))

    const GridExampleGrid = () => <Grid>{columns}</Grid>

    const GridExampleDividedNumber = () => (
        <Grid columns={3} divided>
          <Grid.Row>
            <Grid.Column>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
            <Grid.Column>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
            <Grid.Column>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
          </Grid.Row>
      
          <Grid.Row>
            <Grid.Column>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
            <Grid.Column>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
            <Grid.Column>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
          </Grid.Row>
        </Grid>
      )

      const GridExampleCelled = () => (
        <Grid celled>
          <Grid.Row>
            <Grid.Column width={3}>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
            <Grid.Column width={13}>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
          </Grid.Row>
      
          <Grid.Row>
            <Grid.Column width={3}>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
            <Grid.Column width={10}>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
            <Grid.Column width={3}>
                <Image src='/assets/placeholder.png' />
            </Grid.Column>
          </Grid.Row>
        </Grid>
      )
      
      const ButtonExampleEmphasisShorthand = () => (
        <div>
          <Button content='Primary' primary />
          <Button content='Secondary' secondary />
        </div>
      )

      const ButtonExampleAnimated = () => (
        <div>
          <Button animated>
            <Button.Content visible>Next</Button.Content>
            <Button.Content hidden>
              <Icon name='arrow right' />
            </Button.Content>
          </Button>
          <Button animated='vertical'>
            <Button.Content hidden>Shop</Button.Content>
            <Button.Content visible>
              <Icon name='shop' />
            </Button.Content>
          </Button>
          <Button animated='fade'>
            <Button.Content visible>Sign-up for a Pro account</Button.Content>
            <Button.Content hidden>$12.99 a month</Button.Content>
          </Button>
        </div>
      )
     
      
    return (
        <Fragment>
            <Header as='h1'>
                Playground
            </Header>
            <Grid>
                <Grid.Row height='4'>
                    <Grid.Column width='5'>
                        <Button>
                            Bamse
                        </Button>
                    </Grid.Column>
                    <Grid.Column width='5'>
                        <Button>
                            Kylling
                        </Button>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row height='4'>
                    <Grid.Column width='5'>
                        <Button>
                            Aske
                        </Button>
                    </Grid.Column>
                    <Grid.Column width='5'>
                        <Button>
                            Luna
                        </Button>
                    </Grid.Column>
                </Grid.Row>
            </Grid>
            <GridExampleGrid />
            <GridExampleDividedNumber />
            <GridExampleCelled />
            <ButtonExampleEmphasisShorthand />
            <ButtonExampleAnimated />
        </Fragment>
    )
}
