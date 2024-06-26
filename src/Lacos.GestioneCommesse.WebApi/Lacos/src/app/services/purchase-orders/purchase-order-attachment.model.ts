
export class PurchaseOrderAttachmentModel {

    constructor(
        public id: number,
        public displayName: string,
        public fileName: string,
        public purchaseOrderId: number,
        public isAdminDocument: boolean
    ) {
    }

    static build(o: PurchaseOrderAttachmentModel) {
        return new PurchaseOrderAttachmentModel(o.id, o.displayName, o.fileName, o.purchaseOrderId, o.isAdminDocument);
    }
    
    get isImage(): boolean {
        if (this.fileName.endsWith(".jpg") || this.fileName.endsWith(".png") || this.fileName.endsWith(".jpeg") || this.fileName.endsWith(".gif")) {
            return true;
        }
        else {
            return false;
        }
    }
}
