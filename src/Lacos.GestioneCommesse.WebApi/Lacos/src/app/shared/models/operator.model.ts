import { OperatorDocumentModel } from './operator-document.model';
import { VehicleModel } from './vehicle.model';

export class OperatorModel {
    id: number;
    email: string;
    colorHex: string;
    name: string;
    defaultVehicleId: number;
    defaultVehicle: VehicleModel;
    hasUser:boolean;
    username: string;
    password:string;
    documents: Array<OperatorDocumentModel>;
    activityTypes: number[];

    constructor() {
        this.id = null;
        this.email = null;
        this.colorHex = null;
        this.name = null;
        this.defaultVehicleId = null;
        this.username = "";
        this.password = "";
        this.hasUser = false;
        this.defaultVehicle = new VehicleModel();
        this.documents = [];
        this.activityTypes = [];
    }
}
