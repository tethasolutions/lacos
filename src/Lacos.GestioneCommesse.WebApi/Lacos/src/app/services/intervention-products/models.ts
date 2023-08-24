export class InterventionProduct {

    constructor(
        readonly activityId: number,
        public productId: number
    ) {
    }

}

export interface IInterventionProductReadModel {

    readonly id: number;
    readonly type: string;
    readonly code: string;
    readonly name: string;
    readonly pictureFileName: string;
    readonly qrCode: string;
    readonly activityId: number;
    readonly interventionId: number;
    readonly interventionStart: Date | string;
    readonly interventionEnd: Date | string;
    readonly interventionOperators: string[];
    readonly canBeRemoved: boolean;

}
