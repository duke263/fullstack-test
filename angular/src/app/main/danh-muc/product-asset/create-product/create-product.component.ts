import { CreateInputDto, ProductsAssetServiceServiceProxy, StaffSto } from './../../../../../shared/service-proxies/service-proxies';
// tslint:disable
import {
    Component,
    Injector,
    OnInit,
    EventEmitter,
    Output
} from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import * as _ from 'lodash';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StaffServiceProxy, LookupTableServiceProxy, LookupTableDto, StaffCreateInput } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { CommonComponent } from '@shared/dft/components/common.component';
import { forkJoin } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { ValidationComponent } from '@shared/dft/components/validation-messages.component';

@Component({
  selector: 'app-create-product',
  templateUrl: './create-product.component.html',
  styleUrls: ['./create-product.component.scss']
})
export class CreateProductComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  form : FormGroup;
  saving = false;
  staff = new StaffSto();
  createInputDto: CreateInputDto = new CreateInputDto();
  // id: number;
  // isView: boolean;
  constructor(
    injector: Injector,
    private fb: FormBuilder,
    public bsModalRef: BsModalRef,
    private _productService: ProductsAssetServiceServiceProxy,
    public http: HttpClient
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.CreateProductForm();
  }

  CreateProductForm() {
    this.form = this.fb.group({
      productName: ['', [Validators.required, Validators.maxLength(255),
        ValidationComponent.KtraKhoangTrang]],
      price: ['', [Validators.required, Validators.maxLength(50),
        ValidationComponent.KtraKhoangTrang]],
      category: ['', [Validators.required, Validators.maxLength(255),
        ValidationComponent.KtraKhoangTrang]],
      description:['',[Validators.maxLength(4000),
        ValidationComponent.KtraKhoangTrang]],
    });
  }

  save(): void {
    if (CommonComponent.getControlErr(this.form) === '') {
      this.saving = true;
      this._getValueForSave();
      this._productService.createProduct(this.createInputDto).pipe(
        finalize(() => {
          this.saving = false;
        })
      ).subscribe(() => {
        this.showCreateMessage();
        this.bsModalRef.hide();
        this.onSave.emit();
      });
    }

  }

  private _getValueForSave() {
    this.createInputDto.productName = this.form.controls.productName.value;
    this.createInputDto.price = this.form.controls.price.value;
    this.createInputDto.category = this.form.controls.category.value;
    this.createInputDto.description = this.form.controls.description.value;
  }

}
