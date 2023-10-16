import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent } from '../shared/modal.component';
import { AddressModel } from '../shared/models/address.model';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { OperatorModel } from '../shared/models/operator.model';
import { VehicleModel } from '../shared/models/vehicle.model';
import { VehiclesService } from '../services/vehicles.service';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { SimpleLookupModel } from '../shared/models/simple-lookup.model';
import { OperatorDocumentsModalComponent } from '../operator-documents-modal/operator-documents-modal.component';
import { ApiUrls } from '../services/common/api-urls';
import { RemoveEvent, SuccessEvent, FileInfo,FileState,SelectEvent} from "@progress/kendo-angular-upload";
import { UploadFileModel } from '../shared/models/upload-file.model';
import { Observable } from 'rxjs';
import { OperatorDocumentModel } from '../shared/models/operator-document.model';
import { IActivityTypeOperator } from '../shared/models/activity-type.model';

@Component({
  selector: 'app-operator-modal',
  templateUrl: './operator-modal.component.html',
  styleUrls: ['./operator-modal.component.scss']
})

export class OperatorModalComponent extends ModalComponent<OperatorModel> {

  @ViewChild('form') form: NgForm;
  @ViewChild('operatorDocumentsModal', { static: true }) operatorDocumentsModal: OperatorDocumentsModalComponent;

  private readonly _baseUrl = `${ApiUrls.baseApiUrl}/operators`;
  uploadSaveUrl = `${this._baseUrl}/document/upload-file`;
  uploadRemoveUrl = `${this._baseUrl}/document/remove-file`; 

  attachmentsFileInfo:Array<FileInfo>= [];
  attachmentsUploads: Array<UploadFileModel> =[];
  isUploaded:Array<boolean>= [];

  vehicles: Array<VehicleModel> = [];
  roles: Array<SimpleLookupModel> = [];
  activityTypes: SelectableActivityType[];

  notHasUser:boolean;

  readonly role = Role;

  @Input() operator = new OperatorModel();

  constructor(
      private readonly _messageBox: MessageBoxService,
      private readonly _vehiclesService: VehiclesService
  ) {
      super();
  }

  override open(options: OperatorModel): Observable<boolean> 
  {
    const result = super.open(options);
    this.attachmentsFileInfo = [];
    this.isUploaded = [];
    this.attachmentsUploads = [];
      
    this.notHasUser = !this.options.hasUser;

    this.options.documents.forEach(element => {
      if(element.originalFileName !=null && element.fileName != null)
      {
        const noteAttachment = new UploadFileModel(element.fileName,element.originalFileName);
        this.attachmentsUploads.push(noteAttachment);
        this.attachmentsFileInfo.push({name: element.originalFileName});  
        this.isUploaded.push(true);
      }
    });    
    return result;
  }

  public AttachmentExecutionSuccess(e: SuccessEvent): void
  {
    const body = e.response.body;
    if(body != null)
    {

      const uploadedFile = body as UploadFileModel;
      const operatorAttachment = new UploadFileModel(uploadedFile.fileName,uploadedFile.originalFileName);
      this.attachmentsUploads.push(operatorAttachment);        
      let operatorAttachmentModal = new OperatorDocumentModel();
      operatorAttachmentModal.fileName = uploadedFile.fileName;
      operatorAttachmentModal.originalFileName = uploadedFile.originalFileName;
      this.options.documents.push(operatorAttachmentModal);        
      this.isUploaded.push(true);
    }
    else
    {
      const deletedFile = e.files[0].name;
      const index = this.attachmentsUploads.findIndex(x=>x.originalFileName == deletedFile);
      if(index>-1)
      {
      this.attachmentsUploads.splice(index,1);
      this.options.documents.splice(index,1);        
      this.isUploaded.pop();
      }
    }
  }

  public AttachmentSelect(e: SelectEvent): void
  {
    const files = e.files;
    let popup = false;
    files.forEach(element => {
      var index = this.attachmentsUploads.findIndex(x=>x.originalFileName == element.name);
      if(index > -1)
      {
        files.splice(index,1);
      popup = true;
      }
    });     
    if(popup)
    {
      this._messageBox.alert(`Sono presenti tra i file caricati alcuni file con lo stesso nome di quelli che si vogliono caricare`);
    }
  }

  public CreateUrl(fileName:string) : string
  {
    let ret = "";
    this.attachmentsUploads.forEach(element => {
      if(element.originalFileName == fileName)
      ret = `${this._baseUrl}/document/download-file/${element.fileName}/${element.originalFileName}`;
     });       
     return ret;
  }

  protected _canClose() {
      markAsDirty(this.form);

      if (this.form.invalid) {
          this._messageBox.error('Compilare correttamente tutti i campi');
      }

      return this.form.valid;
  }

  protected _readVehicles() {
    this._subscriptions.push(
      this._vehiclesService.readVehiclesList()
        .pipe(
            tap(e => {
              this.vehicles = e;
            })
        )
        .subscribe()
    );
  }

  setRoles() {
    this.roles = [];
    for(var n in Role) {
        if (typeof Role[n] === 'number') {
          this.roles.push({id: <any>Role[n], name: n});
        }
    }
  }

  viewDocuments() {
    this.operatorDocumentsModal.operatorId = this.options.id;
    this.operatorDocumentsModal.loadData();
    this.operatorDocumentsModal.open(null);
  }


  public loadData() {
    this._readVehicles();
    this.setRoles();
  }

}

class SelectableActivityType {

  readonly id: number;
  readonly name: string;

  constructor(
      activityType: IActivityTypeOperator
  ) {
      this.id = activityType.id;
      this.name = activityType.name;
  }
}
