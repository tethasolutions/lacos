import { OperatorDocumentModel } from './operator-document.model';
import { VehicleModel } from './vehicle.model';

export class OperatorModel {
    id: number;
    email: string;
    colorHex: string;
    name: string;
    defaultVehicleId: number;
    defaultVehicle: VehicleModel;
    documents: Array<OperatorDocumentModel>;

    constructor() {
        this.id = null;
        this.email = null;
        this.colorHex = null;
        this.name = null;
        this.defaultVehicleId = null;
        this.defaultVehicle = new VehicleModel();
        this.documents = null;
    }
}
