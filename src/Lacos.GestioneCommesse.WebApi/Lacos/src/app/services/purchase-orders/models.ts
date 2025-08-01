import { Dictionary } from '../common/models';
import { Job } from '../jobs/models';
import { MessageReadModel } from '../messages/models';
import { PurchaseOrderAttachmentModel } from './purchase-order-attachment.model';

export enum PurchaseOrderStatus {
    Pending,
    Ordered,
    Completed,
    Partial,
    Canceled
}

export const purchaseOrderStatusNames: Dictionary<PurchaseOrderStatus, string> = {

    [PurchaseOrderStatus.Pending]: 'Da ordinare',
    [PurchaseOrderStatus.Ordered]: 'Ordinato',
    [PurchaseOrderStatus.Completed]: 'Consegnato',
    [PurchaseOrderStatus.Partial]: 'Cons. parzialmente',
    [PurchaseOrderStatus.Canceled]: 'Annullato'

};

export interface IPurchaseOrderReadModel {

    readonly id: number;
    readonly jobCodes: string;
    readonly jobIds: string;
    readonly date: Date | string;
    readonly expectedDate: Date | string;
    readonly status: PurchaseOrderStatus;
    readonly jobReferences: string;
    readonly supplierName: string;
    readonly description: string;
    readonly activityTypeName: string;
    readonly operatorName: string;
    readonly hasAttachments: boolean;
    readonly unreadMessages: number;
}

export class PurchaseOrder {

    date: Date;
    expectedDate: Date;

    get code() {
        return `${this.year}/${this.number}`;
    }

    constructor(
        readonly id: number,
        readonly number: number,
        public year: number,
        date: Date | string,
        expectedDate: Date | string,
        public description: string,
        public status: PurchaseOrderStatus,
        public activityTypeId: number,
        public supplierId: number,
        public supplierName: string,
        public operatorId: number,
        public jobs: number[],
        public items: PurchaseOrderItem[],
        public attachments: PurchaseOrderAttachmentModel[],
        public userAttachments: PurchaseOrderAttachmentModel[],
        public adminAttachments: PurchaseOrderAttachmentModel[],
        public messages: MessageReadModel[]
    ) {
        this.date = date ? new Date(date) : null;
        this.expectedDate = expectedDate ? new Date(expectedDate) : null;
    }

    static build(o: PurchaseOrder, operatorId: number) {
        const items = o.items.map(e => PurchaseOrderItem.build(e));
        const attachments = o.attachments.map(e => PurchaseOrderAttachmentModel.build(e));
        const userAttachments = o.attachments.map(e => PurchaseOrderAttachmentModel.build(e)).filter(e => !e.isAdminDocument);
        const adminAttachments = o.attachments.map(e => PurchaseOrderAttachmentModel.build(e)).filter(e => e.isAdminDocument);
        const messages = o.messages.map(e => MessageReadModel.build(e, operatorId));
        return new PurchaseOrder(o.id, o.number, o.year, o.date, o.expectedDate, o.description, o.status, o.activityTypeId, o.supplierId,
            o.supplierName, o.operatorId, o.jobs, items, attachments, userAttachments, adminAttachments, messages);
    }

}

export class PurchaseOrderItem {

    constructor(
        readonly id: number,
        readonly purchaseOrderId: number,
        public productId: number,
        public productName: string,
        public productImage: string,
        public quantity: number
    ) {
    }

    clone() {
        return new PurchaseOrderItem(this.id, this.purchaseOrderId, this.productId, this.productName, this.productImage, this.quantity);
    }

    static build(o: PurchaseOrderItem) {
        return new PurchaseOrderItem(o.id, o.purchaseOrderId, o.productId, o.productName, o.productImage, o.quantity);
    }
}
