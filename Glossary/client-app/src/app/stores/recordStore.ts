import { makeAutoObservable, reaction, runInAction } from "mobx";
import agent from "../api/agent";
import { Record, RecordFormValues } from "../models/record";
import { Pagination, PagingParams } from "../models/pagination";

export default class RecordStore {
    recordRegistry = new Map<string, Record>();
    selectedRecord: Record | undefined = undefined;
    editMode = false;
    loading = false;
    loadingInitial = false;
    pagination: Pagination | null = null;
    pagingParams = new PagingParams();
    predicate = new Map();
    sorting = 'name';

    constructor() {
        makeAutoObservable(this);

        reaction(
            () => this.predicate.keys(),
            () => {
                this.pagingParams = new PagingParams();
                this.recordRegistry.clear();
                this.loadRecords();
            }
        )
    }

    setPagingParams = (pagingParams: PagingParams) => {
        this.pagingParams = pagingParams;
    }

    setSorting = (sorting: string) => {
        this.sorting = sorting;
    }

    resetPredicate = () => {
        this.predicate.forEach((value, key) => {
            this.predicate.delete(key);
        })
    }

    // Det, du gør i denne funktion, influerer på, hvilke Query Params der sendes med i http requestet
    // til back enden. Check det f.eks. ved at aktivere dev tools i browseren og navigere hen til
    // Network tabben
    setPredicate = (
        nameFilter: string,
        categoryFilter: string) => {
        this.resetPredicate();

        if (nameFilter.length > 0)
        {
            this.predicate.set('term', nameFilter);
        }

        if (categoryFilter.length > 0)
        {
            this.predicate.set('category', categoryFilter);
        }

        this.predicate.set('sorting', this.sorting);
    }

    get axiosParams() {
        const params = new URLSearchParams();
        params.append('pageNumber', this.pagingParams.pageNumber.toString());
        params.append('pageSize', this.pagingParams.pageSize.toString());
        this.predicate.forEach((value, key) => params.append(key, value))
        return params;
    }

    get sortedRecords() {
        if (this.sorting === "name") {
            return Array.from(this.recordRegistry.values()).sort((a, b) => {
                return a.term.localeCompare(b.term, 'en');
            });
        }

        return Array.from(this.recordRegistry.values()).sort((a, b) => {
            return a.created > b.created ? -1 : 1
        });
    }

    loadRecords = async () => {
        this.loadingInitial = true;
        try {
            const result = await agent.Records.list(this.axiosParams);
            console.log(result);
            result.data.forEach(record => {
                this.setRecord(record);
            })
            this.setPagination(result.pagination);
            this.setLoadingInitial(false);
        } catch(error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    }

    setPagination = (pagination: Pagination) => {
        this.pagination = pagination;
    }

    loadRecord = async (id: string) => {
        let record = this.getRecord(id);
        if (record) {
            this.selectedRecord = record;
            return record; 
        } else {
            this.loadingInitial = true;
            try {
                record = await agent.Records.details(id);
                this.setRecord(record);
                runInAction(() => {
                    this.selectedRecord = record;
                })
                this.setLoadingInitial(false);
                return record; 
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    private setRecord = (record: Record) => {
        this.recordRegistry.set(record.id, record);
    }

    private getRecord = (id: string) => {
        return this.recordRegistry.get(id);
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    createRecord = async (record: RecordFormValues) => {
        try {
            await agent.Records.create(record);
            const newRecord = new Record(record);
            this.setRecord(newRecord);
            runInAction(() => {
                this.selectedRecord = newRecord;
            })
        } catch(error) {
            console.log(error);
        }
    }

    updateRecord = async (record: RecordFormValues) => {
        try {
            await agent.Records.update(record);
            runInAction(() => {
                if (record.id) {
                    let updatedRecord = {...this.getRecord(record.id), ...record}
                    this.recordRegistry.set(record.id, updatedRecord as Record);
                    this.selectedRecord = updatedRecord as Record;
                }
            })
        } catch(error) {
            console.log(error);
        }
    }

    deleteRecord = async (id: string) => {
        this.loading = true;
        try {
            await agent.Records.delete(id);
            runInAction(() => {
                this.recordRegistry.delete(id);
                this.loading = false;
            })
        } catch(error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

    clearSelectedRecord = () => {
        this.selectedRecord = undefined;
    }
}