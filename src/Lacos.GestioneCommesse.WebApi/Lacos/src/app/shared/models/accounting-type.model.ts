export class AccountingTypeModel {
    id: number;
    name: string;
    description: string;
    generateAlert: boolean;
    isNegative: boolean;
    order: number;

    constructor() {
        this.id = null;
        this.name = null;
        this.description = null;
        this.generateAlert = false;
        this.isNegative = false;
        this.order = 0;
    }
}
