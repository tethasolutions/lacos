import { Component, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { markAsDirty } from '../services/common/functions';
import { JobOperatorModel } from '../shared/models/job-operator.model';
import { OperatorDocumentModel } from '../shared/models/operator-document.model';

@Component({
  selector: 'app-operator-document-modal',
  templateUrl: './operator-document-modal.component.html',
  styleUrls: ['./operator-document-modal.component.scss']
})
export class OperatorDocumentModalComponent extends ModalFormComponent<OperatorDocumentModel> {

  documents: Array<JobOperatorModel> = [];

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
