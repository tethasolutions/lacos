import { ActivityProduct } from '../activity-products/models';
import { Dictionary } from '../common/models';

export enum InterventionStatus {

    Scheduled,
    CompletedSuccesfully,
    CompletedUnsuccesfully

}

export const interventionStatusNames: Dictionary<InterventionStatus, string> = {

    [InterventionStatus.Scheduled]: 'Programmato',
    [InterventionStatus.CompletedSuccesfully]: 'Completato OK',
    [InterventionStatus.CompletedUnsuccesfully]: 'Completato KO'

};

export class Intervention {

    start: Date;
    end: Date;

    constructor(
        readonly id: number,
        start: string | Date,
        end: string | Date,
        public status: InterventionStatus,
        public description: string,
        public vehicleId: number,
        public activityId: number,
        public jobId: number,
        public operators: number[],
        public activityProducts: number[],
        public notes: InterventionNote[]
    ) {
        this.start = new Date(start);
        this.end = new Date(end);
    }

    static build(o: Intervention) {
        const notes = o.notes.map(e => InterventionNote.build(e));
        return new Intervention(o.id, o.start, o.end, o.status, o.description, o.vehicleId,
            o.activityId, o.jobId, o.operators, o.activityProducts, notes);
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
    readonly activityTypeId: string;
    readonly activityType: string;
    readonly activityColor: string;
    readonly activityId: number;
    readonly canBeRemoved: boolean;

}


export interface IInterventionOperatorReadModel {

    readonly id: number;
    readonly name: string;
    readonly colorHex: string;

}

export interface IInterventionProductReadModel {

    readonly interventionProductId: number;
    readonly code: string;
    readonly name: string;
    readonly description: string;
    readonly pictureFileName: string;
    readonly qrCode: string;
    readonly productType: string;

}

export class InterventionProductCheckList {


    constructor(
        readonly interventionProductId: number,
        readonly description: string,
        readonly notes: string,
        readonly items: InterventionProductCheckListItem[]
    ) { }

    static build(o: InterventionProductCheckList) {
        return new InterventionProductCheckList(o.interventionProductId, o.description, o.notes, o.items);
    }
}

export class InterventionProductCheckListItem {

    constructor(
        readonly description: string,
        readonly outcome: string,
        readonly notes: string,
        readonly attachmentFileName: string,
        readonly operatorName: string
    ) { }

    static build(o: InterventionProductCheckListItem) {
        return new InterventionProductCheckListItem(o.description, o.outcome, o.notes, o.operatorName, o.attachmentFileName);
    }
}

export class InterventionNote {

    constructor(
        readonly id: number,
        readonly pictureFileName: string,
        readonly notes: string,
        readonly operatorName: string,
        readonly interventionId: number
    ) { }
    static build(o: InterventionNote) {
        return new InterventionNote(o.id, o.pictureFileName, o.notes, o.operatorName, o.interventionId);
    }
}
