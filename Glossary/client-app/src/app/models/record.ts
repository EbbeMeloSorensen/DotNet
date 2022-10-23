export interface Record {
    id: string;
    firstName: string;
    surname: string | null;
    nickname: string | null;
    address: string | null;
    zipCode: string | null;
    city: string | null;
    birthday: Date | null;
    category: string | null;
    description: string | null;
    dead: string | boolean | null;
    created: Date;
}

export class Record implements Record {
    constructor(init?: RecordFormValues) {
        Object.assign(this, init); // Populate all of the properties that it can into our record
    }
}

export class RecordFormValues {
    id?: string = undefined;
    firstName: string = '';
    surname: string | null = '';
    nickname: string | null = '';
    address: string | null = '';
    zipCode: string | null = '';
    city: string | null = '';
    birthday: Date | null = null;
    category: string | null = '';
    description: string | null = '';
    dead: string | boolean | null = null;
    created: Date | null = null;

    constructor(record?: RecordFormValues) {
        if (record) {
            this.id = record.id;
            this.firstName = record.firstName === null ? "" : record.firstName;
            this.surname = record.surname === null ? "" : record.surname;
            this.nickname = record.nickname === null ? "" : record.nickname;
            this.address = record.address === null ? "" : record.address;
            this.zipCode = record.zipCode === null ? "" : record.zipCode;
            this.city = record.city === null ? "" : record.city;
            this.birthday = record.birthday;
            this.category = record.category === null ? "" : record.category;
            this.description = record.description === null ? "" : record.description;
            this.dead = record.dead;
            this.created = record.created;
        }
    }
}