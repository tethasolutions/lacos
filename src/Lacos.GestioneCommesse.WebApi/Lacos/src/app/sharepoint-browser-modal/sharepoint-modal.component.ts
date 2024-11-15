import { Component, Input, input, OnDestroy, OnInit } from '@angular/core';
import { SharepointFile, SharepointFolder, SharepointItem, SharepointService } from '../services/sharepoint/sharepoint.service';

import { catchError, map, of, Subject, takeUntil, tap, zip } from 'rxjs';
import { ModalComponent } from '../shared/modal.component';

import { BreadCrumbItem } from "@progress/kendo-angular-navigation";
import { CellClickEvent } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';

@Component({
    selector: 'app-sharepoint-modal',
    templateUrl: 'sharepoint-modal.component.html'
})

export class SharepointModalComponent extends ModalComponent<ISharepointModalOptions> implements OnInit, OnDestroy {

    @Input() startPath: string;

    public readonly tenantUrl = this._sharepointService.tenantUrl;

    public folders: SharepointFolder[] = [];
    public files: SharepointFile[] = [];
    public items: SharepointItem[];

    public readonly rootPath = this._sharepointService.rootPath;
    public currentPath = this.rootPath;

    public browseMode: boolean;

    public navigationMenuItems: BreadCrumbItem[];

    private readonly _ngUnsubscribe = new Subject<void>();

    constructor(
        private _sharepointService: SharepointService,
        private _messageBox: MessageBoxService
    ) {
        super();
    }

    ngOnInit() {

    }

    onPathClick(item: BreadCrumbItem) {
        const index = this.navigationMenuItems.indexOf(item);
        const path = this.navigationMenuItems
            .filter((_, i) => i <= index)
            .map(e => e.text)
            .join('/');

        if (!this.browseMode) {
            const pathFolder = path.startsWith('/') ? path : '/' + path;
            if (pathFolder.indexOf(this.startPath) == -1) return;
        }

        this._navigate(path);
    }

    onCellClick($event: CellClickEvent) {
        const dataItem = $event.dataItem as SharepointItem;

        if (dataItem.isFolder) {
            this._navigate(dataItem.path);
        } else {
            this._downloadFile(dataItem.path);
        }
    }

    override open(options: ISharepointModalOptions) {
        this.currentPath = options.path;
        this.browseMode = options.browseMode;
        this.navigationMenuItems = [];

        this._sharepointService.updateSharepointApiAccessToken()
            .pipe(
                takeUntil(this._ngUnsubscribe),
                tap(() => this._navigate(this.currentPath))
            )
            .subscribe();

        return super.open(options);
    }

    protected override _canClose(): boolean {
        return true;
    }

    private _navigate(path: string) {
        this._getFolderContents(path);
        this.currentPath = path;
        this.navigationMenuItems = this.currentPath.split('/').filter(e => e).map(item => ({ text: item, title: item }))
    }

    private _downloadFile(path: string) {
        this._sharepointService.downloadFile(path)
            .pipe(
                takeUntil(this._ngUnsubscribe),
                catchError(e => {
                    this._messageBox.error("Questo file non può essere scaricato");
                    return of(0);
                })
            )
            .subscribe();
    }

    private _getFolderContents(path: string) {
        zip(
            this._sharepointService.getFolders(path),
            this.browseMode ? of(new Array<SharepointFile>()) : this._sharepointService.getFiles(path)
        )
            .pipe(
                takeUntil(this._ngUnsubscribe),
                tap(e => this.items = e[0].orderBy(ee => ee.name).concat(e[1].orderBy(ee => ee.name)).map(ee => new SharepointItem(ee)))
            )
            .subscribe();
    }

}

export interface ISharepointModalOptions {
    path: string;
    browseMode: boolean;
}