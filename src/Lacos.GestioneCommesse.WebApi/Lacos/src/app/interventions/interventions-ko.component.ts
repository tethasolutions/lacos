import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild, input } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { CellClickEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { State } from '@progress/kendo-data-query';
import { InterventionsService } from '../services/interventions/interventions.service';
import { filter, switchMap, tap } from 'rxjs';
import { IInterventionReadModel, Intervention, interventionStatusNames } from '../services/interventions/models';
import { MessageBoxService } from '../services/common/message-box.service';
import { InterventionModalComponent } from './intervention-modal.component';
import { ApiUrls } from '../services/common/api-urls';
import { UserService } from '../services/security/user.service';
import { ActivatedRoute, Params } from '@angular/router';
import { GalleryModalComponent, GalleryModalInput } from '../shared/gallery-modal.component';

@Component({
    selector: 'app-interventions-ko',
    templateUrl: 'interventions-ko.component.html'
})
export class InterventionsKoComponent extends BaseComponent implements OnInit {

    @ViewChild('galleryModal', { static: true }) galleryModal: GalleryModalComponent;

    private cellArgs: CellClickEvent;
    data: GridDataResult;
    gridState: State = {
        skip: 0,
        take: 30,
        filter: {
            filters: [
            ],
            logic: 'and'
        },
        group: [],
        sort: [{ field: 'start', dir: 'desc' }, {field: 'jobCode', dir: 'desc'}]
    };

    album: string[] = [];
    
    constructor(
        private readonly _service: InterventionsService,
        private readonly _userService: UserService
    ) {
        super();
    }

    ngOnInit() {        
        this._read();
    }

    refresh() {
        this._read();
    }

    dataStateChange(state: State) {
        this.gridState = state;
        this._read();
    }

    cellClickHandler(args: CellClickEvent): void {
        this.cellArgs = args;
    }

    downloadReport(interventionId: number) {
        const user = this._userService.getUser();
        window.open(`${ApiUrls.baseApiUrl}/interventions/download-report/${interventionId}?access_token=${user.accessToken}`, "_blank");
    }

    protected _read() {
        this._subscriptions.push(
            this._service.readInterventionsKo(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }
    
    openImage(index: number) {
        const options = new GalleryModalInput(this.album, index);
        this.galleryModal.open(options).subscribe();
    }
}
