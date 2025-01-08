import { Component, OnInit } from '@angular/core';
import { tap } from 'rxjs';
import { ActivityTypesService } from '../services/activityTypes.service';
import { MessageBoxService } from '../services/common/message-box.service';
import { CopyChecklistModel } from '../shared/models/check-list.model';
import { ProductTypesService } from '../services/productTypes.service';
import { ProductTypeModel } from '../shared/models/product-type.model';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { ModalFormComponent } from '../shared/modal.component';

@Component({
    selector: 'app-copy-checklist-modal',
    templateUrl: 'copy-checklist-modal.component.html'
})
export class CopyChecklistModalComponent extends ModalFormComponent<CopyChecklistModel> implements OnInit {

    productTypes: Array<ProductTypeModel> = [];
    activityTypes: Array<ActivityTypeModel> = [];

    constructor(
        messageBox: MessageBoxService,
        private readonly _productTypesService: ProductTypesService,
        private readonly _activityTypesService: ActivityTypesService
    ) {
        super(messageBox);
    }

    ngOnInit() {
        this._readProductTypes();
        this._readActivityTypes();
    }

    override open(options: CopyChecklistModel) {
        const result = super.open(options);

        return result;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

    protected _readProductTypes() {
        this._subscriptions.push(
            this._productTypesService.readProductTypesList()
                .pipe(
                    tap(e => {
                        this.productTypes = e;
                    })
                )
                .subscribe()
        );
    }

    protected _readActivityTypes() {
        this._subscriptions.push(
            this._activityTypesService.readActivityTypesList()
                .pipe(
                    tap(e => {
                        this.activityTypes = e;
                    })
                )
                .subscribe()
        );
    }
}
