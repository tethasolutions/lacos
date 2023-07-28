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
import { ActivityProductTypeModel } from '../shared/models/activity-product-type.model';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { CheckListItemModel } from '../shared/models/check-list-item.model';

@Component({
  selector: 'app-checklist-item-modal',
  templateUrl: './checklist-item-modal.component.html',
  styleUrls: ['./checklist-item-modal.component.scss']
})
export class ChecklistItemModalComponent extends ModalComponent<CheckListItemModel> {

  @ViewChild('form') form: NgForm;
  readonly role = Role;

  constructor(
      private readonly _messageBox: MessageBoxService
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
}
