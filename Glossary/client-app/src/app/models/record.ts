export interface Record {
    id: string;
    term: string;
    source: string | null;
    category: string | null;
    description: string | null;
    created: Date;
}

export class Record implements Record {
    constructor(init?: RecordFormValues) {
        Object.assign(this, init); // Populate all of the properties that it can into our record
    }
}

export class RecordFormValues {
    id?: string = undefined;
    term: string = '';
    source: string | null = '';
    category: string | null = '';
    description: string | null = '';
    created: Date | null = null;

    constructor(record?: RecordFormValues) {
        if (record) {
            this.id = record.id;
            this.term = record.term === null ? "" : record.term;
            this.source = record.source === null ? "" : record.source;
            this.category = record.category === null ? "" : record.category;
            this.description = record.description === null ? "" : record.description;
            this.created = record.created;
        }
    }
}