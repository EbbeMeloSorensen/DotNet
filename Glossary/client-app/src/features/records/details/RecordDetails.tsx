import { observer } from "mobx-react-lite";
import React, { useEffect } from "react";
import { useParams } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponents";
import { useStore } from "../../../app/stores/store";
import RecordDetailedHeader from "./RecordDetailedHeader";
import RecordDetailedInfo from "./RecordDetailedInfo";

export default observer(function RecordDetails() {
    const {recordStore} = useStore();
    const {selectedRecord: record, loadRecord, loadingInitial, clearSelectedRecord} = recordStore;
    const {id} = useParams<{id: string}>();

    useEffect(() => {
        if (id) loadRecord(id);
        return () => clearSelectedRecord();
    }, [id, loadRecord, clearSelectedRecord]);

    if (loadingInitial || !record) return <LoadingComponent />;

    return (
        <>
            <RecordDetailedHeader record={record} />
            <RecordDetailedInfo record={record} />
        </>
    )
})