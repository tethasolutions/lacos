import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { AddressModel } from '../shared/models/address.model';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { OperatorModel } from '../shared/models/operator.model';

@Component({
  selector: 'app-operator-modal',
  templateUrl: './operator-modal.component.html',
  styleUrls: ['./operator-modal.component.scss']
})

export class OperatorModalComponent extends ModalComponent<OperatorModel> {

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
