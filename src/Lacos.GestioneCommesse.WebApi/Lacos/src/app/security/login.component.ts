import { Component } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { Credentials } from '../services/security/models';
import { SecurityService } from '../services/security/security.service';
import { Router } from '@angular/router';
import { tap } from 'rxjs';

@Component({
    selector: 'lacos-login',
    templateUrl: 'login.component.html'
})
export class LoginComponent extends BaseComponent {

    readonly credentials = new Credentials();

    constructor(
        private readonly _security: SecurityService,
        private readonly _router: Router
    ) {
        super();
    }

    login() {
        this._subscriptions.push(
            this._security.login(this.credentials)
                .pipe(
                    tap(() => this._router.navigateByUrl('/home'))
                )
                .subscribe()
        );
    }
}
