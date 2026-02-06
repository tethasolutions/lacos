import { Component } from '@angular/core';
import { ModalFormComponent } from '../shared/modal.component';
import { WarehouseMovementModel, WarehouseMovementType } from '../shared/models/warehouse-movement.model';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';

@Component({
    selector: 'app-warehouse-movement-modal',
    templateUrl: './warehouse-movement-modal.component.html',
    styleUrls: ['./warehouse-movement-modal.component.scss']
})
export class WarehouseMovementModalComponent extends ModalFormComponent<WarehouseMovementModel> {

    movementTypes = [
        { value: WarehouseMovementType.Inbound, label: 'Carico' },
        { value: WarehouseMovementType.Outbound, label: 'Scarico' }
    ];

    productName: string = '';

    constructor(
        messageBox: MessageBoxService
    ) {
        super(messageBox);
    }

    openWithProduct(options: WarehouseMovementModel, productName: string) {
        this.productName = productName;
        return this.open(options);
    }

    protected _canClose() {
        markAsDirty(this.form);

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi');
        }

        return this.form.valid;
    }

    getMovementTypeClass(): string {
        if (!this.options) return '';
        return this.options.movementType === WarehouseMovementType.Inbound ? 'movement-inbound' : 'movement-outbound';
    }
}
