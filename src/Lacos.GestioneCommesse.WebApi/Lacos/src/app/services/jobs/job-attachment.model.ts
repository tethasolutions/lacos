
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
    
    get isImage(): boolean {
        if (this.fileName.endsWith(".jpg") || this.fileName.endsWith(".png") || this.fileName.endsWith(".jpeg") || this.fileName.endsWith(".gif")) {
            return true;
        }
        else {
            return false;
        }
    }
}
