import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivityTypeModalComponent } from './activitytype-modal.component';

describe('ActivityTypeModalComponent', () => {
  let component: ActivityTypeModalComponent;
  let fixture: ComponentFixture<ActivityTypeModalComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ActivityTypeModalComponent]
    });
    fixture = TestBed.createComponent(ActivityTypeModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
