import { Component, ViewChild, OnInit } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { MessageModalOptions, MessageModel } from '../services/messages/models';
import { WindowState } from '@progress/kendo-angular-dialog';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';
import { State } from '@progress/kendo-data-query';
import { tap } from 'rxjs';

@Component({
  selector: 'app-message-modal',
  templateUrl: './message-modal.component.html'
})

export class MessageModalComponent extends ModalFormComponent<MessageModalOptions> implements OnInit {

  public windowState: WindowState = "default";

  operators: OperatorModel[];
  hasTargetOperators: boolean;

  constructor(
    messageBox: MessageBoxService,
    private readonly _operatorsService: OperatorsService
  ) {
    super(messageBox);
  }

  ngOnInit() {
    this._getOperators();
  }

  override open(options: MessageModalOptions) {
    const result = super.open(options);
    this.hasTargetOperators = options.targetOperators.any();
    return result;
  }

  protected _canClose() {
    markAsDirty(this.form);

    if (this.form.invalid) {
      this._messageBox.error('Compilare correttamente tutti i campi');
    }

    return this.form.valid;
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
