// tslint:disable
import { Injector, ElementRef } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import {
    LocalizationService,
    PermissionCheckerService,
    FeatureCheckerService,
    NotifyService,
    SettingService,
    MessageService,
    AbpMultiTenancyService
} from 'abp-ng2-module';

import { AppSessionService } from '@shared/session/app-session.service';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { Table } from 'primeng/table';
import * as Enums from '@shared/AppEnums';
import * as Models from '@shared/AppModels';
import { TreeviewItem } from './dft/dropdown-treeview-select/lib/models/treeview-item';
import { TreeviewItemDto } from './service-proxies/service-proxies';

export abstract class AppComponentBase {

    localizationSourceName = AppConsts.localization.defaultLocalizationSourceName;
    localization: LocalizationService;
    permission: PermissionCheckerService;
    feature: FeatureCheckerService;
    notify: NotifyService;
    setting: SettingService;
    message: MessageService;
    multiTenancy: AbpMultiTenancyService;
    appSession: AppSessionService;
    elementRef: ElementRef;

    // Primeng Table Start
    paginator = true;
    showCurrentPageReport = true;
    paginatorRows = 20; // Số dòng hiển thị mặc định 1 page
    rowsPerPageOptions = [10, 20, 50, 100, 250]; // Chọn số record trong 1 page
    subHeight = 226; // thông số tính scrollHeight
    scrollHeight = '600px';  // độ rộng để table cuộn tùy theo màn hình
    khongCoDuLieu = 'Không có dữ liệu';
    scrollable = true;       // Có cuộn table không
    // Primeng Table End

    separator = ','; // Phân cách chuỗi = dấu ,

    // Datetime Start
    dateFormatPipe = 'dd/MM/yyyy'; // format cho định dạng date
    dateFormatInput = 'dd/mm/yy'; // format cho định dạng date
    dateFormatInsert = 'YYYY-MM-DD'; // format cho định dạng date để insert vào db
    dateTimeFormatInsert = 'YYYY-MM-DD HH:mm:ss'; // format cho định dạng date để insert vào db
    dateTimeFormatPipe = 'dd/MM/yyyy HH:mm:ss'; // format cho định dạng date để insert vào db
    dateFormatMonthOnlyInput = 'mm/yy'; // format cho định dạng date
    dateFormatMonthOnlyInsert = 'YYYY-MM'; // format cho định dạng date
    hourFormat12Input = '12';     // Time định dạng 12h
    hourFormat24Input = '24';     // Time định dạng 24h
    timeFormatInsert = 'HH:mm'; // Khoảng năm từ 2020-2030
    yearRange = '2020:2030'; // Khoảng năm từ 2020-2030
    today = new Date();      // Ngày hôm nay
    // Datetime End

    dropdownPlaceholder = 'Chọn';
    readonlyInput = false;    // Không cho date được nhập
    showButtonBar = true;    // Hiển thị nút today và clear
    minFractionDigits = 2;
    maxFractionDigits = 2;
    swal = Swal;

    // ImportFile Start
    excelAcceptTypes = '.xlsx,.xls';
    imateAcceptTypes = 'image/*';
    // ImportFile End

    // Dialog Delete Start
    confirmButtonColor = '#3085d6';
    cancelButtonColor = '#d33';
    cancelButtonText = 'Cancel';
    confirmButtonText = 'Delete';
    // Dialog Delete End

    first = 0;

    constructor(injector: Injector) {
        this.localization = injector.get(LocalizationService);
        this.permission = injector.get(PermissionCheckerService);
        this.feature = injector.get(FeatureCheckerService);
        this.notify = injector.get(NotifyService);
        this.setting = injector.get(SettingService);
        this.message = injector.get(MessageService);
        this.multiTenancy = injector.get(AbpMultiTenancyService);
        this.appSession = injector.get(AppSessionService);
        this.elementRef = injector.get(ElementRef);
    }

    l(key: string, ...args: any[]): string {
        let localizedText = this.localization.localize(key, this.localizationSourceName);

        if (!localizedText) {
            localizedText = key;
        }

        if (!args || !args.length) {
            return localizedText;
        }

        args.unshift(localizedText);
        return abp.utils.formatString.apply(this, args);
    }

    isGranted(permissionName: string): boolean {
        return this.permission.isGranted(permissionName);
    }

    isGrantedAny(...permissions: string[]): boolean {
        if (!permissions) {
            return false;
        }

        for (const permission of permissions) {
            if (this.isGranted(permission)) {
                return true;
            }
        }

        return false;
    }

    showCreateMessage() {
        this.notify.success("Added successfully!");
    }

    showUpdateMessage() {
        this.notify.success("Updated successfully!");
    }

    showDeleteMessage() {
        this.notify.success("Deleted successfully!");
    }

    showUploadMessage() {
        this.notify.success("Upload successfully!");
    }

    showNotifyMessage(message: string, type = 'success') {
        switch (type) {
            case 'success':
                this.notify.success(message);
                break;
            case 'warn':
                this.notify.warn(message);
                break;
            case 'error':
                this.notify.error(message);
                break;
            case 'info':
                this.notify.info(message);
                break;
        }
    }

    showSwalAlertMessage(message: string, type = 'success') {
        this.swal.fire(
            undefined,
            message,
            type,
        );
    }

    getSortField(table?: Table): string {
        return table && table.sortField ? (table.sortField + (table.sortOrder === 1 ? ' ASC' : ' DESC')) : undefined;
    }

    getLinkFile(res, fileName) {
        return res ? '\\' + res['result'][res['result']
            .findIndex(e => e.includes(fileName))].split('\\').slice(-2).join('\\') : '';
    }

    blobToFile = (theBlob: Blob, fileName: string): File => {
        var b: any = theBlob;
        //A Blob() is almost a File() - it's just missing the two properties below which we will add
        b.lastModifiedDate = new Date();
        b.name = fileName;

        //Cast to a File() type
        return <File>theBlob;
    }

    getTreeviewItem(items: TreeviewItemDto[]) {
        return items.map(e => {
            return new TreeviewItem({
                value: e.value,
                text: e.text,
                children: e.children,
                checked: e.checked,
                collapsed: e.collapsed,
                disabled: e.disabled
            });
        });
    }
}
