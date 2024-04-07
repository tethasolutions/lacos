import { Component, ViewChild, Input, OnInit } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role, User } from '../services/security/models';
import { CustomerModel } from '../shared/models/customer.model';
import { SupplierModel } from '../shared/models/supplier.model';
import { MessageModel } from '../services/messages/models';
import { WindowState } from '@progress/kendo-angular-dialog';
import { MessagesService } from '../services/messages/messages.service';
import { UserService } from '../services/security/user.service';
import { OperatorsService } from '../services/operators.service';
import { OperatorModel } from '../shared/models/operator.model';
import { tap } from 'rxjs';

@Component({
  selector: 'app-message-modal',
  templateUrl: './message-modal.component.html'
})

export class MessageModalComponent extends ModalComponent<MessageModel> implements OnInit {

  @ViewChild('form') form: NgForm;

  public windowState: WindowState = "default";

  constructor(
    private readonly _messageBox: MessageBoxService
  ) {
    super();
  }

  ngOnInit() {
    
  }

  protected _canClose() {
    markAsDirty(this.form);

    if (this.form.invalid) {
      this._messageBox.error('Compilare correttamente tutti i campi');
    }

    return this.form.valid;
  }

}
