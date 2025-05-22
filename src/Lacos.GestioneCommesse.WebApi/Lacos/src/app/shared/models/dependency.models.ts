export class DependencyModel {
    constructor(
        readonly activityDependenciesId: number[],
        readonly purchaseOrderDependenciesId: number[]
    ) { }

    static build(o: DependencyModel) {
        return new DependencyModel(o.activityDependenciesId, o.purchaseOrderDependenciesId);
    }
}