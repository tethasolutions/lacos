export enum InterventionStatus {

    Scheduled,
    CompletedSuccesfully,
    CompletedUnsuccesfully

}

export class Intervention {

    start: Date;
    end: Date;

    constructor(
        readonly id: number,
        start: string | Date,
        end: string | Date,
        readonly status: InterventionStatus,
        public description: string,
        public vehicleId: number,
        public activityId: number,
        public jobId: number,
        public operators: number[],
        public activityProducts: number[]
    ) {
        this.start = new Date(start);
        this.end = new Date(end);
    }

    static build(o: Intervention) {
        return new Intervention(o.id, o.start, o.end, o.status, o.description, o.vehicleId,
            o.activityId, o.jobId, o.operators, o.activityProducts);
    }

}

export interface IInterventionReadModel {

    readonly id: number;
    readonly status: InterventionStatus;
    readonly start: Date | string;
    readonly end: Date | string;
    readonly customer: string;
    readonly customerAddress: string;
    readonly description: string;
    readonly operators: IInterventionOperatorReadModel[];
    readonly activityType: string;

}


export interface IInterventionOperatorReadModel {

    readonly id: number;
    readonly name: string;
    readonly colorHex: string;

}
