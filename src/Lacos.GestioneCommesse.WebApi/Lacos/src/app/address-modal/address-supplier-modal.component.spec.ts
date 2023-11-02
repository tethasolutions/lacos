import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddressSupplierModalComponent } from './address-supplier-modal.component';

describe('AddressSupplierModalComponent', () => {
  let component: AddressSupplierModalComponent;
  let fixture: ComponentFixture<AddressSupplierModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddressSupplierModalComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddressSupplierModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
