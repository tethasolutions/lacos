import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobsActiveComponent } from './jobs-active.component';

describe('JobsActiveComponent', () => {
  let component: JobsActiveComponent;
  let fixture: ComponentFixture<JobsActiveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ JobsActiveComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobsActiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
