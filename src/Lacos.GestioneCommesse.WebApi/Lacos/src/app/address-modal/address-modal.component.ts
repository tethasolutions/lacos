import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { AddressModel } from '../shared/models/address.model';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { CustomerModel } from '../shared/models/customer.model';

@Component({
  selector: 'app-address-modal',
  templateUrl: './address-modal.component.html',
  styleUrls: ['./address-modal.component.scss']
})

export class AddressModalComponent extends ModalComponent<AddressModel> {
  
  @ViewChild('form')
  form: NgForm;

  readonly role = Role;

  @Input() customer = new CustomerModel();

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
