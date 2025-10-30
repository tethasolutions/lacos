import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EMPTY, expand, map, reduce, tap } from 'rxjs';
import { ApiUrls } from '../common/api-urls';

@Injectable({ providedIn: 'root' })
export class SharepointService {

    public tenantUrl = "https://graph.microsoft.com/v1.0";
    public siteId = "0fdfacc1-fe8c-4646-94d0-47e99014b0e2";
    public driveId = "daa8fa9e-0baa-4edb-9bbe-31add2837fd4";
    public rootItemId = "01M6PTOZIUWLYL7KVEUJCLFX2ONYCRAXY2";

    get graphApi() {
        return {
            url: this._apiUrl,
            accessToken: this._apiAccessToken
        };
    }

    private _apiUrl = `${this.tenantUrl}/sites/lacosgroup.sharepoint.com,${this.siteId},${this.driveId}/drive/items`;
    private _apiAccessToken = "";

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/sharepoint`;
    private readonly _tokenEndpoint = `${this._baseUrl}/token`;

    private _headers = new HttpHeaders({
        Authorization: `Bearer ${this._apiAccessToken}`,
        Accept: "application/json"
    });

    constructor(
        private _http: HttpClient
    ) { }

    getChildren(itemId: string = this.rootItemId) {
        // richiedo un numero elevato di elementi per pagina (max 999 per Graph)
        const url = `${this._apiUrl}/${itemId}/children?$top=999`;

        return this._http.get<any>(url, { headers: this._headers }).pipe(
            // espandi le richieste finché esiste nextLink
            expand(response => {
                const next = response['@odata.nextLink'];
                return next ? this._http.get<any>(next, { headers: this._headers }) : EMPTY;
            }),
            // estrae solo la proprietà value di ogni pagina
            map(response => response.value as IItem[]),
            // concatena tutte le pagine in un unico array
            reduce((acc, value) => acc.concat(value), [] as IItem[]),
            // converte IItem in SharepointFolder o SharepointFile
            map(items => items.map(item => {
                return item.folder
                    ? new SharepointFolder(item.name, item.id, item.parentReference.path, item.webUrl ?? '')
                    : new SharepointFile(item.name, item.id, item.parentReference.path, item.webUrl ?? '');
            }))
        );
    }

    getParentFolderPath(itemId: string) {
        const url = `${this._apiUrl}/${itemId}`;
        return this._http.get<IItem>(url, { headers: this._headers })
            .pipe(
                map(item => item.parentReference.path)
            );
    }

    getFileContentUrl(itemId: string) {
        const url = `${this._apiUrl}/${itemId}/content`;
        return this._http.get(url, { headers: this._headers, responseType: 'blob' })
            .pipe(
                map(blob => {
                    const fileUrl = URL.createObjectURL(blob);
                    return fileUrl;
                })
            );
    }

    downloadFile(itemId: string, fileName: string) {
        return this.getFileContentUrl(itemId)
            .pipe(
                tap(url => {
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = fileName;
                    a.click();
                    window.URL.revokeObjectURL(url);
                })
            );
    }

    updateSharepointApiAccessToken() {
        return this._http.get<SharepointResult>(this._tokenEndpoint)
            .pipe(
                map(response => response.accessToken),
                tap(token => {
                    this._apiAccessToken = token;
                    this._headers = this._headers.set('Authorization', `Bearer ${token}`);
                })
            );
    }

}

interface SharepointResult {
    readonly accessToken: string;
}

interface GraphResponse<T> {
    value: Array<T>;
}

interface IItem {
    id: string;
    name: string;
    parentReference: {
        path: string;
    };
    file?: object;
    folder?: object;
    webUrl?: string;
}

export class SharepointFolder {

    constructor(
        public name: string,
        public id: string,
        public path: string,
        public webUrl: string
    ) { }

}

export class SharepointFile {

    public isImg: boolean;

    constructor(
        public name: string,
        public id: string,
        public path: string,
        public webUrl: string
    ) {
        this.isImg = this._isImg(this.name);
    }

    private _isImg(file: string) {
        const imgExtensions = ['apng', 'avif', 'gif', 'jpg', 'jpeg', 'png', 'svg', 'webp'];
        const fileExtension = file.split('.').pop();

        return imgExtensions.includes(fileExtension?.toLowerCase() || "");
    }

}

export class SharepointItem {

    readonly isFolder: boolean;
    readonly isImg: boolean;
    readonly name: string;
    readonly id: string;
    readonly path: string;
    readonly webUrl: string;

    constructor(
        item: SharepointFile | SharepointFolder
    ) {
        this.name = item.name;
        this.id = item.id;
        this.path = item.path;
        this.webUrl = item.webUrl;
        this.isImg = item instanceof SharepointFile && item.isImg;
        this.isFolder = item instanceof SharepointFolder;
    }

}
