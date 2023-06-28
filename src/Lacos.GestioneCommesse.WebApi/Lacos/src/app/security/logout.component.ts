import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { SecurityService } from '../services/security/security.service';
import { Router } from '@angular/router';
import { tap } from 'rxjs';

@Component({
    selector: 'app-logout',
    template: ''
})
export class LogoutComponent extends BaseComponent implements OnInit {

    constructor(
        private readonly _security: SecurityService,
        private readonly _router: Router
    ) {
        super();
    }

    ngOnInit() {
        this._subscriptions.push(
            this._security.logout()
                .pipe(
                    tap(() => this._router.navigateByUrl('/login'))
                )
                .subscribe()
        );
    }
}
