import { Component, Input } from '@angular/core';
import { BaseComponent } from './base.component';
import { AbstractControl } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';

@Component({
    selector: 'lacos-validation-message,[lacosValidationMessage],.validation-message',
    templateUrl: 'validation-message.component.html'
})
export class ValidationMessageComponent extends BaseComponent {

    private readonly _datePipe = new DatePipe('it');
    private readonly _numberPipe = new DecimalPipe('it');

    @Input()
    control: AbstractControl;

    get error() {
        const errors = this.control.errors;

        switch (true) {
            case errors['required']:
                return 'Campo obbligatorio';
            case errors['minlength']:
                return `Minimo ${errors['minlength'].requiredLength} caratteri`;
            case errors['maxlength']:
                return `Massimo ${errors['maxlength'].requiredLength} caratteri`;
            case errors['email']:
                return 'Email non valida';
            case errors['minError'] && errors['minError'].minValue != null:
                return `Minimo ${this._getText(errors['minError'].minValue)}`;
            case errors['maxError'] && errors['maxError'].maxValue != null:
                return `Massimo ${this._getText(errors['maxError'].maxValue)}`;
            case errors['custom']:
                return errors['custom'];
            default:
                return '';
        }
    }

    constructor() {
        super();
    }

    private _getText(value: any) {
        if (value instanceof Date) {
            return this._datePipe.transform(value, 'dd/MM/yyyy');
        }

        if (typeof (value) === 'number') {
            return this._numberPipe.transform(value, '1.0-2');
        }

        return value.toString();
    }

}
