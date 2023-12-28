import { Dictionary } from '../common/models';

export enum PurchaseOrderStatus {
    Pending,
    Ordered,
    Completed,
    Canceled
}

export const purchaseOrderStatusNames: Dictionary<PurchaseOrderStatus, string> = {

    [PurchaseOrderStatus.Pending]: 'In attesa',
    [PurchaseOrderStatus.Ordered]: 'Ordinato',
    [PurchaseOrderStatus.Completed]: 'Evaso',
    [PurchaseOrderStatus.Canceled]: 'Annullato'

};

export interface IPurchaseOrderReadModel {

    readonly id: number;
    readonly code: string;
    readonly date: Date | string;
    readonly description: string;
    readonly status: PurchaseOrderStatus;
    readonly jobId: number;
    readonly jobReference: string;
    readonly jobHasHighPriority: boolean;
    readonly supplierName: string;

}

export class PurchaseOrder {

    date: Date;

    get code() {
        return `${this.year}/${this.number}`;
    }

    constructor(
        readonly id: number,
        readonly number: number,
        public year: number,
        date: Date | string,
        public description: string,
        public status: PurchaseOrderStatus,
        public jobId: number,
        public supplierId: number,
        public supplierName: string
    ) {
        this.date = date ? new Date(date) : null;
    }

    static build(o: PurchaseOrder) {
        return new PurchaseOrder(o.id, o.number, o.year, o.date, o.description, o.status, o.jobId, o.supplierId, o.supplierName);
    }

}

export class PurchaseOrderItem {

    constructor(
        readonly id: number,
        readonly purchaseOrderId: number,
        public productId: number,
        public quantity: number
    ) {
    }

    static build(o: PurchaseOrderItem) {
        return new PurchaseOrderItem(o.id, o.purchaseOrderId, o.productId, o.quantity);
    }
}

export interface IPurchaseOrderItemReadModel {

    readonly id: number;
    readonly purchaseOrderId: number;
    readonly productId: number;
    readonly productName: string;
    readonly quantity: number;

}