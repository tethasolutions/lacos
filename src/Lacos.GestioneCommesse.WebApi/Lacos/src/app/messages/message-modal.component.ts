import { Component, ViewChild, OnInit } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { MessageModalOptions, MessageModel } from '../services/messages/models';
import { WindowState } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'app-message-modal',
  templateUrl: './message-modal.component.html'
})

export class MessageModalComponent extends ModalComponent<MessageModalOptions> implements OnInit {

  @ViewChild('form') form: NgForm;

  public windowState: WindowState = "default";

  constructor(
    private readonly _messageBox: MessageBoxService
  ) {
    super();
  }

  ngOnInit() {
    
  }
  
  override open(options: MessageModalOptions) {
      const result = super.open(options);
      return result;
  }

  protected _canClose() {
    markAsDirty(this.form);

    if (this.form.invalid) {
      this._messageBox.error('Compilare correttamente tutti i campi');
    }

    return this.form.valid;
  }

}
