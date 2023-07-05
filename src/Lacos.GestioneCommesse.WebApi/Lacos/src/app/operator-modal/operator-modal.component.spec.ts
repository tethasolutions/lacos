import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperatorModalComponent } from './operator-modal.component';

describe('OperatorModalComponent', () => {
  let component: OperatorModalComponent;
  let fixture: ComponentFixture<OperatorModalComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [OperatorModalComponent]
    });
    fixture = TestBed.createComponent(OperatorModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
