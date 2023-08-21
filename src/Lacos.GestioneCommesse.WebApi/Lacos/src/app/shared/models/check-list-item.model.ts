import { v4 as uuidv4 } from 'uuid';

export class CheckListItemModel {
    id: number;
    description: string;
    checkListId: number;
    tempId: string;

    constructor() {
        this.id = null;
        this.description = null;
        this.checkListId = 0;
        this.tempId = uuidv4();
    }
}
