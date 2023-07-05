import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Router, NavigationEnd } from '@angular/router';
import { VehicleModel } from '../shared/models/vehicle.model';
import { VehicleModalComponent } from '../vehicle-modal/vehicle-modal.component';
import { VehiclesService } from '../services/vehicles.service';

@Component({
  selector: 'app-vehicles',
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.scss']
})

export class VehiclesComponent extends BaseComponent implements OnInit {

  @ViewChild('vehicleModal', { static: true }) vehicleModal: VehicleModalComponent;

  dataVehicles: GridDataResult;
  stateGridVehicles: State = {
      skip: 0,
      take: 10,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };

  vehicleSelezionato = new VehicleModel();

  constructor(
      private readonly _vehiclesService: VehiclesService,
      private readonly _messageBox: MessageBoxService,
      private readonly _router: Router
  ) {
      super();
  }

  ngOnInit() {
      this._readVehicles();
  }

  dataStateChange(state: State) {
      this.stateGridVehicles = state;
      this._readVehicles();
  }

  protected _readVehicles() {
    this._subscriptions.push(
      this._vehiclesService.readVehicles(this.stateGridVehicles)
        .pipe(
            tap(e => {
              this.dataVehicles = e;
            })
        )
        .subscribe()
    );
  }

  createVehicle() {
    const request = new VehicleModel();

    this._subscriptions.push(
        this.vehicleModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._vehiclesService.createVehicle(request)),
                tap(e => {
                  this._messageBox.success(`Trasporto ${request.name} ${request.plate} creato`);
                }),
                tap(() => this._readVehicles())
            )
            .subscribe()
    );
  }

  editVehicle(vehicle: VehicleModel) {
    this._subscriptions.push(
      this._vehiclesService.getVehicleDetail(vehicle.id)
        .pipe(
            map(e => {
              return Object.assign(new VehicleModel(), e);
            }),
            switchMap(e => this.vehicleModal.open(e)),
            filter(e => e),
            map(() => this.vehicleModal.options),
            switchMap(e => this._vehiclesService.updateVehicle(e, vehicle.id)),
            map(() => this.vehicleModal.options),
            tap(e => this._messageBox.success(`Trasporto ${vehicle.name} ${vehicle.plate} aggiornato`)),
            tap(() => this._readVehicles())
        )
      .subscribe()
    );
  }

  deleteVehicle(vehicle: VehicleModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare il trasporto ${vehicle.name} ${vehicle.plate}?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._vehiclesService.deleteVehicle(vehicle.id)
            .pipe(
              tap(e => this._messageBox.success(`Trasporto ${vehicle.name} ${vehicle.plate} cancellato con successo`)),
              tap(() => this._readVehicles())
            )
          .subscribe()
        );
      }
    });
  }

}
