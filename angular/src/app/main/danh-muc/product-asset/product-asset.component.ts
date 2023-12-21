import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateInputDto, GetAllOutputDto, InputProductDto,
  ProductsAssetServiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { LazyLoadEvent } from 'primeng';
import { Table } from 'primeng/table';
import { CreateProductComponent } from './create-product/create-product.component';
import { EditProductComponent } from './edit-product/edit-product.component';
import { finalize } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { FileDownloadService } from '@shared/file-download.service';
import { AppConsts } from '@shared/AppConsts';
import { ImportExcelDialogComponent } from '@shared/components/import-excel/import-excel-dialog.component';

const URL = AppConsts.remoteServiceBaseUrl + '/api/Upload/ProductUpload';

@Component({
  selector: 'app-product-asset',
  templateUrl: './product-asset.component.html',
  styleUrls: ['./product-asset.component.scss']
})
export class ProductAssetComponent extends AppComponentBase implements OnInit {
  @ViewChild('dt') table: Table;
  form: FormGroup;
  keyword = '';
  loading = true;
  totalCount = 0;
  productAsset: CreateInputDto[] = [];
  records: GetAllOutputDto[] = [];
  exporting = false;
  input: InputProductDto;
  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _productService: ProductsAssetServiceServiceProxy,
    public http: HttpClient,
    private _fileDownloadService: FileDownloadService,
  ) {super(injector); }

  ngOnInit(): void {
    this.productFormCreate();
  }

  productFormCreate() {
    this.form = new FormGroup({
      TextInput1: new FormControl(),
      TextInput2: new FormControl()
    });
  }

  getDataPage(lazyLoad?: LazyLoadEvent) {
    this.loading = true;
    this._productService
    .getAll(
      this.keyword || undefined,
      // this.getSortField(this.table),
      lazyLoad ? lazyLoad.first : this.table.first,
      lazyLoad ? lazyLoad.rows : this.table.rows,
    ).pipe(finalize(() => { this.loading = false; }))
    .subscribe(result => {
      this.records = result.items;
      this.totalCount = result.totalCount;
    });
  }

  createProduct(): void {
    this._showCreateProductOrEditProductComponent();
  }

  editProduct(id?: number): void {
    this._showCreateProductOrEditProductComponent(id);
  }

  viewProduct(id?: number): void {
    this._showCreateProductOrEditProductComponent(id, true);
  }

  deleteProduct(product: CreateInputDto) {
    this.swal.fire({
      title: 'Are your sure?',
      text: 'Asset ' + product.productName + ' sẽ bị Delete!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: this.confirmButtonColor,
      cancelButtonColor: this.cancelButtonColor,
      cancelButtonText: this.cancelButtonText,
      confirmButtonText: this.confirmButtonText
    }).then((result) => {
      if (result.value) {
        this._productService.deleteProductp(product.id).subscribe(() => {
          this.showDeleteMessage();
          this.getDataPage();
        });
      }
    });
  }

  importExcel() {
    this._showImportDemoDialog();
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
      this._productService.downloadFileMau().subscribe(result => {
        importExcelDialog.content.downLoading = false;
        this._fileDownloadService.downloadTempFile(result);
      });
    });

    // Upload
    importExcelDialog.content.onSave.subscribe((ouput) => {
      importExcelDialog.content.returnMessage = 'Đang upload file....';
      const formdata = new FormData();
      for (let i = 0; i < ouput.length; i++) {
        formdata.append((i + 1) + '', ouput[i]);
      }
      this.http.post(URL, formdata).subscribe((res) => {
        this._productService.importFileExcel(res['result'][0]).subscribe({
          next: (result) => {
          importExcelDialog.content.returnMessage = result;
          },
          error: (e) => {
          importExcelDialog.content.uploading = false;
          importExcelDialog.content.saveDisabled = false;
          },
          complete: () => {
            importExcelDialog.content.uploading = false;
            importExcelDialog.content.uploadDone();
          },
        });
      });
    });

    // Close
    importExcelDialog.content.onClose.subscribe(() => {
      this.getDataPage();
    });
  }

  exportToExcel() {
    this.exporting = true;
    this.input = new InputProductDto();
    this.input.skipCount = 0;
    this.input.maxResultCount = 10000000;
    this._productService.exportToExcel(this.input).pipe(finalize(() => {
      this.exporting = false;
    })).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  formatNumberWithCommas(number) {
    // tslint:disable-next-line:tsr-detect-unsafe-regexp
    return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  }

  private _showCreateProductOrEditProductComponent(id?: number, isView = false): void {
    // tslint:disable-next-line:prefer-const
    let createOrEditProduct: BsModalRef;
    if (!id) {
      createOrEditProduct = this._modalService.show(
        CreateProductComponent,
        {
          class: 'modal-xl',
        }
      );
    } else {
      createOrEditProduct = this._modalService.show(
        EditProductComponent,
        {
          class: 'modal-xl',
          initialState: {
            // tslint:disable-next-line:object-literal-shorthand
            id: id,
            isView,
          },
        }
      );
    }

    // ouput emit
    createOrEditProduct.content.onSave.subscribe(() => {
      this.getDataPage();
    });
  }

}
