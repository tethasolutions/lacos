import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { Intervention, InterventionNote, InterventionProductCheckList} from './models';
import { readData } from '../common/functions';
import { GridDataResult } from '@progress/kendo-angular-grid';

@Injectable()
export class InterventionsService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/interventions`;

    constructor(
        private readonly _http: HttpClient
    ) {
    }

    read(state: State) {
        const url = `${this._baseUrl}/read`

        return readData(this._http, state, url);
    }

    get(id: number) {
        return this._http.get<Intervention>(`${this._baseUrl}/${id}`)
            .pipe(
                map(e => Intervention.build(e))
            );
            
    }

    create(intervention: Intervention) {
        return this._http.post<Intervention>(this._baseUrl, intervention)
            .pipe(
                map(e => Intervention.build(e))
            );
    }

    update(intervention: Intervention) {
        return this._http.put<Intervention>(`${this._baseUrl}/${intervention.id}`, intervention)
            .pipe(
                map(e => Intervention.build(e))
            );
    }

    delete(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/${id}`)
            .pipe(
                map(() => { })
            );
    }
    
    readProductsByIntervention(state: State, interventionId: number) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/intervention-products-by-intervention/${interventionId}?${params}`)
            .pipe(
                map(e =>
                    <GridDataResult>{
                        data: hasGroups ? translateDataSourceResultGroups(e.data) : e.data,
                        total: e.total
                    }
                )
            );
    }

    readProductCheckListByProductId(interventionProductId: number) {
        return this._http.get<InterventionProductCheckList>(`${this._baseUrl}/intervention-checklist-by-product/${interventionProductId}`)
            .pipe(
                map(e => InterventionProductCheckList.build(e))
            );
            
    }
    
    readInterventionsSingleProduct(state: State, activityId: number, product: string) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/interventions-singleproduct/${activityId}/${product}?${params}`)
            .pipe(
                map(e =>
                    <GridDataResult>{
                        data: hasGroups ? translateDataSourceResultGroups(e.data) : e.data,
                        total: e.total
                    }
                )
            );
    }

    readInterventionsKo(state: State) {

        return readData(this._http, state, `${this._baseUrl}/interventions-ko`);

    }

    getInterventionAttachments(jobId: number, activityId: number) {
        return this._http.get<Array<InterventionNote>>(`${this._baseUrl}/all-attachments/${jobId}/${activityId}`)
            .pipe(
                map(e => e.map(ee => InterventionNote.build(ee)))
            );
    }

    sendReport(interventionId: number, customerEmail: string) {
        return this._http.post<void>(`${this._baseUrl}/send-report/${interventionId}`, {customerEmail})
            .pipe(
                map(() => { })
            );
    }
}
