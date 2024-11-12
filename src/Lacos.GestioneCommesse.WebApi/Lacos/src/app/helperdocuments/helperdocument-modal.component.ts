import { Component, ViewChild, Input, OnInit } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { HelperDocumentModel, HelperTypeModel } from '../services/helper/models';
import { State } from '@progress/kendo-data-query';
import { HelperTypesService } from '../services/helper/helperTypes.service';
import { tap } from 'rxjs';
import { ApiUrls } from '../services/common/api-urls';
import { SuccessEvent } from '@progress/kendo-angular-upload';
import { UploadFileModel } from '../shared/models/upload-file.model';

@Component({
  selector: 'app-helperDocument-modal',
  templateUrl: './helperdocument-modal.component.html'
})

export class HelperDocumentModalComponent extends ModalFormComponent<HelperDocumentModel> implements OnInit {

  readonly role = Role;
  helperTypes: HelperTypeModel[];
  private readonly _baseUrl = `${ApiUrls.baseApiUrl}/helperDocuments`;
  uploadSaveUrl = `${this._baseUrl}/document/upload-file`;
  uploadRemoveUrl = `${this._baseUrl}/document/remove-file`;
  pathDocument = `${ApiUrls.baseUrl}/attachments/`;
  attachmentsFileInfo: any;
  attachmentsUploads: Array<UploadFileModel> = [];
  imageLabel: string;
  isUploaded: Array<boolean> = [];

  constructor(
    private readonly _helperTypesService: HelperTypesService,
    messageBox: MessageBoxService
  ) {
    super(messageBox);
  }

  ngOnInit() {
    this._getHelperTypes();
  }

  protected _canClose(): boolean {
    markAsDirty(this.form);

    if (this.form.invalid) {
      this._messageBox.error('Compilare correttamente tutti i campi');
    }

    return this.form.valid ?? false;
  }

  private _getHelperTypes() {
    const state: State = {
      sort: [
        { field: 'type', dir: 'asc' }
      ]
    };

    this._subscriptions.push(
      this._helperTypesService.read(state)
        .pipe(
          tap(e => this.helperTypes = e.data as HelperTypeModel[])
        )
        .subscribe()
    )
  }
  
  public ImageExecutionSuccess(e: SuccessEvent): void {
    const body = e.response.body;
    if (body != null) {

        const uploadedFile = body as UploadFileModel;
        this.options.fileName = uploadedFile.fileName;
        this.isUploaded.push(true);
    }
    else {
        const deletedFile = e.files[0].name;
        const index = this.attachmentsUploads.findIndex(x => x.originalFileName == deletedFile);
        if (index > -1) {
            this.attachmentsUploads.splice(index, 1);
            this.isUploaded.pop();
        }
    }
}

public loadData() {
    this.attachmentsFileInfo = null;
    if (this.options != undefined) {
        if (this.options.fileName != null)
            this.imageLabel = "Cambia documento"
        else
            this.imageLabel = "Documento"
    }
    else
        this.imageLabel = "Documento"

}

}
