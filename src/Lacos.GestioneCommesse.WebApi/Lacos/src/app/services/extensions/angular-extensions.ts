import { NgForm } from '@angular/forms';
import { markAsDirty } from '../common/functions';

declare module '@angular/forms' {

    interface NgForm {
        markAsDirty(): void;
    }

}

export function angularExtensions() {

    NgForm.prototype.markAsDirty = function () {
        markAsDirty(this);
    }

}
