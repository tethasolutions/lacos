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
import { CheckListModel } from '../shared/models/check-list.model';
import { CheckListService } from '../services/check-list.service';
import { ChecklistItemModalComponent } from '../checklist-item-modal/checklist-item-modal.component';
import { ActivityProductTypeModel } from '../shared/models/activity-product-type.model';
import { ActivityTypeModel } from '../shared/models/activity-type.model';

@Component({
  selector: 'app-checklist-modal',
  templateUrl: './checklist-modal.component.html',
  styleUrls: ['./checklist-modal.component.scss']
})

export class ChecklistModalComponent extends ModalComponent<CheckListModel> {

  @ViewChild('form') form: NgForm;
  @ViewChild('checklistItemModal', { static: true }) checklistItemModal: ChecklistItemModalComponent;

  productTypes: Array<ActivityProductTypeModel> = [];
  activityTypes: Array<ActivityTypeModel> = [];

  readonly role = Role;

  constructor(
      private readonly _messageBox: MessageBoxService,
      private readonly _checkListService: CheckListService
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

  protected _readProductTypes() {
    this._subscriptions.push(
      this._checkListService.readProductTypes()
        .pipe(
            tap(e => {
              this.productTypes = e;
            })
        )
        .subscribe()
    );
  }

  protected _readActivityTypes() {
    this._subscriptions.push(
      this._checkListService.readActivityTypes()
        .pipe(
            tap(e => {
              this.activityTypes = e;
            })
        )
        .subscribe()
    );
  }

  createNewItemCheckList() {

  }

  editCheckListItem(dataItem: any) {

  }

  deleteOperatorItem(dataItem: any) {

  }

  public loadData() {
    this._readProductTypes();
    this._readActivityTypes();
  }

}
