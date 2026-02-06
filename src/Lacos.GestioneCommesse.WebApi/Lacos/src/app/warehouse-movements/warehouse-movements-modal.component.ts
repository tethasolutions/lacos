import { Component, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { Subject } from 'rxjs';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { WarehouseService } from '../services/warehouse.service';
import { WarehouseMovementModel, WarehouseMovementType } from '../shared/models/warehouse-movement.model';
import { MessageBoxService } from '../services/common/message-box.service';
import { filter, switchMap, tap } from 'rxjs/operators';
import { WarehouseMovementModalComponent } from '../warehouse-movements/warehouse-movement-modal.component';

@Component({
    selector: 'app-warehouse-movements-modal',
    templateUrl: './warehouse-movements-modal.component.html',
    styleUrls: ['./warehouse-movements-modal.component.scss']
})
export class WarehouseMovementsModalComponent extends BaseComponent {

    @ViewChild('movementModal', { static: true }) movementModal: WarehouseMovementModalComponent;

    opened = false;
    productId: number;
    productName: string;

    movements: GridDataResult;

    stateGrid: State = {
        skip: 0,
        take: 20,
        filter: {
            filters: [],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'movementDate', dir: 'desc' }]
    };

    private _closeSubject: Subject<boolean>;
    private _overflow: string;

    constructor(
        private readonly _warehouseService: WarehouseService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    open(productId: number, productName: string) {
        if (!this._closeSubject) {
            this._closeSubject = new Subject<boolean>();
        }

        this._overflow = document.body.style.overflow;
        document.body.style.overflow = 'hidden';

        this.productId = productId;
        this.productName = productName;
        this.opened = true;

        this._readMovements();

        return this._closeSubject.asObservable();
    }

    dismiss() {
        this._closeSubject.next(false);
        this._closeSubject.complete();
        this._closeSubject = null;
        this.opened = false;
        document.body.style.overflow = this._overflow;
    }

    dataStateChange(state: State) {
        this.stateGrid = state;
        this._readMovements();
    }

    getMovementTypeLabel(type: WarehouseMovementType): string {
        switch (type) {
            case WarehouseMovementType.Inbound:
                return 'Carico';
            case WarehouseMovementType.Outbound:
                return 'Scarico';
            default:
                return '';
        }
    }

    createMovement() {
        const request = new WarehouseMovementModel();
        request.productId = this.productId;
        request.movementDate = new Date();
        request.movementType = WarehouseMovementType.Inbound;
        request.quantity = 1;

        this._subscriptions.push(
            this.movementModal.openWithProduct(request, this.productName)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._warehouseService.createMovement(request)),
                    tap(() => this._messageBox.success(`Movimento creato con successo`)),
                    tap(() => this._readMovements())
                )
                .subscribe()
        );
    }

    deleteMovement(movement: WarehouseMovementModel) {
        this._messageBox.confirm(`Sei sicuro di voler cancellare questo movimento?`, 'Conferma l\'azione').subscribe(result => {
            if (result == true) {
                this._subscriptions.push(
                    this._warehouseService.deleteMovement(movement.id)
                        .pipe(
                            tap(() => this._messageBox.success(`Movimento cancellato con successo`)),
                            tap(() => this._readMovements())
                        )
                        .subscribe()
                );
            }
        });
    }

    private _readMovements() {
        this._subscriptions.push(
            this._warehouseService.readMovements(this.productId, this.stateGrid)
                .pipe(
                    tap(e => {
                        this.movements = e;
                    })
                )
                .subscribe()
        );
    }
}
