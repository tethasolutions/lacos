import { Injectable } from '@angular/core';

@Injectable()
export class StorageService {

    private readonly _baseKey = 'lacos_';

    constructor(
    ) {
    }

    get<T>(key: string, session: boolean) {
        key = this._formatKey(key);
        const json = this._getStorage(session).getItem(key);
        const value = json == null
            ? null
            : JSON.parse(json) as T;

        return value;
    }

    save<T>(value: T, key: string, session: boolean) {
        key = this._formatKey(key);
        const json = JSON.stringify(value);

        this._getStorage(session).setItem(key, json);
    }

    delete(key: string, session: boolean) {
        key = this._formatKey(key);
        this._getStorage(session).removeItem(key);
    }

    private _getStorage(session?: boolean) {
        return session ? sessionStorage : localStorage;
    }

    private _formatKey(key: string) {
        return this._baseKey + key;
    }

}
