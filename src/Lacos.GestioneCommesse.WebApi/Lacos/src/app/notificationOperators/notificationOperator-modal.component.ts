import { Component, OnInit, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { filter, map, switchMap, tap } from 'rxjs';
import { MessageBoxService } from '../services/common/message-box.service';
import { ApiUrls } from '../services/common/api-urls';
import { NotificationOperator } from '../shared/models/notificationOperator-model';

@Component({
    selector: 'app-notificationOperator-modal',
    templateUrl: 'notificationOperator-modal.component.html'
})
export class NotificationOperatorModalComponent extends ModalFormComponent<NotificationOperator> implements OnInit {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/notificationOperators`;
    
    constructor(
        messageBox: MessageBoxService,
    ) {
        super(messageBox);
    }

    ngOnInit() {
    }

    override open(notificationOperator: NotificationOperator) {
        const result = super.open(notificationOperator);
        
        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

}
