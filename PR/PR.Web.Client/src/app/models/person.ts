export interface Person {
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

export class Person implements Person {
    constructor(init?: PersonFormValues) {
        Object.assign(this, init); // Populate all of the properties that it can into our person
    }
}

export class PersonFormValues {
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

    constructor(person?: PersonFormValues) {
        if (person) {
            this.id = person.id;
            this.firstName = person.firstName === null ? "" : person.firstName;
            this.surname = person.surname === null ? "" : person.surname;
            this.nickname = person.nickname === null ? "" : person.nickname;
            this.address = person.address === null ? "" : person.address;
            this.zipCode = person.zipCode === null ? "" : person.zipCode;
            this.city = person.city === null ? "" : person.city;
            this.birthday = person.birthday;
            this.category = person.category === null ? "" : person.category;
            this.description = person.description === null ? "" : person.description;
            this.dead = person.dead;
            this.created = person.created;
        }
    }
}