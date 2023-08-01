import { ActivityProductTypeModel } from "./activity-product-type.model";

export class ProductModel {
    id: number;
    code: string;
    name: string;
    description: string;
    pictureFileName: string;
    qrCode: string;
    productTypeId: number;
    productType: ActivityProductTypeModel;
    customerId: number;
    customerAddressId: number;
    files: Array<File>;

    constructor() {
        this.id = null;
        this.code = null;
        this.name = null;
        this.description = null;
        this.pictureFileName = null;
        this.qrCode = null;
        this.productTypeId = null;
        this.productType = new ActivityProductTypeModel();
        this.customerId = null;
        this.customerAddressId = null;
        this.files = [];
    }
}
