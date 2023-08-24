import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../base.component';
import { ActivityStatus, activityStatusNames } from 'src/app/services/activities/models';

@Pipe({
    name: 'activityStatus'
})
export class ActivityStatusPipe extends BaseComponent implements PipeTransform {

    transform(value: ActivityStatus) {
        return activityStatusNames[value];
    }

}
