import { Component } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { SecurityService } from '../services/security/security.service';
import { Role } from '../services/security/models';

@Component({
    selector: 'lacos-home',
    templateUrl: 'home.component.html'
})
export class HomeComponent extends BaseComponent {

    readonly isAdmin: boolean;
    readonly isOperator: boolean;

    constructor(
        security: SecurityService
    ) {
        super();

        this.isAdmin = security.isAuthorized(Role.Administrator);
        this.isOperator = security.isAuthorized(Role.Operator);
    }

}
