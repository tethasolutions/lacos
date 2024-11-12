export class HelperTypeModel {

    constructor(
        readonly id: number,
        readonly type: string,
    ) {
    }

    static build(o: HelperTypeModel) {
        return new HelperTypeModel(o.id, o.type);
    }

}

export class HelperDocumentModel {
    id: number;
    helperTypeId: number;
    description: string;
    fileName: string;

    constructor() {
        this.id = 0;
        this.helperTypeId = null;
        this.description = null;
        this.fileName = null;
    }

}
