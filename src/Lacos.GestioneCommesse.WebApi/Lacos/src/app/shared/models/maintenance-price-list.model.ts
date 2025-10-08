import { v4 as uuidv4 } from 'uuid';

export class MaintenancePriceListModel {
    id: number;
    description: string;
    hourlyRate: number;
    items: MaintenancePriceListItemModel[];

    constructor() {
        this.id = null;
        this.description = null;
        this.hourlyRate = 0;
        this.items = [];
    }
}

export class MaintenancePriceListItemModel {
    id: number;
    description: string;
    maintenancePriceListId: number;
    serviceCallFee: number;
    travelFee: number;
    limitKm: number;
    extraFee: number;
    tempId: string;

    constructor() {
        this.id = null;
        this.description = null;
        this.maintenancePriceListId = 0;
        this.serviceCallFee = 0;
        this.travelFee = 0;
        this.limitKm = 0;
        this.extraFee = 0;
        this.tempId = uuidv4();
    }
}
