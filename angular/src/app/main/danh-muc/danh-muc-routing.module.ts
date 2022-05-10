import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { DemoComponent } from './demo/demo.component';
import { AppRouteGuard } from '../../../shared/auth/auth-route-guard';
import { Demo2Component } from './demo2/demo2.component';
@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: 'demo',
                component: DemoComponent,
                canActivate: [AppRouteGuard]
            },

            {
                path: 'demo2', component: Demo2Component,
                canActivate: [AppRouteGuard]
            },
        ])
    ],
    exports: [RouterModule],
    declarations: [],
    providers: [],
})
export class DanhMucRoutingModule { }
