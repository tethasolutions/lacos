import { Component, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { JobAccountingsService } from '../services/jobs/job-accountings.service';
import { JobAccountingModalComponent } from './jobaccounting-modal.component';
import { JobAccountingModel } from '../services/jobs/job-accounting.model';
import { Workbook } from '@progress/kendo-angular-excel-export';
import { saveAs } from '@progress/kendo-file-saver';

@Component({
  selector: 'app-jobaccountings',
  templateUrl: './jobaccountings.component.html'
})

export class JobAccountingsComponent extends BaseComponent implements OnInit {
  dataJobAccountings: GridDataResult;
  stateGridJobAccountings: State = {
    skip: 0,
    take: 30,
    filter: {
      filters: [
        this._buildJobIdFilter()
      ],
      logic: 'and'
    },
    group: [],
    sort: [{ field: 'jobCode', dir: 'asc' }, { field: 'accountingTypeName', dir: 'asc' }]
  };

  _jobId: number;
  screenWidth: number;

  @Input() viewExportExcel: boolean = true;
  @Input() jobId: number = null;
  @ViewChild('jobAccountingModal', { static: true }) jobAccountingModal: JobAccountingModalComponent;

  constructor(
    private readonly _jobAccountingsService: JobAccountingsService,
    private readonly _messageBox: MessageBoxService,
    private readonly _route: ActivatedRoute
  ) {
    super();
  }

  ngOnInit() {
    this.updateScreenSize();
    this._subscribeRouteParams();
  }

  private _subscribeRouteParams() {
    this._route.queryParams
      .pipe(
        tap(e => {
          this._setParams(e);
          this._read();
        })
      )
      .subscribe();
  }

  private _setParams(params: Params) {
    this._jobId = isNaN(+params['jobId']) ? null : +params['jobId'];
    if (!this._jobId && this.jobId) { this._jobId = this.jobId; }
    this._read();
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.updateScreenSize();
  }

  private updateScreenSize(): void {
    this.screenWidth = window.innerWidth - 44;
    if (this.screenWidth > 1876) this.screenWidth = 1876;
    if (this.screenWidth < 1400) this.screenWidth = 1400;
  }


  dataStateChange(state: State) {
    this.stateGridJobAccountings = state;
    this._read();
  }

  protected _read() {
    this._subscriptions.push(
      this._jobAccountingsService.readJobAccountings(this.stateGridJobAccountings)
        .pipe(
          tap(e => {
            this.dataJobAccountings = e;
          })
        )
        .subscribe()
    );
  }

  createJobAccounting() {
    const request = new JobAccountingModel(0, this._jobId, null, null, null, false, []);
    this._subscriptions.push(
      this.jobAccountingModal.open(request)
        .pipe(
          filter(e => e),
          switchMap(() => this._jobAccountingsService.createJobAccounting(request)),
          tap(e => {
            this._messageBox.success(`Voce creata`);
          }),
          tap(() => this._read())
        )
        .subscribe()
    );
  }

  editJobAccounting(jobAccounting: JobAccountingModel) {
    this._subscriptions.push(
      this._jobAccountingsService.getJobAccountingDetail(jobAccounting.id)
        .pipe(
          map(e => {
            return Object.assign(new JobAccountingModel(e.id, e.jobId, e.accountingTypeId, e.amount, e.note, e.isPaid, e.targetOperators), e);
          }),
          switchMap(e => this.jobAccountingModal.open(e)),
          filter(e => e),
          map(() => this.jobAccountingModal.options),
          switchMap(e => this._jobAccountingsService.updateJobAccounting(e, e.id)),
          map(() => this.jobAccountingModal.options),
          tap(e => this._messageBox.success(`Voce aggiornata`)),
          tap(() => this._read())
        )
        .subscribe()
    );
  }

  deleteJobAccounting(jobAccounting: JobAccountingModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare la voce selezionata?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._jobAccountingsService.deleteJobAccounting(jobAccounting.id)
            .pipe(
              tap(e => this._messageBox.success(`Voce ${jobAccounting.note} cancellata con successo`)),
              tap(() => this._read())
            )
            .subscribe()
        );
      }
    });
  }

  private _buildJobIdFilter() {
    const that = this;

    return {
      field: 'jobId',
      get operator() {
        return that._jobId
          ? 'eq'
          : 'isnotnull'
      },
      get value() {
        return that._jobId;
      }
    };
  }

  exportToExcel(): void {
    const options = this.getExportOptions();
    const workbook = new Workbook(options);
    workbook.toDataURL().then((dataURL) => {
      saveAs(dataURL, 'voci_contabili.xlsx');
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
          { autoWidth: true }
        ],
        title: 'Voci Contabili',
        rows: [
          {
            cells: [
              { value: 'Cliente', bold: true },
              { value: 'Commessa', bold: true },
              { value: 'Riferimento', bold: true },
              { value: 'Tipo', bold: true },
              { value: 'Importo', bold: true },
              { value: 'Note', bold: true },
              { value: 'Pagato', bold: true }
            ]
          },
          ...this.dataJobAccountings.data.map((item: any) => ({
            cells: [
              { value: item.customer },
              { value: item.jobCode },
              { value: item.jobReference },
              { value: item.accountingTypeName },
              { value: item.amount },
              { value: item.note },
              { value: item.isPaid ? 'Si' : 'No' }
            ]
          }))
        ]
      }]
    };
  }
}
