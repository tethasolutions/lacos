import { v4 as uuidv4 } from 'uuid';

export class CheckListItemModel {
    id: number;
    description: string;
    checkListId: number;
    tempId: string;

    constructor() {
        this.id = null;
        this.description = null;
        this.checkListId = null;
        this.tempId = uuidv4();
    }
}
