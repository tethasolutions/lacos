import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../base.component';
import { JobStatus, jobStates } from 'src/app/services/jobs/models';

@Pipe({
    name: 'jobStatus'
})
export class JobStatusPipe extends BaseComponent implements PipeTransform {

    transform(value: JobStatus) {
        return jobStates[value];
    }

}
