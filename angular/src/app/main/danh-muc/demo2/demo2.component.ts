import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FileDownloadService } from '../../../../shared/file-download.service';
import { DemoServiceProxy, DemoForView, DemoGetAllInputDto, LookupTableDto } from './../../../../shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FormGroup, FormControl } from '@angular/forms';
import { DemoDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { finalize } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CreateDemo2OrEditDemo2Component } from './create-demo2-or-edit-demo2/create-demo2-or-edit-demo2.component';
import { ImportExcelDialogComponent } from '@shared/components/import-excel/import-excel-dialog.component';
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
  demos: DemoForView[] = [];
  input: DemoGetAllInputDto;
  totalCount = 0;
  config = {
    animated: false
  };

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _demoService: DemoServiceProxy,
    public http: HttpClient,
    private _fileDownloadService: FileDownloadService,

  ) { super(injector); }

  ngOnInit(): void {
    this.khoiTaoForm();
  }

  khoiTaoForm() {
    this.form = new FormGroup({
      TextInput1: new FormControl(),
      TextInput2: new FormControl()
    });
  }

  getDataPage(lazyLoad?: LazyLoadEvent) {
    this.loading = true;
    this._demoService.getAll(
      this.keyword,
      lazyLoad ? lazyLoad.first : this.table.first,
      lazyLoad ? lazyLoad.rows : this.table.rows,
    ).pipe(finalize(() => { this.loading = false; }))
      .subscribe(result => {
        this.demos = result.items;
        this.totalCount = result.totalCount;
      });
  }

  createDemo2(id?: number) {
    this._showCreateDemo2OrEditDemo2Component(id);
  }

  viewDemo2(id?: number) {
    this._showCreateDemo2OrEditDemo2Component(id, true);
  }

  exportToExcel() {
    this.exporting = true;
    this.input = new DemoGetAllInputDto();
    this.input.skipCount = 0;
    this.input.maxResultCount = 10000000;
    this._demoService.exportToExcel(this.input).pipe(finalize(() => {
      this.exporting = false;
    })).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  importExcel()  {
    this._showImportDemoDialog();
  }

  protected _deletaDemo2(demo2: DemoDto) {
    this.swal.file({
      title: 'Bạn có chắc chắn không?',
      text: 'ID ' + demo2.ma + ' sẽ bị xóa!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: this.confirmButtonColor,
      cancelButtonColor: this.cancelButtonColor,
      cancelButtonText: this.cancelButtonText,
      confirmButtonText: this.confirmButtonText
    }).then((result) => {
      if (result.value) {
        this._demoService.delete(demo2.id).subscribe(() => {
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

  private _showImportDemoDialog(): void {
    let importExcelDialog: BsModalRef;

    importExcelDialog = this._modalService.show(
      ImportExcelDialogComponent,
      {
        class: 'modal-lg',
        ignoreBackdropClick: true,
        initialState: {
          maxFile: 1,
          excelAcceptTypes: this.excelAcceptTypes
        }
      }
    );

    // Tải file mẫu
    importExcelDialog.content.onDownload.subscribe(() => {
      this._demoService.downloadFileMau().subscribe(result => {
        importExcelDialog.content.downLoading = false;
        this._fileDownloadService.downloadTempFile(result);
      });
    });

    // Close
    importExcelDialog.content.onClose.subscribe(() => {
      this.getDataPage();
    });
  }
}
