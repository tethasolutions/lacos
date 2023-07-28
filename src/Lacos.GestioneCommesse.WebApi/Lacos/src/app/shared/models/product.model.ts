import { ActivityProductTypeModel } from "./activity-product-type.model";

export class ProductModel {
    id: number;
    code: string;
    name: string;
    description: string;
    pictureFileName: string;
    qrCode: string;
    productType: ActivityProductTypeModel;

    constructor() {
        this.id = null;
        this.code = null;
        this.name = null;
        this.description = null;
        this.pictureFileName = null;
        this.qrCode = null;
        this.productType = new ActivityProductTypeModel();
    }
}
