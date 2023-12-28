import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../base.component';
import { PurchaseOrderStatus, purchaseOrderStatusNames } from 'src/app/services/purchase-orders/models';

@Pipe({
    name: 'purchaseOrderStatus'
})
export class PurchaseOrderStatusPipe extends BaseComponent implements PipeTransform {

    transform(value: PurchaseOrderStatus) {
        return purchaseOrderStatusNames[value];
    }

}
