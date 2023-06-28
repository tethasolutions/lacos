import { Injectable } from '@angular/core';
import { User } from './models';
import { StorageService } from '../common/storage.service';
import { Subject } from 'rxjs';

@Injectable()
export class UserService {

    private readonly _userChanged = new Subject<User>();
    private readonly _userKey = 'currentUser';
    private _currentUser: User = null;

    readonly userChanged = this._userChanged.asObservable();

    constructor(
        private readonly _storage: StorageService
    ) {
    }

    getUser() {
        if (this._currentUser) {
            return this._currentUser;
        }

        const json = this._storage.get<User>(this._userKey, false);

        if (json) {
            this._currentUser = User.build(json);
        }

        return this._currentUser;
    }

    setUser(user: User) {
        this._removeUser();

        user = User.build(user);

        this._storage.save(user, this._userKey, false);
        this._currentUser = user;

        this._userChanged.next(this._currentUser);

        return this._currentUser;
    }

    removeUser() {
        this._removeUser();

        this._userChanged.next(null);
    }

    hasUser() {
        return !!this._storage.get(this._userKey, false);
    }

    private _removeUser() {
        this._storage.delete(this._userKey, false);
        this._currentUser = null;
    }
}
