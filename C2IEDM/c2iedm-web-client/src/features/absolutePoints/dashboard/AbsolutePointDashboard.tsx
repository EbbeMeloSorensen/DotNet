import { observer } from "mobx-react-lite";
import React, { useEffect, useState } from "react";
import InfiniteScroll from "react-infinite-scroller";
import { Grid, Loader } from "semantic-ui-react";
import { PagingParams } from "../../../app/models/pagination";
import { useStore } from "../../../app/stores/store";
import AbsolutePointListItemPlaceholder from "./AbsolutePointItemPlaceholder";
import AbsolutePointList from "./AbsolutePointList";

export default observer(function AbsolutePointDashboard() {
  const { absolutePointStore } = useStore();
  const {
    loadAbsolutePoints,
    absolutePointRegistry,
    setPagingParams,
    pagination,
  } = absolutePointStore;
  const [loadingNext, setLoadingNext] = useState(false);

  function handleGetNext() {
    setLoadingNext(true);
    setPagingParams(new PagingParams(pagination!.currentPage + 1));
    loadAbsolutePoints().then(() => setLoadingNext(false));
  }

  useEffect(() => {
    if (absolutePointRegistry.size <= 1) loadAbsolutePoints();
  }, [absolutePointRegistry.size, loadAbsolutePoints]);

  return (
    <Grid>
      <Grid.Column width="10">
        {absolutePointStore.loadingInitial && !loadingNext ? (
          <>
            <AbsolutePointListItemPlaceholder />
          </>
        ) : (
          <InfiniteScroll
            pageStart={0}
            loadMore={handleGetNext}
            hasMore={
              !loadingNext &&
              !!pagination &&
              pagination.currentPage < pagination.totalPages
            }
            initialLoad={false}
          >
            <AbsolutePointList />
          </InfiniteScroll>
        )}
      </Grid.Column>
      <Grid.Column width={10}>
        <Loader active={loadingNext} />
      </Grid.Column>
    </Grid>
  );
});
