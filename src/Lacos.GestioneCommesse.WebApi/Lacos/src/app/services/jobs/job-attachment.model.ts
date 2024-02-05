
export class JobAttachmentModel {

    constructor(
        public id: number,
        public displayName: string,
        public fileName: string,
        public jobId: number
    ) {
    }

    static build(o: JobAttachmentModel) {
        return new JobAttachmentModel(o.id, o.displayName, o.fileName, o.jobId);
    }
}
