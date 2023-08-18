import { ProductTypeModel } from './product-type.model';
import { JobSourceModel } from './job-source.model';
import { JobStatusEnum } from '../enums/job-status.enum';
import { AddressModel } from './address.model';
import { CustomerModel } from './customer.model';

export class JobModel {

    id: number;
    createdOn: Date;

    number: number;
    year: number;
    jobDate: Date;
    description: string;
    status: JobStatusEnum;
    // statusChangedOn: Date;

    // customerId: number;
    customer: CustomerModel;

    /* customerAddressId: number;

    sourceId: number;
    source: JobSourceModel;

    productTypeId: number;
    productType: ProductTypeModel;

    notes: NoteModel[];
    quotations: QuotationModel[];
    orders: OrderModel[];
    activities: ActivityModel[]; */

    customerAddress: AddressModel;

    get fullDescription(): string {
        return `${this.id} - ${this.customer.name} - ${this.customerAddress.fullAddress}`;
    }

    get code(): string {
        return `${this.number}/${this.year}`;
    }

    constructor() {
        this.id = null;
        this.createdOn = null;
        this.number = null;
        this.year = null;
        this.jobDate = null;
        this.description = null;
        this.status = null;
        this.customer = new CustomerModel();
        this.customerAddress = new AddressModel();
    }
}