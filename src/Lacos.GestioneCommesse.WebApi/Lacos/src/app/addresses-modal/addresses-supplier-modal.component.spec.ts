import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddressesSupplierModalComponent } from './addresses-supplier-modal.component';

describe('AddressesModalComponent', () => {
  let component: AddressesSupplierModalComponent;
  let fixture: ComponentFixture<AddressesSupplierModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddressesSupplierModalComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddressesSupplierModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
