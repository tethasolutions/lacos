import { JobCounterModel } from "./job-counter.model";

export class JobCountersModel {
    acceptance: JobCounterModel;
    actives: JobCounterModel;
    preventives: JobCounterModel;
    supplierOrders: JobCounterModel;
    interventions: JobCounterModel;
    billed: JobCounterModel;

    constructor() {
        this.acceptance = new JobCounterModel();
        this.actives = new JobCounterModel();
        this.preventives = new JobCounterModel();
        this.supplierOrders = new JobCounterModel();
        this.interventions = new JobCounterModel();
        this.billed = new JobCounterModel();
    }
}
