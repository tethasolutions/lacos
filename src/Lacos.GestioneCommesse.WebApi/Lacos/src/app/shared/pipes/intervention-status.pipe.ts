import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../base.component';
import { InterventionStatus, interventionStatusNames } from 'src/app/services/interventions/models';

@Pipe({
    name: 'interventionStatus'
})
export class InterventionStatusPipe extends BaseComponent implements PipeTransform {

    transform(value: InterventionStatus) {
        return interventionStatusNames[value];
    }

}
