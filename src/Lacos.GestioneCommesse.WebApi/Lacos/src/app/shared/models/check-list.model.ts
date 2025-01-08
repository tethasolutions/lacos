import { ProductTypeModel } from "./product-type.model";
import { ActivityTypeModel } from "./activity-type.model";
import { CheckListItemModel } from "./check-list-item.model";

export class CheckListModel {
    id: number;
    pictureFileName: string;
    description: string;
    productTypeId: number;
    productType: ProductTypeModel;
    activityTypeId: number;
    activityType: ActivityTypeModel;
    items: CheckListItemModel[];

    constructor() {
        this.id = null;
        this.pictureFileName = null;
        this.description = null;
        this.productTypeId = null;
        this.productType = new ProductTypeModel();
        this.activityTypeId = null;
        this.activityType = new ActivityTypeModel();
        this.items = [];
    }
}

export class CopyChecklistModel {
    constructor(
        readonly sourceChecklistId: number,
        readonly activityTypeId: number,
        readonly productTypeId: number
    ) { }
    static build(o: CopyChecklistModel) {
        return new CopyChecklistModel(o.sourceChecklistId, o.activityTypeId, o.productTypeId);
    }
}