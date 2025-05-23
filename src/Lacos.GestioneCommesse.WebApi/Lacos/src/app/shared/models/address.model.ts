import { v4 as uuidv4 } from 'uuid';

export class AddressModel {
    id: number;
    customerId: number;
    supplierId: number;
    description: string;
    city: string;
    streetAddress: string;
    province: string;
    zipCode: string;
    telephone: string;
    email: string;
    isMainAddress: boolean;
    notes: string;
    jobReference: string;
    contactName: string;
    contactReference: string;
    tempId: string;

    get fullAddress(): string {
        let result = '';
        if (this.description !== '') { result += `${this.description} - `; }
        if (this.streetAddress !== null) { result += `${this.streetAddress}, `; }
        if (this.city !== null) { result += `${this.city} `; }
        if (this.province !== null) { result += ` (${this.province}) `; }
        if (this.zipCode !== null) { result += `, ${this.zipCode}`; }
        return result;
    }

    constructor() {
        this.id = null;
        this.customerId = null;
        this.supplierId = null;
        this.description = '';
        this.city = null;
        this.streetAddress = null;
        this.province = null;
        this.zipCode = null;
        this.isMainAddress = false;
        this.notes = null;
        this.jobReference = null;
        this.contactName = null;
        this.contactReference = null;
        this.tempId = uuidv4();
    }
}
