import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { VehicleModel } from '../shared/models/vehicle.model';

@Component({
  selector: 'app-vehicle-modal',
  templateUrl: './vehicle-modal.component.html',
  styleUrls: ['./vehicle-modal.component.scss']
})

export class VehicleModalComponent extends ModalFormComponent<VehicleModel> {

  readonly role = Role;
  
  @Input() vehicleModal = new VehicleModel();

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

  public loadData() {
  }
}
