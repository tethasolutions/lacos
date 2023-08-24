export class ActivityProduct {

    constructor(
        readonly activityId: number,
        public productId: number
    ) {
    }

}

export interface IActivityProductReadModel {

    readonly id: number;
    readonly type: string;
    readonly code: string;
    readonly name: string;
    readonly pictureFileName: string;
    readonly qrCode: string;
    readonly activityId: number;
    readonly canBeRemoved: boolean;

}
