import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductAssetComponent } from './product-asset.component';

describe('ProductAssetComponent', () => {
  let component: ProductAssetComponent;
  let fixture: ComponentFixture<ProductAssetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductAssetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductAssetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
