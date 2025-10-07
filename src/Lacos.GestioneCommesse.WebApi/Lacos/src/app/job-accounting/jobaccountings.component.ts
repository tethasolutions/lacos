import { Component, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { GroupResult, State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { JobAccountingsService } from '../services/jobs/job-accountings.service';
import { JobAccountingModalComponent } from './jobaccounting-modal.component';
import { IJobAccountingReadModel, JobAccountingModel } from '../services/jobs/job-accounting.model';
import { ExcelExportComponent, Workbook, WorkbookOptions } from '@progress/kendo-angular-excel-export';
import { saveAs } from '@progress/kendo-file-saver';

type JobKey = string;

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
    sort: [{ field: 'jobCode', dir: 'asc' }, { field: 'accountingTypeOrder', dir: 'asc' }]
  };

  _jobId: number;
  screenWidth: number;

  @Input() viewExportExcel: boolean = true;
  @Input() jobId: number = null;
  @ViewChild('jobAccountingModal', { static: true }) jobAccountingModal: JobAccountingModalComponent;
  @ViewChild('exporter', { static: true }) private exporter!: ExcelExportComponent;

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

  /** Esporta ciò che è attualmente nel Grid (rispetta filtri/pagine/gruppi del Grid) */
  exportCurrentGrid(): void {
    const state: State = {
      skip: 0,
      filter: {
        filters: [
          this._buildJobIdFilter()
        ],
        logic: 'and'
      },
      sort: [{ field: 'jobCode', dir: 'asc' }, { field: 'accountingTypeOrder', dir: 'asc' }]
    };

    this._subscriptions.push(
      this._jobAccountingsService.readJobAccountings(state)
        .pipe(
          tap(e => {
            const data = e;
            const items = this.toItemsArrayFromGrid(data);
            this.exportCore(items, 'voci_contabili.xlsx');
          })
        )
        .subscribe()
    );

  }

  /** Esempio: esporta solo NON pagati dal dataset completo (ignora paging del Grid) */
  exportOnlyUnpaidAll(): void {
    const state: State = {
      skip: 0,
      filter: {
        filters: [
          this._buildJobIdFilter(),
          {
            field: 'isPaid',
            operator: 'eq',
            value: false
          }
        ],
        logic: 'and'
      },
      sort: [{ field: 'jobCode', dir: 'asc' }, { field: 'accountingTypeName', dir: 'asc' }]
    };

    this._subscriptions.push(
      this._jobAccountingsService.readJobAccountings(state)
        .pipe(
          tap(e => {
            const data = e;
            const items = this.toItemsArrayFromGrid(data);
            this.exportCore(items, 'voci_contabili_non_pagate.xlsx');
          })
        )
        .subscribe()
    );
  }

  private toItemsArrayFromGrid(grid: GridDataResult | null | undefined): IJobAccountingReadModel[] {
    if (!grid || !Array.isArray(grid.data)) return [];
    const result: IJobAccountingReadModel[] = [];

    const walk = (nodes: any[]): void => {
      for (const n of nodes) {
        if (this.isGroup(n)) {
          // n è un GroupResult (può essere annidato)
          walk(n.items);
        } else {
          // n è una riga "leaf" del Grid -> cast al modello
          result.push(n as IJobAccountingReadModel);
        }
      }
    };

    walk(grid.data as any[]);
    return result;
  }

  private isGroup(node: any): node is GroupResult {
    return node && Array.isArray(node.items);
  }

  private exportCore(source: readonly IJobAccountingReadModel[], filename: string): void {
    const { rows, typeCols } = this.buildPivot(source);
    const workbook = new Workbook(this.buildWorkbookOptions(rows, typeCols));
    workbook.toDataURL().then((dataURL) => {
      saveAs(dataURL, filename);
    });
  }
  
  private buildPivot(source: readonly IJobAccountingReadModel[]): {
    rows: Array<Record<string, string | number>>;
    typeCols: string[];
  } {
    type TypeMeta = { name: string; order: number; isNegative: boolean };

    // 1) Costruisco la mappa dei tipi: name -> { order, isNegative }
    const typesMap = new Map<string, TypeMeta>();
    for (const r of source) {
      const name = (r.accountingTypeName ?? '').trim();
      if (!name) continue;

      const order = Number.isFinite(r.accountingTypeOrder)
        ? (r.accountingTypeOrder as number)
        : Number.MAX_SAFE_INTEGER; // fallback se non impostato

      const isNegative = !!r.accountingTypeIsNegative;

      const existing = typesMap.get(name);
      if (!existing) {
        typesMap.set(name, { name, order, isNegative });
      } else {
        // preferisco l'ordine minimo e aggrego il flag negativo (OR logico)
        if (order < existing.order) existing.order = order;
        existing.isNegative = existing.isNegative || isNegative;
      }
    }

    // 2) Calcolo le colonne dinamiche ordinate per 'order', poi per nome
    const typeCols = Array
      .from(typesMap.values())
      .sort((a, b) => (a.order - b.order) || a.name.localeCompare(b.name))
      .map(m => m.name);

    // 3) Group by chiavi (rimango fedele al tuo codice attuale: customer, jobCode, jobReference)
    //    Se vuoi includere anche jobId: aggiungilo nella chiave e nelle staticCols.
    const map = new Map<JobKey, {
      customer: string;
      jobCode: string;
      jobReference: string;
      amounts: Record<string, number>;
    }>();

    for (const r of source) {
      const key = `${r.customer}||${r.jobCode}||${r.jobReference}`;
      if (!map.has(key)) {
        const amounts: Record<string, number> = {};
        for (const t of typeCols) amounts[t] = 0;
        map.set(key, {
          customer: r.customer,
          jobCode: r.jobCode,
          jobReference: r.jobReference,
          amounts
        });
      }
      const entry = map.get(key)!;
      const col = (r.accountingTypeName ?? '').trim();
      if (col) {
        entry.amounts[col] = (entry.amounts[col] ?? 0) + (Number.isFinite(r.amount) ? r.amount : 0);
      }
    }

    // 4) Materializzo le righe + calcolo Totale con segno per tipo
    const rows = Array.from(map.values()).map(entry => {
      const obj: Record<string, string | number> = {
        customer: entry.customer,
        jobCode: entry.jobCode,
        jobReference: entry.jobReference
      };

      // valori per colonna tipo (sempre somma "positiva"/grezza)
      for (const t of typeCols) obj[t] = entry.amounts[t] ?? 0;

      // Totale: sommo o sottraggo in base al flag del tipo
      const total = typeCols.reduce((acc, t) => {
        const amount = Number(obj[t]) || 0;
        const neg = typesMap.get(t)?.isNegative ?? false;
        return acc + (neg ? -amount : amount);
      }, 0);

      obj['Total'] = total;
      return obj;
    });

    return { rows, typeCols };
  }


  private buildWorkbookOptions(
    rows: Array<Record<string, string | number>>,
    typeCols: string[]
  ): WorkbookOptions {
    const staticCols: Array<{ key: string; label: string }> = [
      { key: 'customer', label: 'Cliente' },
      { key: 'jobCode', label: 'Commessa' },
      { key: 'jobReference', label: 'Riferimento' }
    ];

    const dynamicCols = typeCols.map(t => ({ key: t, label: t }));
    const totalCol = { key: 'Total', label: 'Totale' };
    const colsMeta = [...staticCols, ...dynamicCols, totalCol];

    const columns = colsMeta.map(c => ({
      width: ['jobReference', 'customer'].includes(c.key) ? 300
        : c.key === 'jobCode' ? 160
          : 120
    }));

    const headerRow = {
      cells: colsMeta.map(c => ({
        value: c.label,
        background: '#F3F3F3',
        bold: true
      }))
    };

    // Righe dati: leggi i VALORI usando le KEY originali
    const dataRows = rows.map(r => ({
      cells: colsMeta.map(c => {
        const v = r[c.key];
        const isNumber = typeof v === 'number';
        return isNumber
          ? { value: v, format: '#,##0.00' }
          : { value: v ?? '' };
      })
    }));

    return {
      sheets: [
        {
          name: 'Voci Contabili',
          columns,
          rows: [headerRow, ...dataRows],
          frozenRows: 1
        }
      ]
    };
  }
}
