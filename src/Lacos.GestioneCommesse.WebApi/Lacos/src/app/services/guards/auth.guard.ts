import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { SecurityService } from '../security/security.service';
import { Injectable, inject } from '@angular/core';
import { Role } from '../security/models';

@Injectable()
export class AuthGuard {

    static readonly asInjectableGuard: CanActivateFn = (r: ActivatedRouteSnapshot, s: RouterStateSnapshot) => inject(AuthGuard).canActivate(r, s);

    constructor(
        private readonly _security: SecurityService,
        private readonly _router: Router
    ) {
    }

    canActivate(_: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (!this._security.isAuthenticated()) {
            this._router.navigate(['/login']);
            return false;
        }

        const url = state.url;

        switch (true) {
            case url === '/':
            case url === '/home':
                return this._security.isAuthenticated();
            case url === '/users':
                return this._security.isAuthorized(Role.Administrator);
            default:
                throw new Error(`Url ${state.url} sconosciuto`);
        }
    }

}
