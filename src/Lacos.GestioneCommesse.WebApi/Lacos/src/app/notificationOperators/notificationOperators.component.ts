import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { CellClickEvent, GridComponent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { NotificationOperatorModalComponent } from './notificationOperator-modal.component';
import { getToday } from '../services/common/functions';
import { OperatorModel } from '../shared/models/operator.model';
import { User } from '../services/security/models';
import { NotificationOperatorsService } from '../services/notificationOperators.service';
import { NotificationOperator, NotificationOperatorReadModel } from '../shared/models/notificationOperator-model';

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
    screenWidth: number;

    constructor(
        private readonly _service: NotificationOperatorsService,
        private readonly _messageBox: MessageBoxService,
    ) {
        super();
    }

    ngOnInit() {
        this.updateScreenSize();
        this._read();
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
        const notificationOperator = new NotificationOperator();

        this._subscriptions.push(
            this.notificationOperatorModal.open(notificationOperator)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(notificationOperator)),
                    tap(e => this._read())
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
                    tap(e => this._read())
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
                        tap(e => this._read())
                    )
                    .subscribe()
            );
        }
    }
    
    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }

    askRemove(notificationOperator: NotificationOperatorReadModel) {
        const text = `Sei sicuro di voler rimuovere l'operatore ${notificationOperator.operatorName}?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(notificationOperator.id)),
                    tap(() => this._read())
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

}
