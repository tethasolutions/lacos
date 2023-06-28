import { Injectable, TemplateRef } from '@angular/core';
import { NotificationService } from '@progress/kendo-angular-notification';
import { DialogService, DialogCloseResult, DialogSettings } from '@progress/kendo-angular-dialog';
import { map } from 'rxjs/operators';

@Injectable()
export class MessageBoxService {

    constructor(
        private readonly _notification: NotificationService,
        private readonly _dialog: DialogService
    ) {

    }

    info(text: string) {
        this._notify('info', text);
    }

    success(text: string) {
        this._notify('success', text);
    }

    warning(text: string) {
        this._notify('warning', text);
    }

    error(text: string) {
        this._notify('error', text);
    }

    confirm(text: string | TemplateRef<any> | Function, title?: string, yes: string = 'Si', no: string = 'No') {
        const options: DialogSettings = {
            title: title,
            content: text,
            actions: [
                {
                    text: no
                },
                {
                    text: yes
                }
            ],
            width: 450,
            height: 200,
            minWidth: 250
        };
        const dialogResult = this._dialog
            .open(options)
            .result
            .pipe(
                map(e => !(e instanceof DialogCloseResult) && e.text === yes)
            );

        return dialogResult;
    }

    alert(text: string | TemplateRef<any> | Function, title?: string, yes?: string) {
        const options = {
            title: title,
            content: text,
            actions: [
                {
                    text: yes || 'Ok',
                    primary: true
                }
            ],
            width: 450,
            height: 200,
            minWidth: 250
        };
        const dialogResult = this._dialog
            .open(options)
            .result
            .pipe(
                map(() => { })
            );

        return dialogResult;
    }

    private _notify(style: 'success' | 'warning' | 'error' | 'info', text: string) {
        this._notification.show({
            content: text,
            hideAfter: 5000,
            animation: {
                type: 'slide',
                duration: 400
            },
            position: {
                horizontal: 'right',
                vertical: 'bottom'
            },
            type: {
                style: style,
                icon: true
            }
        });
    }
}
