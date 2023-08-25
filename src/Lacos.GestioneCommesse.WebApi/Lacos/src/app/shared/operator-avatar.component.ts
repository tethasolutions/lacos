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

    @HostBinding('style.color')
    color: string;

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
            this.backgroundColor = this._getBackgroundColor(this.options.colorHex);
            this.color = this._getColor(this.backgroundColor);
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

    private _getBackgroundColor(hex: string) {
        return hex
            ? hex
            : '#ffffff';
    }

    private _getColor(backgroundHex: string) {
        if (!backgroundHex) {
            return 'black';
        }

        const backgroundRgb = {
            r: parseInt(backgroundHex.substring(1, 2), 16),
            g: parseInt(backgroundHex.substring(3, 2), 16),
            b: parseInt(backgroundHex.substring(5, 2), 16)
        };
        const brightness = Math.round((
            backgroundRgb.r * 299 +
            backgroundRgb.g * 587 +
            backgroundRgb.b * 114
        ) / 1000);

        return brightness > 125
            ? 'black'
            : 'white';
    }

}

export interface IOperatorAvatarOptions {

    readonly name: string;
    readonly colorHex: string;

}
