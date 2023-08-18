import { AddressModel } from './address.model';
import { CustomerModel } from './customer.model';
import { ProductTypeModel } from './product-type.model';
import { JobSourceModel } from './job-source.model';
import { JobStatusEnum } from '../enums/job-status.enum';

export class JobDetailModel {
    id: number;
    description: string;
    operatorId: number;
    jobDate: Date;
    customerId: number;
    customer: CustomerModel;
    customerAddressId: number;
    customerAddress: AddressModel;
    status: JobStatusEnum;

    constructor() {
        this.id = null;
        this.description = null;
        this.operatorId = null;
        this.jobDate = new Date();
        this.customerId = null;
        this.customer = new CustomerModel();
        this.customerAddressId = null;
        this.customerAddress = null;
        this.status = JobStatusEnum.Pending;
    }
}
