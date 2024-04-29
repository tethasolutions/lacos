export class ActivityProduct {

    constructor(
        readonly id: number,
        readonly activityId: number,
        public productId: number,
        public description: string,
        public location: string,
        public productTypeId: number
    ) {
    }

    static build(o: ActivityProduct) {
        return new ActivityProduct(o.id, o.activityId, o.productId, o.description, o.location, o.productTypeId);
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
    readonly colorHex: string;

}
