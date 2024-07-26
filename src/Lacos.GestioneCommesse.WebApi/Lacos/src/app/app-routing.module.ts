import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './services/guards/auth.guard';
import { UsersComponent } from './security/users.component';
import { LogoutComponent } from './security/logout.component';
import { LoginComponent } from './security/login.component';
import { HomeComponent } from './home/home.component';
import { CustomersComponent } from './customers/customers.component';
import { VehiclesComponent } from './vehicles/vehicles.component';
import { OperatorsComponent } from './operators/operators.component';
import { ChecklistComponent } from './checklist/checklist.component';
import { ProductsComponent } from './products/products.component';
import { ActivityTypesComponent } from './activitytypes/activitytypes.component';
import { ProductTypesComponent } from './producttypes/producttypes.component';
import { JobsComponent } from './jobs/jobs.component';
import { ActivityComponent } from './activities/activity.component';
import { ActivitiesService } from './services/activities/activities.service';
import { InterventionsComponent } from './interventions/interventions.component';
import { InterventionsListComponent } from './interventions/interventions-list.component';
import { TicketsComponent } from './ticket/tickets.component';
import { ActivitiesComponent } from './activities/activities.component';
import { SuppliersComponent } from './suppliers/suppliers.component';
import { PurchaseOrdersComponent } from './purchase-order/purchase-orders.component';
import { JobsCompletedComponent } from './jobs/jobs-completed.component';
import { JobDetailsComponent } from './jobs/job-details.component';
import { InterventionsKoComponent } from './interventions/interventions-ko.component';
import { ActivitiesFromProductComponent } from './activities/activities-from-product.component';
import { MessagesListComponent } from './messages/messages-list.component';

const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'logout', component: LogoutComponent },
    { path: 'users', component: UsersComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'vehicles', component: VehiclesComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'customers', component: CustomersComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'suppliers', component: SuppliersComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'operators', component: OperatorsComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'checklist', component: ChecklistComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'products', component: ProductsComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'activitytypes', component: ActivityTypesComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'producttypes', component: ProductTypesComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'jobs', component: JobsComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'jobs-completed', component: JobsCompletedComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'tickets', component: TicketsComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'activities', component: ActivitiesComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'activities-from-product', component: ActivitiesFromProductComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'activities/:activityId', component: ActivityComponent, canActivate: [AuthGuard.asInjectableGuard], resolve: { activity: ActivitiesService.asActivityDetailResolver } },
    { path: 'interventions-list', component: InterventionsListComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'interventions', component: InterventionsComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'interventions-ko', component: InterventionsKoComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'purchase-orders', component: PurchaseOrdersComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'job-details', component: JobDetailsComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'messages-list', component: MessagesListComponent, canActivate: [AuthGuard.asInjectableGuard] },
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { useHash: true, onSameUrlNavigation: 'reload' })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
