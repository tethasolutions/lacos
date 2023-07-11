import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperatorDocumentsModalComponent } from './operator-documents-modal.component';

describe('OperatorDocumentsModalComponent', () => {
  let component: OperatorDocumentsModalComponent;
  let fixture: ComponentFixture<OperatorDocumentsModalComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [OperatorDocumentsModalComponent]
    });
    fixture = TestBed.createComponent(OperatorDocumentsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
