import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { HelperTypeModel } from '../services/helper/models';

@Component({
  selector: 'app-helperType-modal',
  templateUrl: './helpertype-modal.component.html'
})

export class HelperTypeModalComponent extends ModalFormComponent<HelperTypeModel> {
  
  readonly role = Role;

  constructor(
      messageBox: MessageBoxService
  ) {
      super(messageBox);
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
