export class ProductDocumentModel {

    constructor(
        public id: number,
        public productId: number,
        public originalFileName: string,
        public fileName: string,
        public description: string
    ) { }

    static build(o: ProductDocumentModel) {
        return new ProductDocumentModel(o.id, o.productId, o.originalFileName, o.fileName, o.description);
    }
}

export interface ProductDocumentReadModel {
    id: number;
    productId: number;
    originalFileName: string;
    fileName: string;
    description: string;
}
