import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { tap } from 'rxjs';
import { InterventionsService } from '../services/interventions/interventions.service';
import { IInterventionProductReadModel } from '../services/interventions/models';
import { InterventionProductChecklistItemsModalComponent } from './intervention-product-checklist-items-modal.component';

@Component({
    selector: 'app-interventions-products-grid',
    templateUrl: 'interventions-products-grid.component.html'
})
export class InterventionsProductsGridComponent extends BaseComponent implements OnInit, OnChanges {

    @Input() interventionId: number;

    @ViewChild('interventionProductModal', { static: true }) interventionProductModalComponent: InterventionProductChecklistItemsModalComponent;

    selectedInterventionProduct: IInterventionProductReadModel;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        group: [],
        sort: []
    };

    constructor(
        private readonly _service: InterventionsService,
    ) {
        super();
    }

    ngOnInit() {
        this._read();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['interventionId'] && this.interventionId) {
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

    openChecklist(interventionProductId: number) {
        this._subscriptions.push(
            this.interventionProductModalComponent.open(interventionProductId)
                .subscribe()
        );
    }

    protected _read() {
        this._subscriptions.push(
            this._service.readProductsByIntervention(this.gridState, this.interventionId)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

}
