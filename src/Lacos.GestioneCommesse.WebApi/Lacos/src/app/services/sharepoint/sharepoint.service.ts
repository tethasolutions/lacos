import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of, tap, zip } from 'rxjs';

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
    private _apiAccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IktRMnRBY3JFN2xCYVZWR0JtYzVGb2JnZEpvNCIsImtpZCI6IktRMnRBY3JFN2xCYVZWR0JtYzVGb2JnZEpvNCJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvdGV0aGEzNjUuc2hhcmVwb2ludC5jb21AMTM2ZjU3OWEtMWJlMi00NGFiLWE2ZGUtM2QxOWFmNjZkNjAzIiwiaXNzIjoiMDAwMDAwMDEtMDAwMC0wMDAwLWMwMDAtMDAwMDAwMDAwMDAwQDEzNmY1NzlhLTFiZTItNDRhYi1hNmRlLTNkMTlhZjY2ZDYwMyIsImlhdCI6MTcyMzQ0Nzc2NiwibmJmIjoxNzIzNDQ3NzY2LCJleHAiOjE3MjM1MzQ0NjYsImlkZW50aXR5cHJvdmlkZXIiOiIwMDAwMDAwMS0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDBAMTM2ZjU3OWEtMWJlMi00NGFiLWE2ZGUtM2QxOWFmNjZkNjAzIiwibmFtZWlkIjoiYWFiYjEyODAtNjdlMC00NDk1LWFiMWMtNjVmZmQ5NmU4OTFmQDEzNmY1NzlhLTFiZTItNDRhYi1hNmRlLTNkMTlhZjY2ZDYwMyIsIm9pZCI6ImRmOWEzYzY3LWY5MDItNDNkYS1iODE2LWNmOTNmNmZjODE2OSIsInN1YiI6ImRmOWEzYzY3LWY5MDItNDNkYS1iODE2LWNmOTNmNmZjODE2OSIsInRydXN0ZWRmb3JkZWxlZ2F0aW9uIjoiZmFsc2UifQ.O9C9wGu5msm3oB1hLbn2GKeQyF4JUUaX-F1g9GfmvbSevg9qgm-e7ttmULjHP9ZdaCU2iG5SDWGPPB0DRMVoAj2nlneeSaYh4U1S56X65WZ7q5Nl753XPP10eK6f7xGinskhs3ddLTyekQasflAkIKMNUMip5OGhfmCsx-iOcnGoNbAVZ0ygurKliXS6r3cK_h3HD_bPI8whtIhhcT39FElM_7Y0iGnN7IC_vOc79qVzG0m9l0qWTlYWTHewOEZu4B084QJxqB-KWCMH_OpMuqvmGSARR_LvWGpSLUbBjPaDgXB4t3KjCAhzm_cydK-GQKImBE0VjsNxE04ZPJzQqQ";

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

    downloadFile(path: string) {
        return this._http.get(`${this._apiUrl}/GetFileByServerRelativeUrl('${path}')/$value`, { headers: this._headers, responseType: 'arraybuffer' })
            .pipe(
                tap(e => {
                    const blob = new Blob([e], { type: "file/download" });
                    const url = URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = path.split('/').reverse()[0];
                    a.click();
                    window.URL.revokeObjectURL(url);
                })
            )
    }

}

declare type SharepointResponse<T> = {
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