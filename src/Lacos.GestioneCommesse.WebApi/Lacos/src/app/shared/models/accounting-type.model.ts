export class AccountingTypeModel {
    id: number;
    name: string;
    description: string;
    generateAlert: boolean;

    constructor() {
        this.id = null;
        this.name = null;
        this.description = null;
        this.generateAlert = false;
    }
}
