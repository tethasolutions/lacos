import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { AccountingTypeModel } from '../shared/models/accounting-type.model';

@Component({
  selector: 'app-accountingType-modal',
  templateUrl: './accountingType-modal.component.html'
})

export class AccountingTypeModalComponent extends ModalFormComponent<AccountingTypeModel> {
  
  readonly role = Role;

  @Input() accountingType = new AccountingTypeModel();

  constructor(
      messageBox: MessageBoxService
  ) {
      super(messageBox);
      this.options = new AccountingTypeModel();
  }

  protected _canClose(): boolean {
      markAsDirty(this.form);

      if (this.form.invalid) {
          this._messageBox.error('Compilare correttamente tutti i campi');
      }

      return this.form.valid ?? false ;
  }
  
  public loadData() {
  }

}
