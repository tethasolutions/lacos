import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './services/guards/auth.guard';
import { UsersComponent } from './security/users.component';
import { LogoutComponent } from './security/logout.component';
import { LoginComponent } from './security/login.component';
import { HomeComponent } from './home/home.component';
import { CustomersComponent } from './customers/customers.component';

const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'logout', component: LogoutComponent },
    { path: 'users', component: UsersComponent, canActivate: [AuthGuard.asInjectableGuard] },
    { path: 'customers', component: CustomersComponent, canActivate: [AuthGuard.asInjectableGuard] }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { useHash: true, onSameUrlNavigation: 'reload' })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
