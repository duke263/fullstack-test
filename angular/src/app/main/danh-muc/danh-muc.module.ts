import { DanhMucRoutingModule } from './danh-muc-routing.module';
import { SharedModule } from '../../../shared/shared.module';
import { NgModule } from '@angular/core';
import { DemoComponent } from './demo/demo.component';
import { CreateOrEditDemoDialogComponent } from './demo/create-or-edtit/create-or-edit-demo-dialog.component';
import { Demo2Component } from './demo2/demo2.component';
import { CreateDemo2OrEditDemo2Component } from './demo2/create-demo2-or-edit-demo2/create-demo2-or-edit-demo2.component';
import { EditDemo2Component } from './demo2/edit-demo2/edit-demo2/edit-demo2.component';

@NgModule({
    imports: [
        SharedModule,
        DanhMucRoutingModule
    ],
    declarations: [
        DemoComponent,
        CreateOrEditDemoDialogComponent,
        Demo2Component,
        CreateDemo2OrEditDemo2Component,
        EditDemo2Component,
    ],
})
export class DanhMucModule { }
