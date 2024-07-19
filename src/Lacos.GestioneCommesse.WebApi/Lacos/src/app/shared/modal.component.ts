import { BaseComponent } from './base.component';
import { filter, Subject, tap } from 'rxjs';
import { Directive, EventEmitter, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MessageBoxService } from '../services/common/message-box.service';
import { tableNodes } from '@progress/kendo-angular-editor';

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
export abstract class ModalFormComponent<T> extends ModalComponent<T> {

    @ViewChild('form', { static: false }) form: NgForm;
    
    constructor (protected readonly _messageBox: MessageBoxService){
        super();

    }

    override dismiss() {
        if (this.form.dirty) {
            this._messageBox.confirm("Sei sicuro di voler chiudere la finestra senza salvare?", "Conferma chiusura")
            .pipe(
                filter(e => e),
                tap(() => super.dismiss())
            )
            .subscribe();
        }
        else
            super.dismiss();
    }
}