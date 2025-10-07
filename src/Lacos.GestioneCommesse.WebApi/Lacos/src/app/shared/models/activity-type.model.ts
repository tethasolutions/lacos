export class ActivityTypeModel {
    id: number;
    name: string;
    pictureRequired: boolean;
    isInternal: boolean;
    isExternal: boolean;
    colorHex: string;
    order: number;

    statusLabel0: string;
    statusLabel1: string;
    statusLabel2: string;
    statusLabel3: string;

    influenceJobStatus: boolean;
    viewInPurchaseOrder: boolean;
    hasDependencies: boolean;

    constructor() {
        this.id = null;
        this.name = null;
        this.pictureRequired = false;
        this.isInternal = false;
        this.isExternal = false;
        this.colorHex = null;
        this.statusLabel0 = "In attesa";
        this.statusLabel1 = "In corso";
        this.statusLabel2 = "Pronto";
        this.statusLabel3 = "Completata";
        this.influenceJobStatus = false;
        this.viewInPurchaseOrder = false;
        this.hasDependencies = false;
    }
}

export interface IActivityTypeOperator {

    readonly id: number;
    readonly name: string;

}
