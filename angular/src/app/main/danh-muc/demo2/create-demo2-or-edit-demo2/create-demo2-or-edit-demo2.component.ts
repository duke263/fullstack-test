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
import { StaffServiceProxy, LookupTableServiceProxy, LookupTableDto, StaffCreateInput, StaffSto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { CommonComponent } from '@shared/dft/components/common.component';
import { forkJoin } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { FileDownloadService } from '@shared/file-download.service';
import { AppComponentBase } from '@shared/app-component-base';
import { AbpValidationError } from '@shared/components/validation/abp-validation.api';
const URL = AppConsts.remoteServiceBaseUrl + '/api/Upload/';
@Component({
  templateUrl: './create-demo2-or-edit-demo2.component.html',
})
export class CreateDemo2OrEditDemo2Component extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  form : FormGroup;
  saving = false;
  isEdit = false;
  staffs: LookupTableDto[] = [];
  arrTrangThaiDuyet: LookupTableDto[] = [];
  arrTrangHieuLuc: LookupTableDto[] = [];
  staffSto: StaffCreateInput = new StaffCreateInput();
  id: number;
  isView: boolean;
  suggestionsSingle: LookupTableDto[];
  staffSelectedSingle: LookupTableDto;
  suggestionsMultiple: LookupTableDto[];
  staffSelectedMultiple: LookupTableDto;
  constructor(
    injector: Injector,
    private _fb: FormBuilder,
    public http: HttpClient,
    private _lookupTableService: LookupTableServiceProxy,
    public bsModalRef: BsModalRef,
    private _staffService: StaffServiceProxy,
  ) {
    super(injector);
  }

  logg(event) {
    console.log(99, event);
  }

  ngOnInit(): void {
    // folkjoin(
    //   this._lookupTableService.getAll()
    // ).subscribe
    this.khoiTaoForm();
  }

  khoiTaoForm() {
    this.form = this._fb.group({
      Ma: '',
      Name: '',
      Address: '',
      Email:'',
    });
  }

}
