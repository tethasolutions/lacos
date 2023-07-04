export class JobCounterModel {
    active: number;
    expired: number;

    constructor() {
        this.active = 0;
        this.expired = 0;
    }
}
