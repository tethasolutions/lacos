import { Component, Input } from '@angular/core';
import { ModalFormComponent } from '../shared/modal.component';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { JobAccountingModel } from '../services/jobs/job-accounting.model';

@Component({
  selector: 'app-jobaccountings-modal',
  templateUrl: './jobaccountings-modal.component.html'
})

export class JobAccountingsModalComponent extends ModalFormComponent<number> {

  @Input() jobId: number;

  constructor(
    readonly messageBox: MessageBoxService
  ) {
    super(messageBox);
  }

  protected _canClose(): boolean {
    return true;
  }

}
