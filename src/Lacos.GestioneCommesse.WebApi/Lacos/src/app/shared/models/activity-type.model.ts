export class ActivityTypeModel {
    id: number;
    name: string;
    pictureRequired: boolean;
    isInternal: boolean;
    colorHex: string;

    constructor() {
        this.id = null;
        this.name = null;
        this.pictureRequired = false;
        this.isInternal = false;
        this.colorHex = null;
    }
}

export interface IActivityTypeOperator {

    readonly id: number;
    readonly name: string;

}
