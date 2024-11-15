import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, tap } from 'rxjs';
import { ApiUrls } from '../common/api-urls';

@Injectable({ providedIn: 'root' })
export class SharepointService {

    public tenantUrl = "https://tetha365.sharepoint.com";

    public rootPath = "RootTest";

    get sharepointApi() {
        return {
            url: this._apiUrl,
            accessToken: this._apiAccessToken
        }
    }

    private _apiUrl = `${this.tenantUrl}/_api/web`;
    private _apiAccessToken = "";

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/sharepoint`;
    private readonly _tokenEndpoint = `${this._baseUrl}/token`;

    private _headers = new HttpHeaders({
        Authorization: `Bearer ${this._apiAccessToken}`,
        Accept: "application/json;odata=verbose"
    });

    constructor(
        private _http: HttpClient
    ) { }

    getFolders(path: string) {
        return this._http.get<SharepointResponse<IFolder>>(`${this._apiUrl}/GetFolderByServerRelativeUrl('${path}')/Folders`, { headers: this._headers })
            .pipe(
                map(e => e.d.results),
                map(e => e.map(e => new SharepointFolder(e.Name, e.ServerRelativeUrl)))
            );
    }

    getFiles(path: string) {
        return this._http.get<SharepointResponse<IFile>>(`${this._apiUrl}/GetFolderByServerRelativeUrl('${path}')/Files`, { headers: this._headers })
            .pipe(
                map(e => e.d.results),
                map(e => e.map(e => new SharepointFile(e.Name, e.ServerRelativeUrl)))
            )
    }

    getParentFolderPath(path: string) {
        return this._http.get<SharepointResponse<string>>(`${this._apiUrl}/GetFolderByServerRelativeUrl('${path}')/ParentFolder`, { headers: this._headers })
            .pipe(
                map(e => e.d.ServerRelativeUrl)
            );
    }

    getFileContentUrl(path: string) {
        return this._http.get(`${this._apiUrl}/GetFileByServerRelativeUrl('${path}')/$value`, { headers: this._headers, responseType: 'arraybuffer' })
            .pipe(
                map(e => {
                    const blob = new Blob([e], { type: "file/download" });
                    const url = URL.createObjectURL(blob);

                    return url;
                })
            )
    }

    downloadFile(path: string) {
        return this.getFileContentUrl(path)
            .pipe(
                tap(url => {
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = path.split('/').reverse()[0];
                    a.click();
                    window.URL.revokeObjectURL(url);
                })
            );
    }

    updateSharepointApiAccessToken() {
        return this._http.get<SharepointResult>(this._tokenEndpoint)
            .pipe(
                map(e => e.accessToken),
                tap(e => this._apiAccessToken = e)
            )
    }

}

interface SharepointResult {
    readonly accessToken: string;
}

interface SharepointResponse<T> {
    d: {
        results: Array<T>;
        ServerRelativeUrl: string;
    }
}

interface IFolder {
    Name: string;
    ServerRelativeUrl: string;
}

interface IFile {
    Name: string;
    ServerRelativeUrl: string;
}

export class SharepointFolder {

    constructor(
        public name: string,
        public path: string
    ) { }

}

export class SharepointFile {

    public isImg: boolean;

    constructor(
        public name: string,
        public path: string
    ) {
        this.isImg = this._isImg(this.name)
    }

    private _isImg(file: string) {
        const imgExtensions = ['apng', 'avif', 'gif', 'jpg', 'jpeg', 'png', 'svg', 'webp'];
        const fileExtension = file.split('.').last();

        if (imgExtensions.includes(fileExtension.toLowerCase())) {
            return true;
        }
        return false;
    }

}

export class SharepointItem {

    readonly isFolder: boolean;
    readonly isImg: boolean;
    readonly name: string;
    readonly path: string;

    constructor(
        item: SharepointFile | SharepointFolder
    ) {
        this.name = item.name;
        this.path = item.path;
        this.isImg = item instanceof SharepointFile && item.isImg;
        this.isFolder = item instanceof SharepointFolder;
    }

}