import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { MessageBoxService } from '../services/common/message-box.service';
import { OperatorDocumentModalComponent } from '../operator-document-modal/operator-document-modal.component';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { OperatorsService } from '../services/operators.service';
import { OperatorDocumentModel } from '../shared/models/operator-document.model';

@Component({
  selector: 'app-operator-documents-modal',
  templateUrl: './operator-documents-modal.component.html',
  styleUrls: ['./operator-documents-modal.component.scss']
})

export class OperatorDocumentsModalComponent extends ModalComponent<any> {

  @ViewChild('operatorDocumentModal', { static: true }) operatorDocumentModal: OperatorDocumentModalComponent;
  public operatorId: number = null;

  documents: Array<OperatorDocumentModel> = [];

  constructor(private readonly _messageBox: MessageBoxService, private readonly _operatorsService: OperatorsService) {
    super();
  }

  aggiungiDocumento() {
      const request = new OperatorDocumentModel();
      request.operatorId = this.operatorId;
      this._subscriptions.push(
          this.operatorDocumentModal.open(request)
              .pipe(
                  filter(e => e),
                  switchMap(() => this._operatorsService.createDocument(request)),
                  tap(e => {
                    this._messageBox.success(`Documento aggiunto`);
                  }),
                  tap(() => {
                    this.loadData();
                  })
              )
              .subscribe()
      );
  }

  modificaDocumento(document: OperatorDocumentModel) {
      this._subscriptions.push(
        this._operatorsService.getOperatorDocument(document.id)
          .pipe(
              map(e => {
                return e;
              }),
              switchMap(e => this.operatorDocumentModal.open(e)),
              filter(e => e),
              map(() => this.operatorDocumentModal.options),
              switchMap(e => this._operatorsService.updateDocument(e, e.id)),
              map(() => this.operatorDocumentModal.options),
              tap(e => this._messageBox.success(`Documento aggiornato`)),
              tap(() => this.loadData())
          )
        .subscribe()
      );
  }

  scaricaDocumento(certificato: OperatorDocumentModel) {
    var link = document.createElement("a");
    link.target = '_blank';
    link.href = certificato.fileName;
    link.click();
  }

  protected _readDocuments() {
      this._subscriptions.push(
        this._operatorsService.getOperatorDetail(this.operatorId)
          .pipe(
              tap(e => {
                this.documents = e.documents;
              })
          )
          .subscribe()
      );
  }

  public loadData() {
    this._readDocuments();
  }

  protected _canClose() {
      return true;
  }
}
