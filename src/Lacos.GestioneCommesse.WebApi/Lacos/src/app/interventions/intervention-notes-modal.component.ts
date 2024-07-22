import { Component, ViewChild, Input, OnInit } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { MessageBoxService } from '../services/common/message-box.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { NgForm } from '@angular/forms';
import { ApiUrls } from '../services/common/api-urls';
import { Intervention } from '../services/interventions/models';
import { InterventionsService } from '../services/interventions/interventions.service';

@Component({
    selector: 'app-intervention-notes-modal',
    templateUrl: './intervention-notes-modal.component.html'
})

export class InterventionNotesModalComponent extends ModalComponent<Intervention> implements OnInit {

    @ViewChild('form', { static: false }) form: NgForm;
    
    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/interventions`;
    intervention: Intervention;

    constructor(
        private readonly _messageBox: MessageBoxService,
        private readonly _service: InterventionsService
    ) {
        super();
    }

    ngOnInit() {

    }

    override open(options: Intervention) {
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

    public CreateUrl(fileName: string): string {
        return `${this._baseUrl}/intervention-note/download-file/${fileName}`;
    }
}
