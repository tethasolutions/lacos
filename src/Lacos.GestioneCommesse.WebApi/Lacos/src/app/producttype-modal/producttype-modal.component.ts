import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { ProductTypeModel } from '../shared/models/product-type.model';

@Component({
  selector: 'app-productType-modal',
  templateUrl: './producttype-modal.component.html',
  styleUrls: ['./producttype-modal.component.scss']
})

export class ProductTypeModalComponent extends ModalFormComponent<ProductTypeModel> {
  
  readonly role = Role;

  @Input() productType = new ProductTypeModel();

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
