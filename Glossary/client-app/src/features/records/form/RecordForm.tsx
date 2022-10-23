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
import MyDateInput from "../../../app/common/form/MyDateInput";
import { RecordFormValues } from "../../../app/models/record";
import MySelectInput from "../../../app/common/form/MySelectInput";
import { deadOptions } from "../../../app/common/options/deadOptions";

export default observer(function RecordForm() {
    const history = useHistory();
    const {recordStore} = useStore();
    const {createRecord, updateRecord, loadRecord, loadingInitial} = recordStore; // "Destructure the props we need from the record store"
    const {id} = useParams<{id: string}>();

    const [record, setRecord] = useState<RecordFormValues>(new RecordFormValues());

    const validationSchema = Yup.object({
        firstName: Yup.string().required('The term of the record is required')
    })

    useEffect(() => {
        if (id) loadRecord(id).then(record => {
            //console.log("About to populate RecordForm with data");
            let pfv = new RecordFormValues(record);
            // Dead skal sættes som en string eller som null for at den vises i formen
            pfv.dead = pfv.dead === null || pfv.dead.toString() === "" ? null : pfv.dead.toString();
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
                // We handle the "birthday" field like this to ensure it ends up properly in the datebase,
                // where it is stored as UTC time. Notice that the record from the form is given in local time
                birthday: record.birthday === null 
                    ? null 
                    : new Date(Date.UTC(
                        record.birthday.getFullYear(),
                        record.birthday.getMonth(),
                        record.birthday.getDate())),
                // We handle the "dead" field like this to ensure that values are set properly.
                // It is a bit of a hack, but I had to set the values of the deadOptions to strings rather than
                // booleans to get the combobox control to work, and then type of the "dead" property of the
                // RecordFormValues ends up as a string which will cause an error if we pass it to the
                // createRecord method of the agent 
                dead: typeof record.dead === "object" || record.dead.toString() === ""
                    ? null
                    : record.dead.toString() === "true",
                surname: record.surname === "" ? null : record.surname,
                nickname: record.nickname === "" ? null : record.nickname,
                address: record.address === "" ? null : record.address,
                zipCode: record.zipCode === "" ? null : record.zipCode,
                city: record.city === "" ? null : record.city,
                category: record.category === "" ? null : record.category,
                description: record.description === "" ? null : record.description
            };

            console.log(newRecord);

            createRecord(newRecord).then(() => history.push(`/records/${newRecord.id}`))
        } else {
            console.log('updating record..');

            let updatedRecord = {
                ...record, // ("spread" operator)
                birthday: record.birthday === null 
                    ? null 
                    : new Date(Date.UTC(
                        record.birthday.getFullYear(),
                        record.birthday.getMonth(),
                        record.birthday.getDate())),
                dead: typeof record.dead === "object" || record.dead.toString() === ""
                    ? null
                    : record.dead.toString() === "true",
                surname: record.surname === "" ? null : record.surname,
                nickname: record.nickname === "" ? null : record.nickname,
                address: record.address === "" ? null : record.address,
                zipCode: record.zipCode === "" ? null : record.zipCode,
                city: record.city === "" ? null : record.city,
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
                    <MyTextInput name='firstName' placeholder='First Name' />
                    <MyTextInput name='surname' placeholder='Surname' />
                    <MyTextInput name='nickname' placeholder='Nickname' />
                    <MyTextInput name='address' placeholder='Address' />
                    <MyTextInput name='zipCode' placeholder='Zip Code' />
                    <MyTextInput name='city' placeholder='City' />
                    <MyDateInput
                        placeholderText='Birthday'
                        name='birthday'
                        timeCaption='time'
                        dateFormat='MMMM d, yyyy'
                    />
                    <MyTextInput name='category' placeholder='Category' />
                    <MySelectInput options={deadOptions} placeholder='Dead' name='dead'/>
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