import { Component, OnDestroy, OnInit } from '@angular/core';
import { IFile, IFolder, ISharepointModalOptions, SharepointService } from '../services/sharepoint/sharepoint.service';

import { of, Subject, takeUntil, tap } from 'rxjs';
import { ModalComponent } from '../shared/modal.component';

@Component({
    selector: 'app-sharepoint-modal',
    templateUrl: 'sharepoint-modal.component.html'
})

export class SharepointModalComponent extends ModalComponent<ISharepointModalOptions> implements OnInit, OnDestroy {

    public tenantUrl = this._sharepointService.tenantUrl;

    public folders: IFolder[] = [];
    public files: IFile[] = [];

    public rootPath = this._sharepointService.rootPath;
    public currentPath = this.rootPath;

    public pathExist = true;

    public browseMode: boolean;

    private _ngUnsubscribe = new Subject<void>();

    constructor(
        private _sharepointService: SharepointService
    ) {
        super();
    }

    ngOnInit(): void {
        this.navigate(this.rootPath);
    }

    navigate(path: string) {
        this._sharepointService.getFolderContents(path)
            .pipe(
                takeUntil(this._ngUnsubscribe),
                tap(e => this.folders = e.folders),
                tap(e => this.files = e.files),
                tap(() => this.currentPath = path)
            )
            .subscribe();
    }

    downloadFile(path: string) {
        this._sharepointService.downloadFile(path)
            .subscribe();
    }

    isImg(file: IFile) {
        const imgExtensions = ['apng', 'avif', 'gif', 'jpg', 'jpeg', 'png', 'svg', 'webp'];
        const fileExtension = file.Name.split('.').reverse()[0];

        if (imgExtensions.includes(fileExtension.toLowerCase())) {
            return true;
        }
        return false;
    }

    back() {
        this._sharepointService.getParentFolderPath(this.currentPath)
            .pipe(
                takeUntil(this._ngUnsubscribe),
                tap(e => this.navigate(e))
            )
            .subscribe()
    }

    override open(options: ISharepointModalOptions) {
        this.currentPath = options.path;
        this.browseMode = options.browseMode;

        this.navigate(this.currentPath);

        return super.open(options);
    }

    protected override _canClose(): boolean {
        return true;
    }

}