import { Component, Input, input, OnInit, ViewChild } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { MessageBoxService } from '../services/common/message-box.service';
import { PurchaseOrderExpense } from '../services/purchase-orders/models';
import { SelectableJob } from '../services/jobs/models';
import { SecurityService } from '../services/security/security.service';
import { Role } from '../services/security/models';

@Component({
    selector: 'app-purchase-order-expense-modal',
    templateUrl: 'purchase-order-expense-modal.component.html'
})
export class PurchaseOrderExpenseModalComponent extends ModalFormComponent<PurchaseOrderExpense> implements OnInit {

    @Input() jobs = new Array<SelectableJob>();

    readonly isAdmin: boolean = false;

    constructor(
        messageBox: MessageBoxService,
        readonly security: SecurityService
    ) {
        super(messageBox);
        this.isAdmin = security.isAuthorized(Role.Administrator);
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

        if (this.form.valid) {
            this.options.totalAmount = (this.options.quantity ?? 0) * (this.options.unitPrice ?? 0);
        }

        return this.form.valid;
    }

}
