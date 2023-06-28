import { Injectable } from '@angular/core';

@Injectable()
export class Clipboard {

    copy(value: string) {
        const textarea = document.createElement('textarea');

        textarea.style.height = '0px';
        textarea.style.left = '-100px';
        textarea.style.opacity = '0';
        textarea.style.position = 'fixed';
        textarea.style.top = '-100px';
        textarea.style.width = '0px';

        document.body.appendChild(textarea);

        textarea.value = value;
        textarea.select();

        document.execCommand('copy');

        textarea.remove();
    }

}
