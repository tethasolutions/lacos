import { SupplierModel } from "./supplier.model";

export class SupplierAddressModel {
    description: string;
    city: string;
    streetAddress: string;
    province: string;
    zipCode: string;
    telephone: string;
    email: string;
    isMainAddress: boolean;
    notes: string;
    supplierId: number;
    supplier: SupplierModel;
}
