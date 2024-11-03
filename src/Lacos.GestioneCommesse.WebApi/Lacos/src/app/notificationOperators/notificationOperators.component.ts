import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridComponent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { NotificationOperatorModalComponent } from './notificationOperator-modal.component';
import { getToday } from '../services/common/functions';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { StorageService } from '../services/common/storage.service';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';
import { User } from '../services/security/models';
import { UserService } from '../services/security/user.service';
import { CustomerService } from '../services/customer.service';
import { CustomerModel } from '../shared/models/customer.model';
import { CustomerModalComponent } from '../customer-modal/customer-modal.component';
import { ApiUrls } from '../services/common/api-urls';
import { NotificationOperatorsService } from '../services/notificationOperators.service';
import { NotificationOperator } from '../shared/models/notificationOperator-model';

@Component({
    selector: 'app-notificationOperators',
    templateUrl: 'notificationOperators.component.html'
})
export class NotificationOperatorsComponent extends BaseComponent implements OnInit {

    @ViewChild('notificationOperatorModal', { static: true })
    notificationOperatorModal: NotificationOperatorModalComponent;

    @ViewChild('grid', { static: true }) grid: GridComponent;
    
    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        group: []
    };
    expandedDetailKeys = new Array<number>();

    private cellArgs: CellClickEvent;
    user: User;
    currentOperator: OperatorModel;
    private _jobId: number;
    screenWidth: number;

    constructor(
        private readonly _service: NotificationOperatorsService,
        private readonly _messageBox: MessageBoxService,
    ) {
        super();
    }

    ngOnInit() {
        this.updateScreenSize();
      }
    
      @HostListener('window:resize', ['$event'])
      onResize(event: Event): void {
        this.updateScreenSize();
      }
    
      private updateScreenSize(): void {
        this.screenWidth = window.innerWidth -44;
        if (this.screenWidth > 1876) this.screenWidth = 1876;
        if (this.screenWidth < 1400) this.screenWidth = 1400;     
      }


    dataStateChange(state: State) {
        this.gridState = state;
        this._read();
    }

    create() {
        const today = getToday();
        const notificationOperator = new NotificationOperator(0,null);

        this._subscriptions.push(
            this.notificationOperatorModal.open(notificationOperator)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(notificationOperator)),
                    tap(e => this._afterSaved(e))
                )
                .subscribe()
        );
    }

    edit(notificationOperator: NotificationOperator) {
        this._subscriptions.push(
            this._service.get(notificationOperator.id)
                .pipe(
                    switchMap(e => this.notificationOperatorModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.notificationOperatorModal.options)),
                    tap(e => this._afterSaved(e))
                )
                .subscribe()
        );
    }

    onDblClick(): void {
        if (!this.cellArgs.isEdited) {
            this._subscriptions.push(
                this._service.get(this.cellArgs.dataItem.id)
                    .pipe(
                        switchMap(e => this.notificationOperatorModal.open(e)),
                        switchMap(() => this._service.update(this.notificationOperatorModal.options)),
                        tap(e => this._afterSaved(e))
                    )
                    .subscribe()
            );
        }
    }
    
    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }

    askRemove(notificationOperator: NotificationOperator) {
        const text = `Sei sicuro di voler rimuovere l'operatore ${notificationOperator.operatorId}?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(notificationOperator.id)),
                    tap(() => this._afterRemoved(notificationOperator))
                )
                .subscribe()
        );
    }
    
    protected _read() {
        this._subscriptions.push(
            this._service.read(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    private _afterSaved(notificationOperator: NotificationOperator) {
        this._messageBox.success(`Operatore ${notificationOperator.operatorId} salvato.`);

        this._read();
    }

    private _afterRemoved(notificationOperator: NotificationOperator) {
        this._messageBox.success(`Operatore ${notificationOperator.operatorId} rimosso.`);

        this._read();
    }
    
}
