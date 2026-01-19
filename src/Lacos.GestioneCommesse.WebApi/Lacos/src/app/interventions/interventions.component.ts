import { Component, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { InterventionsService } from '../services/interventions/interventions.service';
import { InterventionModalComponent } from './intervention-modal.component';
import { InterventionsCalendarComponent } from './interventions-calendar.component';
import { filter, switchMap, tap } from 'rxjs';
import { Intervention, InterventionStatus } from '../services/interventions/models';
import { MessageBoxService } from '../services/common/message-box.service';

@Component({
    selector: 'app-interventions',
    templateUrl: 'interventions.component.html'
})
export class InterventionsComponent extends BaseComponent {

    @ViewChild('interventionsCalendar', { static: true })
    interventionsCalendar: InterventionsCalendarComponent;

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
        new Date(new Date(new Date().setMinutes(0)).setSeconds(0)).addHours(1), InterventionStatus.Scheduled, false,
        null, null, null, null, null, [], [], [], 0, 0, 0, 0);

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

        this.interventionsCalendar.refresh();
    }

}
