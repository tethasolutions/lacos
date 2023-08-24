import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../base.component';
import { JobStatus } from 'src/app/services/jobs/models';
import { activityStatusNames } from 'src/app/services/activities/models';

@Pipe({
    name: 'jobStatus'
})
export class JobStatusPipe extends BaseComponent implements PipeTransform {

    transform(value: JobStatus) {
        return activityStatusNames[value];
    }

}
