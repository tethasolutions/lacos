import { Component, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { HelperDocumentModel, HelperTypeModel } from '../services/helper/models';
import { HelperDocumentModalComponent } from './helperdocument-modal.component';
import { HelperDocumentsService } from '../services/helper/helperDocuments.service';
import { SecurityService } from '../services/security/security.service';
import { Role } from '../services/security/models';
import { ApiUrls } from '../services/common/api-urls';

@Component({
  selector: 'app-helperdocuments',
  templateUrl: './helperdocuments.component.html'
})

export class HelperDocumentsComponent extends BaseComponent implements OnInit {
  dataHelperDocuments: GridDataResult;
  stateGridHelperDocuments: State = {
    skip: 0,
    take: 30,
    filter: {
      filters: [],
      logic: 'and'
    },
    group: [],
    sort: [
      { field: 'helperTypeName', dir: 'asc' }
    ]
  };

  @ViewChild('helperDocumentModal', { static: true }) helperDocumentModal: HelperDocumentModalComponent;

  screenWidth: number;
  isAdmin: boolean = false;
  pathDocument = `${ApiUrls.baseUrl}/attachments/`;

  constructor(
    private security: SecurityService,
    private readonly _service: HelperDocumentsService,
    private readonly _messageBox: MessageBoxService,
  ) {
    super();
  }

  ngOnInit() {
    this._readHelperDocuments();
    this.updateScreenSize();
    this.isAdmin = this.security.isAuthorized(Role.Administrator);
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
    this.stateGridHelperDocuments = state;
    this._readHelperDocuments();
  }

  protected _readHelperDocuments() {
    this._subscriptions.push(
      this._service.read(this.stateGridHelperDocuments)
        .pipe(
          tap(e => {
            this.dataHelperDocuments = e;
          })
        )
        .subscribe()
    );
  }

  createHelperDocument() {
    const helperDocument = new HelperDocumentModel();

    this._subscriptions.push(
      this.helperDocumentModal.open(helperDocument)
        .pipe(
          filter(e => e),
          switchMap(() => this._service.create(helperDocument)),
          tap(e => {
            this._messageBox.success(`Guida creata`);
          }),
          tap(() => this._readHelperDocuments())
        )
        .subscribe()
    );
  }

  editHelperDocument(helperDocument: HelperDocumentModel) {
    this._subscriptions.push(
      this._service.get(helperDocument.id)
        .pipe(
          map(e => {
            return Object.assign(new HelperDocumentModel(), e);
          }),
          switchMap(e => this.helperDocumentModal.open(e)),
          filter(e => e),
          map(() => this.helperDocumentModal.options),
          switchMap(e => this._service.update(e)),
          tap(e => this._messageBox.success(`Guida aggiornata`)),
          tap(() => this._readHelperDocuments())
        )
        .subscribe()
    );
  }

  deleteHelperDocument(helperDocument: HelperDocumentModel) {
    this._messageBox.info('Eliminazione elemento non attiva');
    this._messageBox.confirm(`Sei sicuro di voler cancellare la guida selezionata?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._service.delete(helperDocument.id)
            .pipe(
              tap(e => this._messageBox.success(`Guida cancellato con successo`)),
              tap(() => this._readHelperDocuments())
            )
            .subscribe()
        );
      }
    });
  }

}
