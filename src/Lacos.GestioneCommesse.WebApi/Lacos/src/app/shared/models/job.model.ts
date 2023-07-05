import { ContactAddressModel } from './contact-address.model';
import { OrderModel } from './order.model';
import { ContactModel } from './contact.model';
import { NoteModel } from './note.model';
import { ActivityModel } from './activity.model';
import { QuotationModel } from './quotation.model';
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
    expirationDate: Date;
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
        return `${this.id} - ${this.customer.customerDescription} - ${this.customerAddress.fullAddress}`;
    }

    get expired(): boolean {
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const expiration = new Date(this.expirationDate);
        expiration.setHours(0, 0, 0, 0);

        return today > expiration;
    }

    get code(): string {
        return `${this.number}/${this.year}`;
    }

    constructor() {
        this.id = null;
        this.createdOn = null;
        this.number = null;
        this.year = null;
        this.expirationDate = null;
        this.description = null;
        this.status = null;
        this.customer = new CustomerModel();
        this.customerAddress = new AddressModel();
    }
}