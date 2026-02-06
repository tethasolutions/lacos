import { Component, HostListener, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { tap } from 'rxjs';
import { InterventionsService } from '../services/interventions/interventions.service';
import { IInterventionProductReadModel, IInterventionReadModel, InterventionStatus } from '../services/interventions/models';
import { InterventionProductChecklistItemsModalComponent } from './intervention-product-checklist-items-modal.component';

@Component({
    selector: 'app-interventions-singleproduct-grid',
    templateUrl: 'interventions-singleproduct-grid.component.html'
})
export class InterventionsSingleProductGridComponent extends BaseComponent implements OnInit, OnChanges {

    @Input() activityId: number;
    @Input() product: string;

    @ViewChild('interventionProductModal', { static: true }) interventionProductModalComponent: InterventionProductChecklistItemsModalComponent;

    selectedInterventionProduct: IInterventionProductReadModel;
    screenWidth: number;

    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 50,
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


    ngOnChanges(changes: SimpleChanges) {
        if ((changes['activityId'] && this.activityId) && (changes['product'] && this.product)) {
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
            this._service.readInterventionsSingleProduct(this.gridState, this.activityId, this.product)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }

    readonly rowCallback = (context: RowClassArgs) => {
        const intervention = context.dataItem as IInterventionReadModel;

        switch (true) {
            case intervention.status === InterventionStatus.Scheduled:
                return { 'intervention-scheduled': true };
            case intervention.status === InterventionStatus.CompletedSuccesfully:
                return { 'intervention-completed': true };
            case intervention.status === InterventionStatus.CompletedUnsuccesfully:
                return { 'intervention-failed': true };
            default:
                return {};
        }
    };
}
