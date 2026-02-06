export class ProductTypeModel {
    id: number;
    code: string;
    name: string;
    description: string;
    isReiDoor: boolean;
    isSparePart: boolean;
    colorHex: string;
    isWarehouseManaged: boolean;

    constructor() {
        this.id = null;
        this.code = null;
        this.name = null;
        this.description = null;
        this.isReiDoor = false;
        this.isSparePart = false;
        this.colorHex = null;
        this.isWarehouseManaged = false;
    }
}
