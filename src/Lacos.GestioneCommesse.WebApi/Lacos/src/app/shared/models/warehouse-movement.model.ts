export class WarehouseMovementModel {
    id: number;
    productId: number;
    productCode: string;
    productName: string;
    movementDate: Date;
    movementType: WarehouseMovementType;
    quantity: number;
    notes: string;

    constructor() {
        this.id = 0;
        this.productId = 0;
        this.productCode = '';
        this.productName = '';
        this.movementDate = new Date();
        this.movementType = WarehouseMovementType.Inbound;
        this.quantity = 0;
        this.notes = '';
    }
}

export enum WarehouseMovementType {
    Inbound = 1,
    Outbound = 2
}
