import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { AddressModel } from '../shared/models/address.model';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { OperatorModel } from '../shared/models/operator.model';
import { VehicleModel } from '../shared/models/vehicle.model';
import { VehiclesService } from '../services/vehicles.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { SimpleLookupModel } from '../shared/models/simple-lookup.model';
import { OperatorDocumentsModalComponent } from '../operator-documents-modal/operator-documents-modal.component';

@Component({
  selector: 'app-operator-modal',
  templateUrl: './operator-modal.component.html',
  styleUrls: ['./operator-modal.component.scss']
})

export class OperatorModalComponent extends ModalComponent<OperatorModel> {

  @ViewChild('form') form: NgForm;
  @ViewChild('operatorDocumentsModal', { static: true }) operatorDocumentsModal: OperatorDocumentsModalComponent;

  vehicles: Array<VehicleModel> = [];
  roles: Array<SimpleLookupModel> = [];

  readonly role = Role;

  constructor(
      private readonly _messageBox: MessageBoxService,
      private readonly _vehiclesService: VehiclesService
  ) {
      super();
  }

  protected _canClose() {
      markAsDirty(this.form);

      if (this.form.invalid) {
          this._messageBox.error('Compilare correttamente tutti i campi');
      }

      return this.form.valid;
  }

  protected _readVehicles() {
    this._subscriptions.push(
      this._vehiclesService.readVehiclesList()
        .pipe(
            tap(e => {
              this.vehicles = e;
            })
        )
        .subscribe()
    );
  }

  setRoles() {
    this.roles = [];
    for(var n in Role) {
        if (typeof Role[n] === 'number') {
          this.roles.push({id: <any>Role[n], name: n});
        }
    }
  }

  viewDocuments() {
    this.operatorDocumentsModal.operatorId = this.options.id;
    this.operatorDocumentsModal.loadData();
    this.operatorDocumentsModal.open(null);
  }

  public loadData() {
    this._readVehicles();
    this.setRoles();
  }

}
