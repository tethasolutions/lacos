import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Router, NavigationEnd } from '@angular/router';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorModalComponent } from '../operator-modal/operator-modal.component';
import { OperatorsService } from '../services/operators.service';

@Component({
  selector: 'app-operators',
  templateUrl: './operators.component.html',
  styleUrls: ['./operators.component.scss']
})

export class OperatorsComponent extends BaseComponent implements OnInit {

  @ViewChild('operatorModal', { static: true }) operatorModal: OperatorModalComponent;

  dataOperators: GridDataResult;
  stateGridOperators: State = {
      skip: 0,
      take: 10,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };

  operatorSelezionato = new OperatorModel();

  constructor(
      private readonly _operatorsService: OperatorsService,
      private readonly _messageBox: MessageBoxService,
      private readonly _router: Router
  ) {
      super();
  }

  ngOnInit() {
      this._readOperators();
  }

  dataStateChange(state: State) {
      this.stateGridOperators = state;
      this._readOperators();
  }

  protected _readOperators() {
    this._subscriptions.push(
      this._operatorsService.readOperators(this.stateGridOperators)
        .pipe(
            tap(e => {
              this.dataOperators = e;
            })
        )
        .subscribe()
    );
  }

  createOperator() {
    const request = new OperatorModel();

    this._subscriptions.push(
        this.operatorModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._operatorsService.createOperator(request)),
                tap(e => {
                  this._messageBox.success(`Operatore ${request.name} creato`);
                }),
                tap(() => this._readOperators())
            )
            .subscribe()
    );
  }

  editOperator(operator: OperatorModel) {
    this._subscriptions.push(
      this._operatorsService.getOperatorDetail(operator.id)
        .pipe(
            map(e => {
              return Object.assign(new OperatorModel(), e);
            }),
            switchMap(e => this.operatorModal.open(e)),
            filter(e => e),
            map(() => this.operatorModal.options),
            switchMap(e => this._operatorsService.updateOperator(e, operator.id)),
            map(() => this.operatorModal.options),
            tap(e => this._messageBox.success(`Operatore ${operator.name} aggiornato`)),
            tap(() => this._readOperators())
        )
      .subscribe()
    );
  }

  deleteOperator(operator: OperatorModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare l'operatore ${operator.name}?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._operatorsService.deleteOperator(operator.id)
            .pipe(
              tap(e => this._messageBox.success(`Operatore ${operator.name} cancellato con successo`)),
              tap(() => this._readOperators())
            )
          .subscribe()
        );
      }
    });
  }

}
