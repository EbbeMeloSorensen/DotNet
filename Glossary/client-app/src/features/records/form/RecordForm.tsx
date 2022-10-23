import { observer } from "mobx-react-lite";
import React, { useEffect, useState } from "react";
import { Link, useHistory, useParams } from "react-router-dom";
import { Button, Header, Segment } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponents";
import { useStore } from "../../../app/stores/store";
import {v4 as uuid} from 'uuid';
import { Formik, Form } from "formik";
import * as Yup from 'yup';
import MyTextInput from "../../../app/common/form/MyTextInput";
import MyTextArea from "../../../app/common/form/MyTextArea";
import { RecordFormValues } from "../../../app/models/record";

export default observer(function RecordForm() {
    const history = useHistory();
    const {recordStore} = useStore();
    const {createRecord, updateRecord, loadRecord, loadingInitial} = recordStore; // "Destructure the props we need from the record store"
    const {id} = useParams<{id: string}>();

    const [record, setRecord] = useState<RecordFormValues>(new RecordFormValues());

    const validationSchema = Yup.object({
        term: Yup.string().required('The term of the record is required')
    })

    useEffect(() => {
        if (id) loadRecord(id).then(record => {
            //console.log("About to populate RecordForm with data");
            let pfv = new RecordFormValues(record);
            setRecord(pfv);
        })
    }, [id, loadRecord]);

    function handleFormSubmit(record: RecordFormValues) {
        if (!record.id) {

            console.log('creating new record..');

            let newRecord = {
                ...record, // ("spread" operator)
                id: uuid(),
                created: new Date(),
                source: record.source === "" ? null : record.source,
                category: record.category === "" ? null : record.category,
                description: record.description === "" ? null : record.description
            };

            console.log(newRecord);

            createRecord(newRecord).then(() => history.push(`/records/${newRecord.id}`))
        } else {
            console.log('updating record..');

            let updatedRecord = {
                ...record, // ("spread" operator)
                source: record.source === "" ? null : record.source,
                category: record.category === "" ? null : record.category,
                description: record.description === "" ? null : record.description,
            };

            console.log(updatedRecord);
            
            updateRecord(updatedRecord).then(() => history.push(`/records/${record.id}`))
        }
    }

    if (loadingInitial) return <LoadingComponent content='Loading record...' />

    return (
        <Segment clearing>
            <Header content='Record Details' sub color='teal' />
            <Formik
            validationSchema={validationSchema}
                enableReinitialize
                initialValues={record}
                onSubmit={values => handleFormSubmit(values)}>
                {({ handleSubmit, isValid, isSubmitting, dirty }) => (
                <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
                    <MyTextInput name='term' placeholder='Term' />
                    <MyTextInput name='Source' placeholder='Source' />
                    <MyTextInput name='category' placeholder='Category' />
                    <MyTextArea rows={3} placeholder='Description' name='description'/>
                    <Button 
                        disabled={isSubmitting || !dirty || !isValid}
                        loading={isSubmitting} floated='right' 
                        positive type='submit' content='Submit' />
                    <Button as={Link} to='/records' floated='right' type='button' content='Cancel' />
                </Form>
                )}
            </Formik>
        </Segment>
    )
})