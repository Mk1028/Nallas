import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonModulesComponent } from './common-modules.component';

describe('CommonModulesComponent', () => {
  let component: CommonModulesComponent;
  let fixture: ComponentFixture<CommonModulesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CommonModulesComponent]
    });
    fixture = TestBed.createComponent(CommonModulesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
