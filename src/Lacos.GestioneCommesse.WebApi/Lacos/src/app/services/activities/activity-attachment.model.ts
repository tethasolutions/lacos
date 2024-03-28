
export class ActivityAttachmentModel {

    constructor(
        public id: number,
        public displayName: string,
        public fileName: string,
        public activityId: number
    ) {
    }

    static build(o: ActivityAttachmentModel) {
        return new ActivityAttachmentModel(o.id, o.displayName, o.fileName, o.activityId);
    }

    get isImage(): boolean {
        if (this.fileName.endsWith(".jpg") || this.fileName.endsWith(".png") || this.fileName.endsWith(".jpeg") || this.fileName.endsWith(".gif")) {
            return true;
        }
        else {
            return false;
        }
    }
}
