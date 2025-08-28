import { Component, OnInit } from '@angular/core';
import { ModalFormComponent } from '../shared/modal.component';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { JobAccountingModel } from '../services/jobs/job-accounting.model';
import { AccountingTypeModel } from '../shared/models/accounting-type.model';
import { AccountingTypesService } from '../services/accountingTypes.service';
import { tap } from 'rxjs';
import { State } from '@progress/kendo-data-query';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';

@Component({
  selector: 'app-jobaccounting-modal',
  templateUrl: './jobaccounting-modal.component.html'
})

export class JobAccountingModalComponent extends ModalFormComponent<JobAccountingModel> implements OnInit {

  accountingTypes: AccountingTypeModel[];
  operators: OperatorModel[];
  initialFlagIsPaid: boolean = false;

  constructor(
    readonly messageBox: MessageBoxService,
    readonly _accountingTypesService: AccountingTypesService,
    readonly _operatorsService: OperatorsService
  ) {
    super(messageBox);
  }

  ngOnInit() {
    this._getAccountingTypes();
    this._getOperators();
  }

  override open(options: JobAccountingModel) {
    this.initialFlagIsPaid = options?.isPaid;
    return super.open(options);
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

  private _getOperators() {
    const state: State = {
      sort: [
        { field: 'name', dir: 'asc' }
      ]
    };

    this._subscriptions.push(
      this._operatorsService.readOperators(state)
        .pipe(
          tap(e => this.operators = e.data as OperatorModel[])
        )
        .subscribe()
    )
  }

}
