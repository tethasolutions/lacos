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
    files: Array<File>;

    constructor() {
        this.id = null;
        this.pictureFileName = null;
        this.description = null;
        this.productTypeId = null;
        this.productType = new ProductTypeModel();
        this.activityTypeId = null;
        this.activityType = new ActivityTypeModel();
        this.items = [];
        this.files = [];
    }
}
