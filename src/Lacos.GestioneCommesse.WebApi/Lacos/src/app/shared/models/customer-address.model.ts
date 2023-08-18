import { CustomerModel } from "./customer.model";

export class CustomerAddressModel {
    description: string;
    city: string;
    streetAddress: string;
    province: string;
    zipCode: string;
    telephone: string;
    email: string;
    isMainAddress: boolean;
    notes: string;
    customerId: number;
    customer: CustomerModel;
}
