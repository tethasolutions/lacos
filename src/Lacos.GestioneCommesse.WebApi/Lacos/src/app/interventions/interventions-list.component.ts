import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { InterventionsService } from '../services/interventions/interventions.service';
import { InterventionModalComponent } from './intervention-modal.component';
import { filter, switchMap, tap } from 'rxjs';
import { Intervention, InterventionStatus } from '../services/interventions/models';
import { MessageBoxService } from '../services/common/message-box.service';
import { InterventionsGridComponent } from './interventions-grid.component';
import { ActivatedRoute, Params } from '@angular/router';

@Component({
    selector: 'app-interventions-list',
    templateUrl: 'interventions-list.component.html'
})
export class InterventionsListComponent extends BaseComponent implements OnInit {

    @Input() viewNewInterventionButton: boolean = true;

    @ViewChild('interventionsGrid', { static: true })
    interventionsGrid: InterventionsGridComponent;

    @ViewChild('interventionModal', { static: true })
    interventionModal: InterventionModalComponent;

    private _jobId: number;

    constructor(
        private readonly _service: InterventionsService,
        private readonly _route: ActivatedRoute,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() {
        this._jobId = null;
        this._subscribeRouteParams();
    }

    create() {
        const now = new Date().addHours(1);
        now.setMinutes(0);
        const intervention = new Intervention(0, new Date(new Date(new Date().setMinutes(0)).setSeconds(0)),
            new Date(new Date(new Date().setMinutes(0)).setSeconds(0)).addHours(1), InterventionStatus.Scheduled, false,
            null, null, null, null, this._jobId, [], [], [], 0, 0, 0, 0);

        this._subscriptions.push(
            this.interventionModal.open(intervention)
                .pipe(
                    filter(e => e),
                    switchMap(() => this._service.create(intervention)),
                    tap(() => this._afterCreated())
                )
                .subscribe()
        );
    }

    private _afterCreated() {
        this._messageBox.success(`Intervento programmato.`);

        this.interventionsGrid.refresh();
    }

    private _subscribeRouteParams() {
        this._route.queryParams
            .pipe(
                tap(e => this._setParams(e))
            )
            .subscribe();
    }

    private _setParams(params: Params) {
        this._jobId = isNaN(+params['jobId']) ? null : +params['jobId'];
    }
}
