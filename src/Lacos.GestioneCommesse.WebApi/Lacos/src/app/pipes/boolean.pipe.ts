import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../shared/base.component';

@Pipe({
    name: 'bool'
})
export class BooleanPipe extends BaseComponent implements PipeTransform {

    transform(value: boolean) {
        switch (value) {
            case true:
                return 'Si';
            case false:
                return 'No';
            default:
                return value;
        }
    }

}
