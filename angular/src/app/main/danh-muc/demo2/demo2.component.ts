import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FileDownloadService } from '../../../../shared/file-download.service';
import { StaffServiceProxy, StaffForView, DemoGetAllInputDto, LookupTableDto } from './../../../../shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FormGroup, FormControl } from '@angular/forms';
import { StaffSto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { finalize } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CreateDemo2OrEditDemo2Component } from './create-demo2-or-edit-demo2/create-demo2-or-edit-demo2.component';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
const URL = AppConsts.remoteServiceBaseUrl + '/api/Upload/DemoUpload';
@Component({
  selector: 'app-demo2',
  templateUrl: './demo2.component.html',
  animations: [appModuleAnimation()],
})
export class Demo2Component extends AppComponentBase implements OnInit {
  @ViewChild('dt') table: Table;
  form: FormGroup;
  keyword = '';
  advancedFiltersVisible = false;
  isActive = false;
  loading = true;
  exporting = false;
  staff: StaffForView[] = [];
  input: DemoGetAllInputDto;
  totalCount = 0;
  config = {
    animated: false
  };

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _staffService: StaffServiceProxy,
    public http: HttpClient,

  ) { super(injector); }

  ngOnInit(): void {
    this.khoiTaoForm();
    console.log(1);
  }

  khoiTaoForm() {
    this.form = new FormGroup({
      TextInput1: new FormControl(),
      TextInput2: new FormControl()
    });
  }

  getDataPage(lazyLoad?: LazyLoadEvent) {
    this.loading = true;
    this._staffService.getAll(
      this.keyword,
      lazyLoad ? lazyLoad.first : this.table.first,
      lazyLoad ? lazyLoad.rows : this.table.rows,
    ).pipe(finalize(() => { this.loading = false; }))
      .subscribe(result => {
        this.staff = result.items;
        this.totalCount = result.totalCount;
        console.log(66 , this.staff);
      });

  }

  createDemo2(id?: number) {
    this._showCreateDemo2OrEditDemo2Component(id);
  }

  viewDemo2(id?: number) {
    this._showCreateDemo2OrEditDemo2Component(id, true);
  }




  protected _deletaDemo2(staff: StaffSto) {
    this.swal.file({
      title: 'Bạn có chắc chắn không?',
      text: 'ID ' + staff.id + ' sẽ bị xóa!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: this.confirmButtonColor,
      cancelButtonColor: this.cancelButtonColor,
      cancelButtonText: this.cancelButtonText,
      confirmButtonText: this.confirmButtonText
    }).then((result) => {
      if (result.value) {
        this._staffService.delete(staff.id).subscribe(() => {
          this.showDeleteMessage();
          this.getDataPage();
        });
      }
    });
  }

  private _showCreateDemo2OrEditDemo2Component (id?: number, isView = false): void
  {
    //coppy
    let createOrEditUserDialog: BsModalRef;
    createOrEditUserDialog = this._modalService.show(
      CreateDemo2OrEditDemo2Component,
      {
        class: 'modal-xl',
        ignoreBackdropClick: true,
        initialState: {
          id,
          isView,
        },
      }
    );

    // ouput emit
    createOrEditUserDialog.content.onSave.subscribe(() => {
      this.getDataPage();
    });
  }
}
