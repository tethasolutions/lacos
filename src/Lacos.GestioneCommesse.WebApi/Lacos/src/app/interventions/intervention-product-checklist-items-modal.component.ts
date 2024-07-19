import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { InterventionsService } from '../services/interventions/interventions.service';
import { InterventionProductCheckList } from '../services/interventions/models';
import { tap } from 'rxjs';
import { ApiUrls } from '../services/common/api-urls';

@Component({
  selector: 'app-intervention-product-checklist-items-modal',
  templateUrl: './intervention-product-checklist-items-modal.component.html'
})
export class InterventionProductChecklistItemsModalComponent extends ModalFormComponent<number> {

  readonly role = Role;
  readonly imagesUrl = `${ApiUrls.baseUrl}/attachments`;

  interventionProductCheckList: InterventionProductCheckList;

  constructor(
    private readonly _service: InterventionsService,
    messageBox: MessageBoxService
  ) {
    super(messageBox);
  }

  override open(interventionProductId: number) {
    const result = super.open(interventionProductId);

    this._service.readProductCheckListByProductId(interventionProductId)
      .pipe(
        tap(e => this.interventionProductCheckList = e)
      )
      .subscribe();

    return result;
  }

  downloadAttachment(attachmentFileName: string) {
    window.open(`${this.imagesUrl}/${attachmentFileName}`, "_blank");
  }

  protected _canClose() {
    markAsDirty(this.form);

    if (this.form.invalid) {
      this._messageBox.error('Compilare correttamente tutti i campi');
    }

    return this.form.valid;
  }
}
