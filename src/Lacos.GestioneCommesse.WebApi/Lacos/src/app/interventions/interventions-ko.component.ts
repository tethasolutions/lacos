import { Component, EventEmitter, HostListener, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { CellClickEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { saveAs } from '@progress/kendo-file-saver';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { State } from '@progress/kendo-data-query';
import { InterventionsService } from '../services/interventions/interventions.service';
import { filter, switchMap, tap } from 'rxjs';
import { ApiUrls } from '../services/common/api-urls';
import { UserService } from '../services/security/user.service';
import { GalleryModalComponent, GalleryModalInput } from '../shared/gallery-modal.component';
import { MessageBoxService } from '../services/common/message-box.service';

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

    pathImage = `${ApiUrls.baseAttachmentsUrl}/`;
    album: string[] = [];
    screenWidth: number;

    constructor(
        private readonly _service: InterventionsService,
        private readonly _messageBox: MessageBoxService,
        private readonly _userService: UserService
    ) {
        super();
    }

    ngOnInit() {        
        this._read();
        this.updateScreenSize();
      }
    
      @HostListener('window:resize', ['$event'])
      onResize(event: Event): void {
        this.updateScreenSize();
      }
    
      private updateScreenSize(): void {
        this.screenWidth = window.innerWidth -44;
        if (this.screenWidth > 1876) this.screenWidth = 1876;
        if (this.screenWidth < 1400) this.screenWidth = 1400;     
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

    protected _read() {
        this._subscriptions.push(
            this._service.readInterventionsKo(this.gridState)
                .pipe(
                    tap(e => this.data = e)
                )
                .subscribe()
        );
    }
    
    openImage(fileName: string) {
        this.album = [];
        this.album.push(this.pathImage + fileName);
        const options = new GalleryModalInput(this.album, 0);
        this.galleryModal.open(options).subscribe();
    }
    
    exportToExcel(): void {
        const options = this.getExportOptions();
        const workbook = new Workbook(options);
        workbook.toDataURL().then((dataURL) => {
            saveAs(dataURL, 'interventi_ko.xlsx');
        });
    }

    private getExportOptions(): any {
        return {
            sheets: [{
                columns: [
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true }
                ],
                title: 'Interventi KO',
                rows: [
                    {
                        cells: [
                            { value: 'Data Interv.', bold: true },
                            { value: 'Commessa', bold: true },
                            { value: 'Cliente', bold: true },
                            { value: 'Nome Prod', bold: true },
                            { value: 'Prodotto', bold: true },
                            { value: 'Descrizione intervento', bold: true },
                            { value: 'Voce Checklist', bold: true },
                            { value: 'Note KO', bold: true },
                            { value: 'Azioni Correttive', bold: true }
                        ]
                    },
                    ...this.data.data.map((item: any) => ({
                        cells: [
                            { value: item.start, format: 'dd/MM/yyyy HH:mm' },
                            { value: item.jobCode },
                            { value: item.customer },
                            { value: item.productName },
                            { value: item.productDescription },
                            { value: item.interventionDescription },
                            { value: item.checklistItem },
                            { value: item.outcomeNotes },
                            { value: item.outcomeCorrectiveAction }
                        ]
                    }))
                ]
            }]
        };
    }
}
