import { CalendarResourceModel } from './calendar-resource.model';

export class CalendarResourcesSettingsModel {
    data: Array<any>;
    field: string;
    valueField: string;
    textField: string;
    colorField: string;

    constructor() {
        this.data = [];
        this.field = null;
        this.valueField = null;
        this.textField = null;
        this.colorField = null;
    }
}
