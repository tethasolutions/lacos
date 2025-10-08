import { Component } from '@angular/core';
import { ModalFormComponent } from '../shared/modal.component';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { MaintenancePriceListItemModel } from '../shared/models/maintenance-price-list.model';

@Component({
  selector: 'app-maintenance-price-list-item-modal',
  templateUrl: './maintenance-price-list-item-modal.component.html'
})
export class MaintenancePriceListItemModalComponent extends ModalFormComponent<MaintenancePriceListItemModel> {

  readonly role = Role;

  constructor(
      messageBox: MessageBoxService
  ) {
      super(messageBox);
  }

  protected _canClose() {
      markAsDirty(this.form);

      if (this.form.invalid) {
          this._messageBox.error('Compilare correttamente tutti i campi');
      }

      return this.form.valid;
  }
}
