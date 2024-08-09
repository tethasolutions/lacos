import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, tap, zip } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SharepointService {

    public tenantUrl = "https://tetha365.sharepoint.com";

    public rootPath = "/RootTest";

    get sharepointApi() {
        return {
            url: this._apiUrl,
            accessToken: this._apiAccessToken
        }
    }

    private _apiUrl = `${this.tenantUrl}/_api/web`;
    private _apiAccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IktRMnRBY3JFN2xCYVZWR0JtYzVGb2JnZEpvNCIsImtpZCI6IktRMnRBY3JFN2xCYVZWR0JtYzVGb2JnZEpvNCJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvdGV0aGEzNjUuc2hhcmVwb2ludC5jb21AMTM2ZjU3OWEtMWJlMi00NGFiLWE2ZGUtM2QxOWFmNjZkNjAzIiwiaXNzIjoiMDAwMDAwMDEtMDAwMC0wMDAwLWMwMDAtMDAwMDAwMDAwMDAwQDEzNmY1NzlhLTFiZTItNDRhYi1hNmRlLTNkMTlhZjY2ZDYwMyIsImlhdCI6MTcyMzE5NzcxMywibmJmIjoxNzIzMTk3NzEzLCJleHAiOjE3MjMyODQ0MTMsImlkZW50aXR5cHJvdmlkZXIiOiIwMDAwMDAwMS0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDBAMTM2ZjU3OWEtMWJlMi00NGFiLWE2ZGUtM2QxOWFmNjZkNjAzIiwibmFtZWlkIjoiYWFiYjEyODAtNjdlMC00NDk1LWFiMWMtNjVmZmQ5NmU4OTFmQDEzNmY1NzlhLTFiZTItNDRhYi1hNmRlLTNkMTlhZjY2ZDYwMyIsIm9pZCI6ImRmOWEzYzY3LWY5MDItNDNkYS1iODE2LWNmOTNmNmZjODE2OSIsInN1YiI6ImRmOWEzYzY3LWY5MDItNDNkYS1iODE2LWNmOTNmNmZjODE2OSIsInRydXN0ZWRmb3JkZWxlZ2F0aW9uIjoiZmFsc2UifQ.ncNGeynKwpZiKtBJCAFewCO9U6XD6gW4TRcVPBCK4d_J3_qf24DMN51u4m7KZxtlskpQSeXAPbwt4ueERZq2Keaz6jcFqHFXAgHm_s-TkJU5JtpCfdNFn0MrFRwA3JwHFyar_OLSZv38j8JeTLIloCmUPdkDBeUC-LbBpUOs810BTuIOv_Rq6G-ZhBimmk8ZRmY5ZCnfnzXb2bMdv6WW6QZrkFH5VRJGIop9lWvg4gZs2mpMl_Y68riCv_ZOfPv78Vlnh0uDQurVYDR-PmLOP-2Kh_Bh_QqKXVw4SbukIdogXwt1ALrnobUptmp8D58QmmJUBNU0cVGppLQ4smoGDw";

    private _headers = new HttpHeaders({
        Authorization: `Bearer ${this._apiAccessToken}`,
        Accept: "application/json;odata=verbose"
    });

    constructor(
        private _http: HttpClient
    ) { }

    getFolderContents(path: string) {
        const folders = this._http.get<SharepointResponse<IFolder>>(`${this._apiUrl}/GetFolderByServerRelativeUrl('${path}')/Folders`, { headers: this._headers })
            .pipe(
                map(e => e.d.results)
            );

        const files = this._http.get<SharepointResponse<IFile>>(`${this._apiUrl}/GetFolderByServerRelativeUrl('${path}')/Files`, { headers: this._headers })
            .pipe(
                map(e => e.d.results)
            );

        return zip(folders, files)
            .pipe(
                map(e => ({ folders: e[0], files: e[1] }))
            );
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

export interface IFolder {
    Name: string;
    ServerRelativeUrl: string;
}

export interface IFile {
    Name: string;
    ServerRelativeUrl: string;
}

export interface ISharepointModalOptions {
    path: string;
    browseMode: boolean;
}