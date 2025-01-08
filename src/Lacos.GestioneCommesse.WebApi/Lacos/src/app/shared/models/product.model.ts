import { getDate } from "@progress/kendo-date-math";
import { ProductDocumentModel } from "./product-document.model";
import { ProductTypeModel } from "./product-type.model";
import { getNow } from "@progress/kendo-angular-dateinputs/util";

export class ProductModel {
    id: number;
    code: string;
    name: string;
    description: string;
    note: string;
    pictureFileName: string;
    qrCodePrefix: string;
    qrCodeNumber: string;
    productTypeId: number;
    productType: ProductTypeModel;

    customerId: number;
    addressId: number;
    location: string;
    serialNumber: string;
    reiType: string;
    constructorName: string;
    hasPushBar: boolean;
    year: number;
    vocType: string;
    numberOfDoors: number;
    monthlyMaintenance: boolean;
    isDecommissioned: boolean;
    
    documents: Array<ProductDocumentModel>;

    constructor() {
        this.id = null;
        this.code = null;
        this.name = null;
        this.description = null;
        this.note = null;
        this.pictureFileName = null;
        this.qrCodePrefix = null;
        this.qrCodeNumber = null;
        this.productTypeId = null;
        this.productType = new ProductTypeModel();
        this.customerId = null;
        this.addressId = null;
        this.location = null;
        this.serialNumber = null;
        this.reiType = null;
        this.constructorName = null;
        this.hasPushBar = false;
        this.year = new Date().getFullYear();
        this.vocType = null;
        this.numberOfDoors = null;
        this.monthlyMaintenance = false;
        this.isDecommissioned = false;

        this.documents = [];
    }
}

export interface ProductReadModel {
    readonly id: number;
    readonly code: string;
    readonly name: string;
    readonly description: string;
    readonly location: string;
    readonly pictureFileName: string;
    readonly qrCode: string;
    readonly productType: string;
    readonly addressId: number;
    readonly productTypeId: number;
    readonly monthlyMaintenance: boolean;
    readonly isDecommissioned: boolean;
}
