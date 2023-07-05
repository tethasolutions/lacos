import { AddressModel } from './address.model';
import { CustomerModel } from './customer.model';
import { ProductTypeModel } from './product-type.model';
import { JobSourceModel } from './job-source.model';
import { JobStatusEnum } from '../enums/job-status.enum';

export class JobDetailModel {
    id: number;
    description: string;
    operatorId: number;
    createdOn: Date;
    expirationDate: Date;
    customerId: number;
    customer: CustomerModel;
    customerAddressId: number;
    customerAddress: AddressModel;
    sourceId: number;
    source: JobSourceModel;
    productTypeId: number;
    productType: ProductTypeModel;
    status: JobStatusEnum;

    constructor() {
        this.id = null;
        this.description = null;
        this.operatorId = null;
        this.createdOn = new Date();
        this.expirationDate = new Date((new Date()).getTime() + (1000 * 60 * 60 * 24 * 2));
        this.customerId = null;
        this.customer = new CustomerModel();
        this.customerAddressId = null;
        this.customerAddress = null;
        this.sourceId = null;
        this.source = new JobSourceModel();
        this.productTypeId = null;
        this.productType = new ProductTypeModel();
        this.status = JobStatusEnum.Pending;
    }
}
