import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditDemo2Component } from './edit-demo2.component';

describe('EditDemo2Component', () => {
  let component: EditDemo2Component;
  let fixture: ComponentFixture<EditDemo2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditDemo2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditDemo2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
