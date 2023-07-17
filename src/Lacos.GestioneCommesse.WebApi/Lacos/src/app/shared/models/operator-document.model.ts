export class OperatorDocumentModel {
    id: number;
    operatorId: number;
    description: string;
    fileName: string;
    files: Array<File>;

    constructor() {
        this.id = null;
        this.operatorId = null;
        this.description = null;
        this.fileName = null;
        this.files = [];
    }
}
