
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
}
