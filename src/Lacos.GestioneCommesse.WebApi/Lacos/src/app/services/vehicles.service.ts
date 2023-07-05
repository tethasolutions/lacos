import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from './common/api-urls';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { VehicleModel } from '../shared/models/vehicle.model';

@Injectable()
export class VehiclesService {
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/vehicles`;

    constructor(
        private readonly _http: HttpClient
    ) {}

    readVehicles(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/vehicles?${params}`)
            .pipe(
                map(e =>
                    {
                        const vehicles: Array<VehicleModel> = [];
                        e.data.forEach(item => {
                            const vehicle: VehicleModel = Object.assign(new VehicleModel(), item);
                            vehicles.push(vehicle);
                        });
                        return <GridDataResult>{
                            data: hasGroups ? translateDataSourceResultGroups(vehicles) : vehicles,
                            total: e.total
                        };
                    }
                )
            );
    }

    createVehicle(request: VehicleModel) {
        return this._http.post<number>(`${this._baseUrl}/vehicle`, request)
            .pipe(
                map(e => {
                    return e;
                })
            );
    }

    updateVehicle(request: VehicleModel, id: number) {
        return this._http.put<void>(`${this._baseUrl}/vehicle/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    deleteVehicle(id: number) {
        return this._http.delete<void>(`${this._baseUrl}/vehicle/${id}`)
            .pipe(
                map(() => { })
            );
    }

    getVehicleDetail(id: number) {
        return this._http.get<VehicleModel>(`${this._baseUrl}/vehicle-detail/${id}`)
            .pipe(
                map(e => {
                    const vehicle = Object.assign(new VehicleModel(), e);
                    return vehicle;
                })
            );
    }

    readVehiclesList() {
        return this._http.get<Array<VehicleModel>>(`${this._baseUrl}/vehicles-list`)
            .pipe(
                map(e =>
                    {
                        const vehicles: Array<VehicleModel> = [];
                        e.forEach(item => {
                            const vehicle: VehicleModel = Object.assign(new VehicleModel(), item);
                            vehicles.push(vehicle);
                        });
                        return vehicles;
                    }
                )
            );
    }
}
