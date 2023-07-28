import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChecklistItemModalComponent } from './checklist-item-modal.component';

describe('ChecklistItemModalComponent', () => {
  let component: ChecklistItemModalComponent;
  let fixture: ComponentFixture<ChecklistItemModalComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ChecklistItemModalComponent]
    });
    fixture = TestBed.createComponent(ChecklistItemModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
