import { observer } from "mobx-react-lite";
import React, { useEffect, useState } from "react";
import InfiniteScroll from "react-infinite-scroller";
import { Grid, Loader } from "semantic-ui-react";
import { PagingParams } from "../../../app/models/pagination";
import { useStore } from "../../../app/stores/store";
import RecordFilters from "./RecordFilters";
import RecordList from "./RecordList";
import RecordListItemPlaceholder from "./RecordListItemPlaceholder";

export default observer(function RecordDashboard() {
    const {recordStore} = useStore();
    const {loadRecords, recordRegistry, setPagingParams, pagination} = recordStore;
    const [loadingNext, setLoadingNext] = useState(false);
  
    function handleGetNext() {
        setLoadingNext(true);
        setPagingParams(new PagingParams(pagination!.currentPage + 1))
        loadRecords().then(() => setLoadingNext(false));
    }

    useEffect(() => {
      if (recordRegistry.size <= 1) loadRecords();
    }, [recordRegistry.size, loadRecords])

    return (
        <Grid>
            <Grid.Column width='10'>
                {recordStore.loadingInitial && !loadingNext ? (
                    <>
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                        <RecordListItemPlaceholder />
                    </>
                ) : (
                        <InfiniteScroll
                            pageStart={0}
                            loadMore={handleGetNext}
                            hasMore={!loadingNext && !!pagination && pagination.currentPage < pagination.totalPages}
                            initialLoad={false}
                        >
                            <RecordList />
                        </InfiniteScroll>
                    )}
            </Grid.Column>
            <Grid.Column width='6'>
                <RecordFilters />
            </Grid.Column>
            <Grid.Column width={10}>
               <Loader active={loadingNext} />
            </Grid.Column>
        </Grid>
    )
})