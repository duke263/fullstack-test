import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output
} from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import * as _ from 'lodash';
import { AppComponentBase } from '@shared/app-component-base';
import {
  StaffServiceProxy,
  StaffSto } from '@shared/service-proxies/service-proxies';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CommonComponent } from '@shared/dft/components/common.component';

@Component({
  templateUrl: './edit-demo2.component.html',
})
export class EditDemo2Component extends AppComponentBase implements OnInit {
  saving = false;
  staff = new StaffSto();
  id: number;
  isView = false;
  @Output() onSave = new EventEmitter<any>();
  form: FormGroup;
  constructor(
    injector: Injector,
    private _fb: FormBuilder,
    public bsModalRef: BsModalRef,
    private _staffService: StaffServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.khoiTaoForm();
    this._staffService.getForEdit(this.id).subscribe((result) => {
      this.staff = result;
      this._setValueForEdit();
    });
  }


  khoiTaoForm() {
    this.form = this._fb.group({
      Ma: ['', Validators.required],
      Name: ['', Validators.required],
      Address: ['', Validators.required],
      Email:['', Validators.required],
    });
  }

  save(): void {
    if (CommonComponent.getControlErr(this.form) === '') {
      this.saving = true;
      this._getValueForSave();
      if (this.staff.name.toLocaleLowerCase() === 'admin') {
        this.showSwalAlertMessage('Ten không được trùng!', 'error');
        this.saving = false;
        this._getValueForSave();
      } else {
          this._staffService
            .createOrEdit(this.staff).subscribe(() => {
              this.showUpdateMessage();
              this.bsModalRef.hide();
              this.onSave.emit();
            });
      }
    }
  }

  private _getValueForSave() {
    // this.staff.id = this.form.controls.id.value;
    this.staff.ma = this.form.controls.Ma.value;
    this.staff.name = this.form.controls.Name.value;
    this.staff.address = this.form.controls.Address.value;
    this.staff.email = this.form.controls.Email.value;
  }

  private _setValueForEdit() {
    // this.form.controls.id.setValue(this.staff.id);
    this.form.controls.Ma.setValue(this.staff.ma);
    this.form.controls.Name.setValue(this.staff.name);
    this.form.controls.Address.setValue(this.staff.address);
    this.form.controls.Email.setValue(this.staff.email);
    if (this.isView) {
      this.form.disable();
    }
  }
}

