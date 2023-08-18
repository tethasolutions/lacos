import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, tap } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { CustomerAddressModel } from '../shared/models/customer-address.model';
import { JobModel } from '../shared/models/job.model';
import { CustomerModel } from '../shared/models/customer.model';
import { JobOperatorModel } from '../shared/models/job-operator.model';
import { AddressModel } from '../shared/models/address.model';
import { JobSourceModel } from '../shared/models/job-source.model';
import { ProductTypeModel } from '../shared/models/product-type.model';
import { JobDetailModel } from '../shared/models/job-detail.model';

@Injectable()
export class JobsService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/jobs`;

    constructor(
        private readonly _http: HttpClient        
    ) {}

    readJobs(state: State, jobType: string) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/jobs-${jobType}?${params}`)
            .pipe(
                map(e =>
                    {
                        const jobs: Array<JobDetailModel> = [];
                        e.data.forEach(item => {
                            const job: JobDetailModel = Object.assign(new JobDetailModel(), item);
                            job.jobDate = new Date(job.jobDate);
                            job.customer = Object.assign(new CustomerModel(), job.customer);

                            const addresses: Array<AddressModel> = [];
                            job.customer.addresses.forEach(addressItem => {
                                const address: AddressModel = Object.assign(new AddressModel(), addressItem);
                                addresses.push(address);
                            });
                            job.customer.addresses = addresses;

                            const mainAddress = addresses.find(x => x.isMainAddress == true);
                            if (mainAddress != undefined) {
                                job.customerAddress = mainAddress;
                            }

                            jobs.push(job);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(jobs) : jobs,
                            total: e.total
                        };
                    }
                )
            );
    }

    getOperators() {
        return this._http.get<Array<JobOperatorModel>>(`${this._baseUrl}/operators`)
            .pipe(
                map(response => {
                    const operators: Array<JobOperatorModel> = [];
                    response.forEach(item => {
                        const operator = Object.assign(new JobOperatorModel(), item);
                        operators.push(operator);
                    });
                    return operators;
                })
            );
    }

    getJobCustomers() {
        return this._http.get<Array<CustomerModel>>(`${this._baseUrl}/job-customers`)
            .pipe(
                map(response => {
                    const customers: Array<CustomerModel> = [];

                    response.forEach(item => {
                        const customer: CustomerModel = Object.assign(new CustomerModel(), item);

                        const addresses: Array<AddressModel> = [];
                        customer.addresses.forEach(addressItem => {
                            const address: AddressModel = Object.assign(new AddressModel(), addressItem);
                            addresses.push(address);
                        });
                        customer.addresses = addresses;

                        let mainAddress = addresses.find(x => x.isMainAddress);
                        if (mainAddress == undefined) { mainAddress = new AddressModel(); }
                        customer.mainAddress = mainAddress;

                        customers.push(customer);
                    });

                    return customers;
                })
            );
    }

    getJobSources() {
        return this._http.get<Array<JobSourceModel>>(`${this._baseUrl}/job-sources`)
            .pipe(
                map(response => {
                    const sources: Array<JobSourceModel> = [];
                    response.forEach(item => {
                        const source = Object.assign(new JobSourceModel(), item);
                        sources.push(source);
                    });
                    return sources;
                })
            );
    }

    getJobDetail(id: number) {
        return this._http.get<JobDetailModel>(`${this._baseUrl}/job-detail/${id}`)
            .pipe(
                map(response => {
                    const job: JobDetailModel = Object.assign(new JobDetailModel(), response);

                    job.jobDate = new Date(job.jobDate);

                    const customer: CustomerModel = Object.assign(new CustomerModel(), job.customer);

                    const addresses: Array<AddressModel> = [];
                    customer.addresses.forEach(item => {
                        const address: AddressModel = Object.assign(new AddressModel(), item);
                        addresses.push(address);
                    });
                    customer.addresses = addresses;

                    let mainAddress = addresses.find(x => x.isMainAddress);
                    if (mainAddress == undefined) { mainAddress = new AddressModel(); }
                    customer.mainAddress = mainAddress;

                    job.customer = customer;

                    const customerAddress: AddressModel = Object.assign(new AddressModel(), job.customerAddress);
                    job.customerAddress = customerAddress;

                    return job;
                })
            );
    }

    createJob(request: JobDetailModel) {
        return this._http.post<CustomerModel>(`${this._baseUrl}/create-job`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateJob(request: JobDetailModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/update-job/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteJob(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/job/${id}`)
            .pipe(
                map(() => { })
            );
    }

    getAllJobs() {
        return this._http.get<Array<JobModel>>(`${this._baseUrl}/all-jobs`)
            .pipe(
                map(result =>
                    {
                        const jobs: Array<JobModel> = [];
                        result.forEach(item => {
                            const job: JobModel = Object.assign(new JobModel(), item);
                            job.jobDate = new Date(job.jobDate);
                            job.customer = Object.assign(new CustomerModel(), job.customer);

                            const addresses: Array<AddressModel> = [];
                            job.customer.addresses.forEach(addressItem => {
                                const address: AddressModel = Object.assign(new AddressModel(), addressItem);
                                addresses.push(address);
                            });
                            job.customer.addresses = addresses;

                            const mainAddress = addresses.find(x => x.isMainAddress == true);
                            if (mainAddress != undefined) {
                                job.customerAddress = mainAddress;
                            }

                            jobs.push(job);
                        });
                        return jobs;
                    }
                )
            );
    }
}
