export class ProductDocumentModel {
    id: number;
    operatorId: number;
    originalFileName: string;
    fileName: string;    
    description: string;

    constructor() {
        this.id = null;
        this.operatorId = 0;
        this.originalFileName = null;
        this.fileName = null;
        this.description = null;
    }
}

export interface ProductDocumentReadModel {
    id: number;
    operatorId: number;
    originalFileName: string;
    fileName: string;    
    description: string;
}
