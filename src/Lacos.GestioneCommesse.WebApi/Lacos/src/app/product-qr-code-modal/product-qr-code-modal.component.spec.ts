import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductQrCodeModalComponent } from './product-qr-code-modal.component';

describe('ProductQrCodeModalComponent', () => {
  let component: ProductQrCodeModalComponent;
  let fixture: ComponentFixture<ProductQrCodeModalComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ProductQrCodeModalComponent]
    });
    fixture = TestBed.createComponent(ProductQrCodeModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
