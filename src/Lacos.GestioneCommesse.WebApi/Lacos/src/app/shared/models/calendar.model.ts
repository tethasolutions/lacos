import { SchedulerEvent } from '@progress/kendo-angular-scheduler';
import { CalendarResourcesSettingsModel } from './calendar-resources-settings.model';

export class CalendarModel {
    activities: Array<SchedulerEvent>;
    resourcesSettings: Array<CalendarResourcesSettingsModel>;

    constructor() {
        this.activities = [];
        this.resourcesSettings = [];
    }
}
