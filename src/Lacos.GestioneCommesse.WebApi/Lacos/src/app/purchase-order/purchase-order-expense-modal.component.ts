import { Component, Input, input, OnInit, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { tap } from 'rxjs';
import { MessageBoxService } from '../services/common/message-box.service';
import { ProductsService } from '../services/products.service';
import { ProductReadModel } from '../shared/models/product.model';
import { State } from '@progress/kendo-data-query';
import { ApiUrls } from '../services/common/api-urls';
import { PurchaseOrderExpense } from '../services/purchase-orders/models';
import { SelectableJob } from '../services/jobs/models';

@Component({
    selector: 'app-purchase-order-expense-modal',
    templateUrl: 'purchase-order-expense-modal.component.html'
})
export class PurchaseOrderExpenseModalComponent extends ModalFormComponent<PurchaseOrderExpense> implements OnInit {

    @Input() jobs = new Array<SelectableJob>();

    constructor(
        messageBox: MessageBoxService
    ) {
        super(messageBox);
    }

    ngOnInit() {
    }

    onJobChange() {
        const job = this.jobs
            .find(e => e.id === this.options.jobId);

        this.options.jobCode = job?.code;
    }

    protected override _canClose() {
        this.form.markAsDirty();

        if (this.form.invalid) {
            this._messageBox.error('Compilare correttamente tutti i campi.');
        }

        return this.form.valid;
    }

}
