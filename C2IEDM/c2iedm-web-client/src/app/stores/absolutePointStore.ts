import { makeAutoObservable, reaction, runInAction } from "mobx";
import agent from "../api/agent";
import { AbsolutePoint } from "../models/absolutepoint";
import { Pagination, PagingParams } from "../models/pagination";

export default class PersonStore {
  absolutePointRegistry = new Map<string, AbsolutePoint>();
  selectedAbsolutePoint: AbsolutePoint | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;
  pagination: Pagination | null = null;
  pagingParams = new PagingParams();
  predicate = new Map();
  sorting = "lat";

  constructor() {
    makeAutoObservable(this);

    reaction(
      () => this.predicate.keys(),
      () => {
        this.pagingParams = new PagingParams();
        this.absolutePointRegistry.clear();
        this.loadAbsolutePoints();
      }
    );
  }

  setPagingParams = (pagingParams: PagingParams) => {
    this.pagingParams = pagingParams;
  };

  setSorting = (sorting: string) => {
    this.sorting = sorting;
  };

  resetPredicate = () => {
    this.predicate.forEach((value, key) => {
      this.predicate.delete(key);
    });
  };

  get axiosParams() {
    const params = new URLSearchParams();
    params.append("pageNumber", this.pagingParams.pageNumber.toString());
    params.append("pageSize", this.pagingParams.pageSize.toString());
    this.predicate.forEach((value, key) => params.append(key, value));
    return params;
  }

  get sortedAbsolutePoints() {
    return Array.from(this.absolutePointRegistry.values()).sort((a, b) => {
      return a.latitudeCoordinate > b.latitudeCoordinate ? -1 : 1;
    });
  }

  loadAbsolutePoints = async () => {
    console.log('In loading Absolute Points');
    this.loadingInitial = true;
    try {
      const result = await agent.AbsolutePoints.list(this.axiosParams);
      console.log(result);
      result.data.forEach((absolutePoint) => {
        this.setAbsolutePoint(absolutePoint);
      });
      this.setPagination(result.pagination);
      this.setLoadingInitial(false);
    } catch (error) {
      console.log(error);
      this.setLoadingInitial(false);
    }
  };

  private setAbsolutePoint = (absolutePoint: AbsolutePoint) => {
    this.absolutePointRegistry.set(absolutePoint.id, absolutePoint);
  };

  private getAbsolutePoint = (id: string) => {
    return this.absolutePointRegistry.get(id);
  };

  setPagination = (pagination: Pagination) => {
    this.pagination = pagination;
  };

  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  };
}
