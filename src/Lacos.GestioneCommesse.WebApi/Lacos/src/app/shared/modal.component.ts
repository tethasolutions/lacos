import { BaseComponent } from './base.component';
import { Subject } from 'rxjs';
import { Directive, ViewChild, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { markAsDirty } from '../services/common/functions';

@Directive()
export abstract class ModalComponent<T> extends BaseComponent {

    options: T;
    opened = false;

    private _closeSubject: Subject<boolean>;
    private _oveflow: string;

    public openedEvent: EventEmitter<any> = new EventEmitter();

    constructor() {
        super();
    }

    open(options: T) {
        if (!this._closeSubject) {
            this._closeSubject = new Subject<boolean>();
        }

        this._oveflow = document.body.style.overflow;
        document.body.style.overflow = 'hidden';

        this.opened = true;
        this.options = options;

        this.openedEvent.emit();

        return this._closeSubject.asObservable();
    }

    close() {
        if (!this._canClose()) {
            return;
        }

        this._closeSubject.next(true);
        this._closeSubject.complete();
        this._closeSubject = null;
        this.opened = false;
        document.body.style.overflow = this._oveflow;
    }

    dismiss() {
        this._closeSubject.next(false);
        this._closeSubject.complete();
        this._closeSubject = null;
        this.opened = false;
        document.body.style.overflow = this._oveflow;
    }

    protected abstract _canClose(): boolean;

}

@Directive()
export abstract class FormModalComponent<T> extends ModalComponent<T> {

    @ViewChild('form')
    form: NgForm;

    constructor(
        protected readonly _messageBox: MessageBoxService
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
