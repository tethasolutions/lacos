import { Component, ViewChild, Input, OnInit } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { MessageBoxService } from '../services/common/message-box.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { ActivitiesService } from '../services/activities/activities.service';
import { ActivityAttachmentModel } from '../services/activities/activity-attachment.model';
import { NgForm } from '@angular/forms';
import { ApiUrls } from '../services/common/api-urls';

@Component({
    selector: 'app-activities-attachments-modal',
    templateUrl: './activities-attachments-modal.component.html'
})

export class ActivitiesAttachmentsModalComponent extends ModalComponent<number> implements OnInit {

    @ViewChild('form', { static: false }) form: NgForm;

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/activities`;
    attachments: ActivityAttachmentModel[];

    constructor(
        private readonly _messageBox: MessageBoxService,
        private readonly _activitiesService: ActivitiesService
    ) {
        super();
    }

    ngOnInit() {

    }

    override open(jobId: number) {
        const result = super.open(jobId);

        this._activitiesService.getActivityAttachments(jobId)
            .pipe(
                tap(e => this.attachments = e)
            )
            .subscribe();

        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    public CreateUrl(fileName: string, displayName: string): string {
        return `${this._baseUrl}/activity-attachment/download-file/${fileName}/${displayName}`;
    }
}
