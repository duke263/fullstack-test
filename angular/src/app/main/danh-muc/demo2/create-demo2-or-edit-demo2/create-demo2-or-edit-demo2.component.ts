// tslint:disable
import { Demo_File, FlatTreeSelectDto } from '@shared/service-proxies/service-proxies';
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
import { DemoServiceProxy, LookupTableServiceProxy, LookupTableDto, DemoCreateInput } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { CommonComponent } from '@shared/dft/components/common.component';
import { forkJoin } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { FileDownloadService } from '@shared/file-download.service';
import { AppComponentBase } from '@shared/app-component-base';
import { TreeviewItem } from '@shared/dft/dropdown-treeview-select/lib/models/treeview-item';
import { PermissionTreeEditModel } from '@app/roles/lib/permission-tree-edit.model';
import { CheckboxTreeEditModel } from '@shared/dft/multiple-select-tree/lib/checkbox-tree-edit.model';
const URL = AppConsts.remoteServiceBaseUrl + '/api/Upload/DemoUpload';

@Component({
  templateUrl: './create-demo2-or-edit-demo2.component.html',
})
export class CreateDemo2OrEditDemo2Component extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  form : FormGroup;
  saving = false;
  isEdit = false;
  demos: LookupTableDto[] = [];
  arrTrangThaiDuyet: LookupTableDto[] = [];
  arrTrangHieuLuc: LookupTableDto[] = [];
  demoDto: DemoCreateInput = new DemoCreateInput();
  id: number;
  isView: boolean;
  suggestionsSingle: LookupTableDto[];
  demoSelectedSingle: LookupTableDto;
  suggestionsMultiple: LookupTableDto[];
  demoSelectedMultiple: LookupTableDto;
  constructor(
    injector: Injector,
  ) {
    super(injector);
  }

  ngOnInit(): void {
  }

}
