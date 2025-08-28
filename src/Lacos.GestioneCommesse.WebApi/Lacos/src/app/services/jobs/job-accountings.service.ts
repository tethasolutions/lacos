import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { JobAccountingModel } from './job-accounting.model';

@Injectable()
export class JobAccountingsService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/jobaccountings`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    readJobAccountings(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/read/?${params}`)
            .pipe(
                map(e =>
                    {
                        const jobAccountings: Array<JobAccountingModel> = [];
                        e.data.forEach(e => {
                            const jobAccounting: JobAccountingModel = Object.assign(new JobAccountingModel(e.id, e.jobId, e.accountingTypeId, e.amount, e.note, e.isPaid, e.targetOperators), e);
                            jobAccountings.push(jobAccounting);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(jobAccountings) : jobAccountings,
                            total: e.total
                        };
                    }
                )
            );
    }

    createJobAccounting(request: JobAccountingModel) {
        return this._http.post<number>(`${this._baseUrl}`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateJobAccounting(request: JobAccountingModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteJobAccounting(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }

    getJobAccountingDetail(id: number) {
        return this._http.get<JobAccountingModel>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => {
                    const jobAccounting = Object.assign(new JobAccountingModel(e.id, e.jobId, e.accountingTypeId, e.amount, e.note, e.isPaid, e.targetOperators), e);
                    return jobAccounting;
                })
            );
    }

}
