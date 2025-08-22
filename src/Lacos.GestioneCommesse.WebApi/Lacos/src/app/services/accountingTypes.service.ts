import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { AccountingTypeModel } from '../shared/models/accounting-type.model';

@Injectable()
export class AccountingTypesService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/accountingtypes`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    readAccountingTypes(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/accountingtypes?${params}`)
            .pipe(
                map(e =>
                    {
                        const accountingTypes: Array<AccountingTypeModel> = [];
                        e.data.forEach(item => {
                            const accountingType: AccountingTypeModel = Object.assign(new AccountingTypeModel(), item);
                            accountingTypes.push(accountingType);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(accountingTypes) : accountingTypes,
                            total: e.total
                        };
                    }
                )
            );
    }

    createAccountingType(request: AccountingTypeModel) {
        return this._http.post<number>(`${this._baseUrl}/accountingtype`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateAccountingType(request: AccountingTypeModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/accountingtype/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteAccountingType(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/accountingtypes/${id}`)
            .pipe(
                map(() => { })
            );
    }

    getAccountingTypeDetail(id: number) {
        return this._http.get<AccountingTypeModel>(`${this._baseUrl}/accountingtype-detail/${id}`)
            .pipe(
                map(e => {
                    const accountingType = Object.assign(new AccountingTypeModel(), e);
                    return accountingType;
                })
            );
    }

    readAccountingTypesList() {
        return this._http.get<Array<AccountingTypeModel>>(`${this._baseUrl}/accountingtypes-list`)
            .pipe(
                map(e =>
                    {
                        const accountingTypes: Array<AccountingTypeModel> = [];
                        e.forEach(item => {
                            const accountingType: AccountingTypeModel = Object.assign(new AccountingTypeModel(), item);
                            accountingTypes.push(accountingType);
                        });
                        return accountingTypes;
                    }
                )
            );
    }

    readAccountingTypesListPO() {
        return this._http.get<Array<AccountingTypeModel>>(`${this._baseUrl}/accountingtypes-list-po`)
            .pipe(
                map(e =>
                    {
                        const accountingTypes: Array<AccountingTypeModel> = [];
                        e.forEach(item => {
                            const accountingType: AccountingTypeModel = Object.assign(new AccountingTypeModel(), item);
                            accountingTypes.push(accountingType);
                        });
                        return accountingTypes;
                    }
                )
            );
    }

    readAccountingTypesListCalendar() {
        return this._http.get<Array<AccountingTypeModel>>(`${this._baseUrl}/accountingtypes-list-calendar`)
            .pipe(
                map(e =>
                    {
                        const accountingTypes: Array<AccountingTypeModel> = [];
                        e.forEach(item => {
                            const accountingType: AccountingTypeModel = Object.assign(new AccountingTypeModel(), item);
                            accountingTypes.push(accountingType);
                        });
                        return accountingTypes;
                    }
                )
            );
    }
}
