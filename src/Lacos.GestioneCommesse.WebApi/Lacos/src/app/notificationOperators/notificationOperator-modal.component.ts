import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { filter, map, switchMap, tap } from 'rxjs';
import { MessageBoxService } from '../services/common/message-box.service';
import { ApiUrls } from '../services/common/api-urls';
import { NotificationOperator } from '../shared/models/notificationOperator-model';
import { State } from '@progress/kendo-data-query';
import { OperatorsService } from '../services/operators.service';
import { OperatorModel } from '../shared/models/operator.model';

@Component({
    selector: 'app-notificationOperator-modal',
    templateUrl: 'notificationOperator-modal.component.html'
})
export class NotificationOperatorModalComponent extends ModalFormComponent<NotificationOperator> implements OnInit {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/notificationOperators`;
    
    operators: OperatorModel[];
    
    constructor(
        messageBox: MessageBoxService,
        private readonly _operatorsService: OperatorsService,
    ) {
        super(messageBox);
    }

    ngOnInit() {
        this._getOperators();
    }

    override open(options: NotificationOperator) {
        const result = super.open(options);
        
        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    private _getOperators() {
        const state: State = {
            sort: [
                { field: 'name', dir: 'asc' }
            ]
        };

        this._subscriptions.push(
            this._operatorsService.readOperators(state)
                .pipe(
                    tap(e => this.operators = e.data as OperatorModel[])
                )
                .subscribe()
        )
    }
}
