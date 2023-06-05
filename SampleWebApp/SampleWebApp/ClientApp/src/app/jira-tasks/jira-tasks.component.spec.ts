import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JiraTasksComponent } from './jira-tasks.component';

describe('JiraTasksComponent', () => {
  let component: JiraTasksComponent;
  let fixture: ComponentFixture<JiraTasksComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [JiraTasksComponent]
    });
    fixture = TestBed.createComponent(JiraTasksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
