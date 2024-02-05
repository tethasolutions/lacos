
export class PurchaseOrderAttachmentModel {

    constructor(
        public id: number,
        public displayName: string,
        public fileName: string,
        public purchaseOrderId: number
    ) {
    }

    static build(o: PurchaseOrderAttachmentModel) {
        return new PurchaseOrderAttachmentModel(o.id, o.displayName, o.fileName, o.purchaseOrderId);
    }
}
