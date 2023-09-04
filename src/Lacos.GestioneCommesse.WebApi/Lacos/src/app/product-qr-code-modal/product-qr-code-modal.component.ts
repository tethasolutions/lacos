import { Component, ViewChild, Input } from '@angular/core';
import { ProductReadModel } from '../shared/models/product.model';
import { ModalComponent } from '../shared/modal.component';
import { QRCodeComponent } from '@progress/kendo-angular-barcodes';
import { saveAs } from "@progress/kendo-file-saver";

@Component({
  selector: 'app-product-qr-code-modal',
  templateUrl: './product-qr-code-modal.component.html',
  styleUrls: ['./product-qr-code-modal.component.scss']
})

export class ProductQrCodeModalComponent extends ModalComponent<ProductReadModel> {

  @ViewChild("qrcode")
  private qrcode: QRCodeComponent;
  private qrCodeNum: string;

  protected _canClose() {
    return true;
  }

  public exportQRCode(): void {
    this.qrcode.exportImage().then((dataURI) => {
      saveAs(dataURI, this.options.qrCode + ".png");
    });
  }
  
}
