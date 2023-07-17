import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperatorDocumentModalComponent } from './operator-document-modal.component';

describe('OperatorDocumentModalComponent', () => {
  let component: OperatorDocumentModalComponent;
  let fixture: ComponentFixture<OperatorDocumentModalComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [OperatorDocumentModalComponent]
    });
    fixture = TestBed.createComponent(OperatorDocumentModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
