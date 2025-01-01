import { Component, Input, OnDestroy, OnInit } from '@angular/core';
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

    public readonly rootPath = this._sharepointService.rootItemId;
    public currentPath = this.rootPath;

    public browseMode: boolean;
    public navFolder: string;

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

    resetFolder() {
        this._navigate(this.rootPath,"COMMESSE");
    }

    onCellClick($event: CellClickEvent) {
        const dataItem = $event.dataItem as SharepointItem;

        if (dataItem.isFolder) {
            this._navigate(dataItem.id, dataItem.name);
        } else {
            this._downloadFile(dataItem.id, dataItem.name);
        }
    }

    override open(options: ISharepointModalOptions) {
        this.currentPath = options.path;
        this.browseMode = options.browseMode;
        this.navigationMenuItems = [];

        this._sharepointService.updateSharepointApiAccessToken()
            .pipe(
                takeUntil(this._ngUnsubscribe),
                tap(() => this._navigate(this.currentPath,options.folderName))
            )
            .subscribe();

        return super.open(options);
    }

    protected override _canClose(): boolean {
        return true;
    }

    private _navigate(itemId: string, folderName: string) {
        this._getFolderContents(itemId);
        this._sharepointService.getParentFolderPath(itemId)
            .pipe(
                takeUntil(this._ngUnsubscribe),
                tap(path => {
                    this.navigationMenuItems = path.replace('/drive/root:','').concat('/',folderName).split('/').filter(e => e).map(item => ({ text: item, title: itemId }));
                    this.currentPath = itemId;
                    this.navFolder = path;
                })
            )
            .subscribe();
    }

    private _downloadFile(itemId: string, fileName: string) {
        this._sharepointService.downloadFile(itemId, fileName)
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
        this._sharepointService.getChildren(path)
            .pipe(
                takeUntil(this._ngUnsubscribe),
                tap(items => this.items = items.map(item => new SharepointItem(item)))
            )
            .subscribe();
    }

}

export interface ISharepointModalOptions {
    path: string;
    folderName: string;
    browseMode: boolean;
}
