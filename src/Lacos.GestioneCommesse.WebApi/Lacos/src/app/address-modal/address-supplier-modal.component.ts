import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { SupplierModel } from '../shared/models/supplier.model';
import { AddressSupplierModel } from '../shared/models/address-supplier.model';

@Component({
  selector: 'app-address-supplier-modal',
  templateUrl: './address-supplier-modal.component.html',
  styleUrls: ['./address-supplier-modal.component.scss']
})

export class AddressSupplierModalComponent extends ModalComponent<AddressSupplierModel> {
  
  @ViewChild('form')
  form: NgForm;

  readonly role = Role;

  @Input() supplier = new SupplierModel();

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
