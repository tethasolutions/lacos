import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../base.component';
import { ActivityStatus, activityStatusNames } from 'src/app/services/activities/models';

@Pipe({
    name: 'activityStatus'
})
export class ActivityStatusPipe extends BaseComponent implements PipeTransform {

    transform(value: ActivityStatusWithLabels) {
        return this._getStatusLabel(value) || activityStatusNames[value.status];
    }

    private _getStatusLabel(value: ActivityStatusWithLabels) {
        switch (value.status) {
            case ActivityStatus.Pending:
                return value.statusLabel0;
            case ActivityStatus.InProgress:
                return value.statusLabel1;
            case ActivityStatus.Completed:
                return value.statusLabel2;
            default:
                return null;
        }
    }
}

interface ActivityStatusWithLabels {
    readonly status: ActivityStatus;

    readonly statusLabel0?: string;
    readonly statusLabel1?: string;
    readonly statusLabel2?: string;
}