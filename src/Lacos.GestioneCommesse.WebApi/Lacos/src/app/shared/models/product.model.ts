import { getDate } from "@progress/kendo-date-math";
import { ProductDocumentModel } from "./product-document.model";
import { ProductTypeModel } from "./product-type.model";
import { getNow } from "@progress/kendo-angular-dateinputs/util";

export class ProductModel {
    id: number;
    code: string;
    name: string;
    description: string;
    pictureFileName: string;
    qrCode: string;
    productTypeId: number;
    productType: ProductTypeModel;

    customerId: number;
    customerAddressId: number;
    location: string;
    serialNumber: string;
    reiType: string;
    constructorName: string;
    hasPushBar: boolean;
    year: number;
    vocType: string;
    numberOfDoors: number;

    documents: Array<ProductDocumentModel>;

    constructor() {
        this.id = null;
        this.code = null;
        this.name = null;
        this.description = null;
        this.pictureFileName = null;
        this.qrCode = null;
        this.productTypeId = null;
        this.productType = new ProductTypeModel();
        this.customerId = null;
        this.customerAddressId = null;
        this.location = null;
        this.serialNumber = null;
        this.reiType = null;
        this.constructorName = null;
        this.hasPushBar = false;
        this.year = new Date().getFullYear();
        this.vocType = null;
        this.numberOfDoors = null;

        this.documents = [];
    }
}

export interface ProductReadModel {
    readonly id: number;
    readonly code: string;
    readonly name: string;
    readonly description: string;
    readonly pictureFileName: string;
    readonly qrCode: string;
    readonly productType: string;
    readonly customerAddressId: number;
    readonly productTypeId: number;
}
