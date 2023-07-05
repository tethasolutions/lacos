import { OperatorDocumentModel } from './operator-document.model';

export class OperatorModel {
    id: number;
    email: string;
    colorHex: string;
    name: string;
    defaultVehicleId: number;
    documents: Array<OperatorDocumentModel>;

    constructor() {
        this.id = null;
        this.email = null;
        this.colorHex = null;
        this.name = null;
        this.defaultVehicleId = null;
        this.documents = null;
    }
}
