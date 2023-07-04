import { ContactModel } from './contact.model';
import { JobModel } from './job.model';

export class ContactAddressModel {
    description: string;
    city: string;
    streetAddress: string;
    province: string;
    zipCode: string;
    telephone: string;
    email: string;
    isMainAddress: boolean;
    contactId: number;
    contact: ContactModel;
}
