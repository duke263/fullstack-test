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
import {LookupTableServiceProxy, LookupTableDto} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { CommonComponent } from '@shared/dft/components/common.component';
import { forkJoin } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { ValidationComponent } from '@shared/dft/components/validation-messages.component';

@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  styleUrls: ['./edit-product.component.scss']
})
export class EditProductComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  form : FormGroup;
  saving = false;
  isView = false;
  id: number;
  inputDto: CreateInputDto = new CreateInputDto();
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
    this._productService.viewProduct(this.id).subscribe((result) => {
      this.inputDto = result;
      this._setValueForEdit();
    });
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
      this._productService.updateProduct(this.inputDto).pipe(
        finalize(() => {
          this.saving = false;
        })
      ).subscribe(() => {
        this.showUpdateMessage();
        this.bsModalRef.hide();
        this.onSave.emit();
      });
    }

  }

  private _setValueForEdit() {
    // this.form.controls.id.setValue(this.inputDto.id);
    this.form.controls.productName.setValue(this.inputDto.productName);
    this.form.controls.price.setValue(this.inputDto.price)
    this.form.controls.category.setValue(this.inputDto.category)
    this.form.controls.description.setValue(this.inputDto.description)
    if (this.isView) {
      this.form.disable();
    }
  }

  private _getValueForSave() {
    this.inputDto.productName = this.form.controls.productName.value;
    this.inputDto.price = this.form.controls.price.value;
    this.inputDto.category = this.form.controls.category.value;
    this.inputDto.description = this.form.controls.description.value;
  }
}
