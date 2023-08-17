export class VehicleModel {
    id: number;
    name: string;
    plate: string;
    notes: string;

    get description(): string {
        return `${this.name} - ${this.plate}`;
    }

    constructor() {
        this.id = 0;
        this.name = null;
        this.plate = null;
        this.notes = null;
    }
}
