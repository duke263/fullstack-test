import { StaffSto } from './../../../../../shared/service-proxies/service-proxies';
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
// const URL = AppConsts.remoteServiceBaseUrl + '/api/Upload/DemoUpload';
@Component({
  templateUrl: './create-demo2-or-edit-demo2.component.html',
})
export class CreateDemo2OrEditDemo2Component extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  form : FormGroup;
  saving = false;
  staff = new StaffSto();
  // id: number;
  // isView: boolean;
  constructor(
    injector: Injector,
    private fb: FormBuilder,
    public bsModalRef: BsModalRef,
    public _staffService: StaffServiceProxy,
  ) {
    super(injector);
  }

  logg(event) {
    console.log(99, event);
  }

  ngOnInit(): void {
    this.khoiTaoForm();
    // forkJoin(
    //   this._lookupTableService.getAllStaff(),
    // ).subscribe(([stafff]) => {
    //   this.staff = stafff;
    //   if (!this.id) {
    //     // Thêm mới
    //     this.staffs = new StaffCreateInput();
    //     this.isEdit = false;
    //   } else {
    //       this.isEdit = true;
    //       // Sửa
    //       this._staffService.getForEdit(this.id).subscribe(item => {
    //           this.staff = item;
    //           this._setValueForEdit();
    //       });
    //   }
    //   if (this.isView) {
    //       this.form.disable();
    //   } else {
    //       this.form.enable();
    //   }
    // });
  }

  khoiTaoForm() {
    this.form = this.fb.group({
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
      } else {
          this._staffService
            .createOrEdit(this.staff).subscribe(() => {
              this.showCreateMessage();
              this.bsModalRef.hide();
              this.onSave.emit();
            });
      }

      // const formdata = new FormData();
      // this.http.post(URL, formdata).subscribe((res) => {
      //   this._getValueForSave();
      //   this._staffService.createOrEdit(this.staff).subscribe((result) => {
      //       if (result === 1) {
      //           this.showSwalAlertMessage('Mã đã bị trùng!', 'error');
      //       } else if (!this.id) {
      //           this.showCreateMessage();
      //           this.bsModalRef.hide();
      //           this.onSave.emit();
      //       } else {
      //           this.showUpdateMessage();
      //           this.bsModalRef.hide();
      //           this.onSave.emit();
      //       }
      //   });
      // });
    }

    console.log(this.form);
  }

  private _getValueForSave() {
    // this.staff.id = this.form.controls.id.value;
    this.staff.ma = this.form.controls.Ma.value;
    this.staff.name = this.form.controls.Name.value;
    this.staff.address = this.form.controls.Address.value;
    this.staff.email = this.form.controls.Email.value;
  }

}
