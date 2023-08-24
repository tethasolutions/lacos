import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../base.component';

@Pipe({
    name: 'strings'
})
export class StringsPipe extends BaseComponent implements PipeTransform {

    transform(value: string[], separator: string = ', ') {
        return value.join(separator);
    }

}
