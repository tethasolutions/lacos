import { NgForm } from '@angular/forms';
import { markAsDirty } from '../common/functions';

declare global {
    interface String {
        capitalize(): string;
        removeSpaces(): string;
    }
}

export function stringExtensions() {

    String.prototype.capitalize = function () {
        return this
            .split(' ')
            .map(e =>
                e.length > 1
                    ? e[0].toUpperCase() + e.substring(1)
                    : e.length === 1
                        ? e.toUpperCase()
                        : e
            )
            .join(' ');
    }

    String.prototype.removeSpaces = function () {
        return this.replace(/ /gi, '');
    }

}
