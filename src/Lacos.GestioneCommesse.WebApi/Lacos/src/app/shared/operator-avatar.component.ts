import { Component, HostBinding, Input, OnChanges, SimpleChanges } from '@angular/core';
import { BaseComponent } from './base.component';

@Component({
    selector: 'app-operator-avatar',
    template: '{{content}}'
})
export class OperatorAvatarComponent extends BaseComponent implements OnChanges {

    @HostBinding('attr.title')
    title: string;

    @HostBinding('style.background-color')
    backgroundColor: string;

    @Input()
    options: IOperatorAvatarOptions;

    content: string;

    constructor() {
        super();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['options'] && this.options) {
            this.title = this.options.name;
            this.content = this._getContent(this.options.name);
            this.backgroundColor = this.options.colorHex;
        }
    }

    private _getContent(name: string) {
        const words = name?.trim().split(' ');

        if (!words) {
            return '';
        }

        if (words.length === 1) {
            return words[0][0].toUpperCase();
        }

        return words[0][0].toUpperCase() + words[1][0].toUpperCase();
    }

}

export interface IOperatorAvatarOptions {

    readonly name: string;
    readonly colorHex: string;

}
