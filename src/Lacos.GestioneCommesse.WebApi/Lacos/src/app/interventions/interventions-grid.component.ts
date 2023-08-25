import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { InterventionsService } from '../services/interventions/interventions.service';
import { filter, switchMap, tap } from 'rxjs';
import { IInterventionReadModel, Intervention, interventionStatusNames } from '../services/interventions/models';
import { MessageBoxService } from '../services/common/message-box.service';
import { InterventionModalComponent } from './intervention-modal.component';

@Component({
    selector: 'app-interventions-grid',
    templateUrl: 'interventions-grid.component.html'
})
export class InterventionsGridComponent extends BaseComponent implements OnInit, OnChanges {

    @Input()
    activityId: number;

    @Input()
    interventionModal: InterventionModalComponent;

    @Output()
    readonly interventionUpdated = new EventEmitter<Intervention>();

    @Output()
    readonly interventionRemoved = new EventEmitter<number>();

    readonly interventionStatusNames = interventionStatusNames;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 10,
        filter: {
            filters: [
                this._buildActivityIdFilter()
            ],
            logic: 'and'
        },
        group: [],
        sort: []
    };

    constructor(
        private readonly _service: InterventionsService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() {
        this._read();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['activityId'] && !changes['activityId'].isFirstChange && this.activityId) {
            this._read();
        }
    }

    refresh() {
        this._read();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._read();
    }

    edit(intervention: IInterventionReadModel) {
        this._subscriptions.push(
            this._service.get(intervention.id)
                .pipe(
                    switchMap(e => this.interventionModal.open(e)),
                    filter(e => e),
                    switchMap(() => this._service.update(this.interventionModal.options)),
                    tap(e => this._afterUpdated(e))
                )
                .subscribe()
        );
    }

    askRemove(intervention: IInterventionReadModel) {
        if (!intervention.canBeRemoved) {
            return;
        }

        const text = `Sei sicuro di voler rimuovere l'intervento selezionato?`;

        this._subscriptions.push(
            this._messageBox.confirm(text, 'Attenzione')
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.delete(intervention.id)),
                    tap(() => this._afterRemoved(intervention.id))
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

    private _afterRemoved(id: number) {
        this._messageBox.success(`Intervento rimosso.`);

        this.interventionRemoved.emit(id);

        this._read();
    }

    private _afterUpdated(intervention: Intervention) {
        this._messageBox.success('Intervento aggiornato.');

        this.interventionUpdated.emit(intervention);

        this._read();
    }

    private _buildActivityIdFilter() {
        const that = this;

        return {
            field: 'activityId',
            get operator() {
                return that.activityId
                    ? 'eq'
                    : 'isnotnull'
            },
            get value() {
                return that.activityId;
            }
        };
    }

}
