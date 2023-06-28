import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role, UpdateUserRequest } from '../services/security/models';
import { ModalComponent } from '../shared/modal.component';

@Component({
    selector: 'lacos-user-modal',
    templateUrl: 'user-modal.component.html'
})
export class UserModalComponent extends ModalComponent<UpdateUserRequest> {

    @ViewChild('form')
    form: NgForm;

    readonly role = Role;

    constructor(
        private readonly _messageBox: MessageBoxService
    ) {
        super();
    }

    protected _canClose() {
        markAsDirty(this.form);

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi');
        }

        return this.form.valid;
    }

}
