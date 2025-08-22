import { Component, OnInit } from '@angular/core';
import { ModalFormComponent } from '../shared/modal.component';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { JobAccountingModel } from '../services/jobs/job-accounting.model';
import { AccountingTypeModel } from '../shared/models/accounting-type.model';
import { AccountingTypesService } from '../services/accountingTypes.service';
import { tap } from 'rxjs';
import { State } from '@progress/kendo-data-query';

@Component({
  selector: 'app-jobaccounting-modal',
  templateUrl: './jobaccounting-modal.component.html'
})

export class JobAccountingModalComponent extends ModalFormComponent<JobAccountingModel> implements OnInit {

  accountingTypes: AccountingTypeModel[];

  constructor(
    readonly messageBox: MessageBoxService,
    readonly _accountingTypesService: AccountingTypesService
  ) {
    super(messageBox);
  }

  ngOnInit() {
    this._getAccountingTypes();
  }

  protected _canClose(): boolean {
    markAsDirty(this.form);

    if (this.form.invalid) {
      this._messageBox.error('Compilare correttamente tutti i campi');
    }

    return this.form.valid ?? false;
  }

  public _getAccountingTypes() {
    const state: State = {
      sort: [
        { field: 'name', dir: 'asc' }
      ]
    };

    this._subscriptions.push(
      this._accountingTypesService.readAccountingTypes(state)
        .pipe(
          tap(e => this.accountingTypes = e.data as AccountingTypeModel[])
        ).subscribe()
    );

  }

}
