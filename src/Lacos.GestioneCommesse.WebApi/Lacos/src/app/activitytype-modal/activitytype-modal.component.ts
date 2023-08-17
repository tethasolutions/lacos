import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { ActivityTypeModel } from '../shared/models/activity-type.model';

@Component({
  selector: 'app-activityType-modal',
  templateUrl: './activityType-modal.component.html',
  styleUrls: ['./activityType-modal.component.scss']
})

export class ActivityTypeModalComponent extends ModalComponent<ActivityTypeModel> {
  
  @ViewChild('form') form: NgForm;

  readonly role = Role;

  @Input() activityType = new ActivityTypeModel();

  constructor(
      private readonly _messageBox: MessageBoxService
  ) {
      super();
      this.options = new ActivityTypeModel();
      this.options.pictureRequired = false;
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
