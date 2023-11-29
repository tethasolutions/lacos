import { Activity } from "./models";


export class ActivityAttachmentModel {
    id: number;
    displayName: string;
    fileName: string;
    activityId: number;
    //activity: Activity;

    constructor() {
        this.id = null;
        this.displayName = null;
        this.fileName = null;
        this.activityId = null;
        //this.activity = new Activity();
    }
}
