export class JobOperatorModel {
    id: number;
    name: string;
    surname: string;

    get description(): string {
        return `${this.name} ${this.surname}`;
    }

    constructor() {
        this.id = null;
        this.name = null;
        this.surname = null;
    }
}
