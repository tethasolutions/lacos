export class CopyToJobModel {
    constructor(
        readonly sourceId: number,
        readonly jobId: number
    ) { }
    static build(o: CopyToJobModel) {
        return new CopyToJobModel(o.sourceId, o.jobId);
    }
}