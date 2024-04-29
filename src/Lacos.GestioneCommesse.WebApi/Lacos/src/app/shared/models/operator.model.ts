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
    userName: string;
    password:string;
    documents: Array<OperatorDocumentModel>;
    activityTypes: number[];
    signatureFileName:string;

    constructor() {
        this.id = null;
        this.email = null;
        this.colorHex = null;
        this.name = null;
        this.defaultVehicleId = null;
        this.userName = "";
        this.password = "";
        this.hasUser = false;
        this.defaultVehicle = new VehicleModel();
        this.documents = [];
        this.activityTypes = [];
        this.signatureFileName = null;
    }
}
