import { Injectable } from '@angular/core';
import { UserService } from './user.service';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ApiUrls } from '../common/api-urls';
import { User, Role, Credentials, ChangePasswordModel, UpdateUserRequest } from './models';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';

@Injectable()
export class SecurityService {

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/security`

    constructor(
        private readonly _http: HttpClient,
        private readonly _userService: UserService
    ) {
    }

    login(credentials: Credentials) {
        return this._http.post<User>(`${this._baseUrl}/login`, credentials)
            .pipe(map(user => this._userService.setUser(user)));
    }

    logout() {
        return this._http.post(`${this._baseUrl}/logout`, null)
            .pipe(map(() => this._userService.removeUser()));
    }

    isAuthenticated() {
        const user = this._userService.getUser();

        return !!user && user.enabled;
    }

    isAuthorized(...roles: Role[]) {
        if (!this.isAuthenticated()) {
            return false;
        }

        const user = this._userService.getUser();

        return roles && roles.length && roles.indexOf(user.role) >= 0 ||
            !roles ||
            !roles.length;
    }

    refreshUserData() {
        return this._http.get<User>(`${this._baseUrl}/users/current`)
            .pipe(
                map(user => this._userService.setUser(user))
            );
    }

    changePassword(model: ChangePasswordModel) {
        return this._http.post<User>(`${this._baseUrl}/users/current/changepassword`, model)
            .pipe(
                map(user => this._userService.setUser(user))
            );
    }

    readUsers(state: State) {
        const params = toDataSourceRequestString(state);
        const hasGroups = state.group && state.group.length;

        return this._http.get<GridDataResult>(`${this._baseUrl}/users?${params}`)
            .pipe(
                map(e =>
                    <GridDataResult>{
                        data: hasGroups ? translateDataSourceResultGroups(e.data) : e.data,
                        total: e.total
                    }
                )
            );
    }

    createUser(request: UpdateUserRequest) {
        return this._http.post<User>(`${this._baseUrl}/users`, request)
            .pipe(
                map(e => User.build(e))
            );
    }

    updateUser(request: UpdateUserRequest, id: number) {
        return this._http.put<void>(`${this._baseUrl}/users/${id}`, request)
            .pipe(
                map(() => { })
            );
    }

    getUser(id: number) {
        return this._http.get<User>(`${this._baseUrl}/users/${id}`)
            .pipe(
                map(e => User.build(e))
            );
    }
}

export function refreshUserData(security: SecurityService, user: UserService) {

    return () => user.hasUser() && security.refreshUserData().toPromise();

}
