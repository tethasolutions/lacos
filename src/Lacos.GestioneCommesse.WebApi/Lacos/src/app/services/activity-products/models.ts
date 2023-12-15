export class ActivityProduct {

    constructor(
        readonly activityId: number,
        public productId: number,
        public description: string,
        public location: string
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
    readonly description: string;
    readonly location: string;
    readonly canBeRemoved: boolean;

}
