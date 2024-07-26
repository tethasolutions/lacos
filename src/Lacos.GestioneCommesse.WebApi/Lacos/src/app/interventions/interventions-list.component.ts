import { Component, EventEmitter, Output, output, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { InterventionsService } from '../services/interventions/interventions.service';
import { InterventionModalComponent } from './intervention-modal.component';
import { filter, switchMap, tap } from 'rxjs';
import { Intervention, InterventionStatus } from '../services/interventions/models';
import { MessageBoxService } from '../services/common/message-box.service';
import { InterventionsGridComponent } from './interventions-grid.component';

@Component({
    selector: 'app-interventions-list',
    templateUrl: 'interventions-list.component.html'
})
export class InterventionsListComponent extends BaseComponent {

    @ViewChild('interventionsGrid', { static: true })
    interventionsGrid: InterventionsGridComponent;

    @ViewChild('interventionModal', { static: true })
    interventionModal: InterventionModalComponent;

    constructor(
        private readonly _service: InterventionsService,
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    create() {
        const now = new Date().addHours(1);
        now.setMinutes(0);
        const intervention = new Intervention(0, new Date(new Date(new Date().setMinutes(0)).setSeconds(0)),
            new Date(new Date(new Date().setMinutes(0)).setSeconds(0)).addHours(1), InterventionStatus.Scheduled,
            null, null, null, null, [], [], []);

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

}
