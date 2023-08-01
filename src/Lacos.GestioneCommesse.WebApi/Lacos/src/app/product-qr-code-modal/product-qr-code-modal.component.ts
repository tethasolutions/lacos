import { Component, ViewChild, Input } from '@angular/core';
import { ProductModel } from '../shared/models/product.model';
import { ModalComponent } from '../shared/modal.component';

@Component({
  selector: 'app-product-qr-code-modal',
  templateUrl: './product-qr-code-modal.component.html',
  styleUrls: ['./product-qr-code-modal.component.scss']
})
export class ProductQrCodeModalComponent extends ModalComponent<ProductModel> {

  protected _canClose() {
    return true;
  }
}
