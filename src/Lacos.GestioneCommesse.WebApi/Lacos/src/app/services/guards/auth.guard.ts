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
            case url === '/customers':
            case url === '/operators':
            case url === '/vehicles':
            case url === '/checklist':
            case url === '/products':
            case url === '/activitytypes':
            case url === '/producttypes':
            case url === '/jobs':
            case (/^\/activities\/([0-9]{1,}?)$/gi).test(url):
            case url === '/interventions':
            case url === '/interventions-list':
                return this._security.isAuthenticated();
            case url === '/users':
                return this._security.isAuthorized(Role.Administrator);
            default:
                throw new Error(`Url ${state.url} sconosciuto`);
        }
    }

}
