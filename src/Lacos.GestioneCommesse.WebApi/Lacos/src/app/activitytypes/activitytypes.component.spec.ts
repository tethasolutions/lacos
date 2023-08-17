import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivityTypesComponent } from './activitytypes.component';

describe('ActivityTypesComponent', () => {
  let component: ActivityTypesComponent;
  let fixture: ComponentFixture<ActivityTypesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ActivityTypesComponent]
    });
    fixture = TestBed.createComponent(ActivityTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
