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

    canActivate(route: ActivatedRouteSnapshot, _: RouterStateSnapshot) {
        if (!this._security.isAuthenticated()) {
            this._router.navigate(['/login']);
            return false;
        }

        const url = `/${route.url.join('/')}`;

        switch (true) {
            case url === '/':
            case url === '/home':
            case (/^\/activities\/([0-9]{1,}?)$/gi).test(url):
            case url === '/interventions':
            case url === '/interventions-list':
            case url === '/tickets':
            case url === '/orders':
            case url === '/activities':
            case url === '/users':
            case url === '/customers':
            case url === '/suppliers':
            case url === '/operators':
            case url === '/vehicles':
            case url === '/checklist':
            case url === '/products':
            case url === '/activitytypes':
            case url === '/producttypes':
            case url === '/jobs':
            case url === '/jobs-completed':
            case url === '/purchase-orders':
                return this._security.isAuthenticated();
            //return this._security.isAuthorized(Role.Operator);
            default:
                throw new Error(`Url ${url} sconosciuto`);
        }
    }

}
