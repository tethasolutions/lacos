import { Component, ViewChild, Input } from '@angular/core';
import { ModalComponent, ModalFormComponent } from '../shared/modal.component';
import { NgForm } from '@angular/forms';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { OperatorModel } from '../shared/models/operator.model';
import { VehicleModel } from '../shared/models/vehicle.model';
import { VehiclesService } from '../services/vehicles.service';
import { tap } from 'rxjs/operators';
import { SimpleLookupModel } from '../shared/models/simple-lookup.model';
import { OperatorDocumentsModalComponent } from '../operator-documents-modal/operator-documents-modal.component';
import { ApiUrls } from '../services/common/api-urls';
import { SuccessEvent, FileInfo, SelectEvent } from "@progress/kendo-angular-upload";
import { UploadFileModel } from '../shared/models/upload-file.model';
import { OperatorDocumentModel } from '../shared/models/operator-document.model';
import { IActivityTypeOperator } from '../shared/models/activity-type.model';
import { ActivityTypesService } from '../services/activityTypes.service';

@Component({
    selector: 'app-operator-modal',
    templateUrl: './operator-modal.component.html'
})
export class OperatorModalComponent extends ModalFormComponent<OperatorModel> {

    @ViewChild('operatorDocumentsModal', { static: true })
    operatorDocumentsModal: OperatorDocumentsModalComponent;

    private readonly _baseUrl = `${ApiUrls.baseApiUrl}/operators`;
    pathImage = `${ApiUrls.baseUrl}/attachments/`;
    signatureFileInfo: any;
    signatureUploaded: boolean;

    uploadSaveUrl = `${this._baseUrl}/document/upload-file`;
    uploadRemoveUrl = `${this._baseUrl}/document/remove-file`;
    attachmentsFileInfo: Array<FileInfo> = [];
    attachmentsUploads: Array<UploadFileModel> = [];
    isUploaded: Array<boolean> = [];
    vehicles: Array<VehicleModel> = [];
    roles: Array<SimpleLookupModel> = [];
    activityTypes: SelectableActivityType[];
    notHasUser: boolean;

    readonly role = Role;

    @Input()
    operator = new OperatorModel();

    constructor(
        messageBox: MessageBoxService,
        private readonly _vehiclesService: VehiclesService,
        private readonly _activityTypesService: ActivityTypesService
    ) {
        super(messageBox);
    }

    override open(options: OperatorModel) {
        const result = super.open(options);

        this.attachmentsFileInfo = [];
        this.isUploaded = [];
        this.attachmentsUploads = [];
        this.signatureUploaded = false;

        this.notHasUser = !this.options.hasUser;

        this.options.documents.forEach(element => {
            if (element.originalFileName != null && element.fileName != null) {
                const noteAttachment = new UploadFileModel(element.fileName, element.originalFileName);
                this.attachmentsUploads.push(noteAttachment);
                this.attachmentsFileInfo.push({ name: element.originalFileName });
                this.isUploaded.push(true);
            }
        });

        return result;
    }

    SignatureExecutionSuccess(e: SuccessEvent): void {
        const body = e.response.body;
        if (body != null) {

            const uploadedFile = body as UploadFileModel;
            const operatorAttachment = new UploadFileModel(uploadedFile.fileName, uploadedFile.originalFileName);
            this.options.signatureFileName = uploadedFile.fileName;
            this.signatureUploaded = true;
        }
        else {
            const deletedFile = e.files[0].name;
            this.options.signatureFileName = null;
        }
    }

    AttachmentExecutionSuccess(e: SuccessEvent) {
        const body = e.response.body;
        if (body != null) {

            const uploadedFile = body as UploadFileModel;
            const operatorAttachment = new UploadFileModel(uploadedFile.fileName, uploadedFile.originalFileName);
            this.attachmentsUploads.push(operatorAttachment);
            let operatorAttachmentModal = new OperatorDocumentModel();
            operatorAttachmentModal.fileName = uploadedFile.fileName;
            operatorAttachmentModal.originalFileName = uploadedFile.originalFileName;
            this.options.documents.push(operatorAttachmentModal);
            this.isUploaded.push(true);
        }
        else {
            const deletedFile = e.files[0].name;
            const index = this.attachmentsUploads.findIndex(x => x.originalFileName == deletedFile);
            if (index > -1) {
                this.attachmentsUploads.splice(index, 1);
                this.options.documents.splice(index, 1);
                this.isUploaded.pop();
            }
        }
    }

    AttachmentSelect(e: SelectEvent) {
        const files = e.files;
        let popup = false;
        files.forEach(element => {
            var index = this.attachmentsUploads.findIndex(x => x.originalFileName == element.name);
            if (index > -1) {
                files.splice(index, 1);
                popup = true;
            }
        });
        if (popup) {
            this._messageBox.alert(`Sono presenti tra i file caricati alcuni file con lo stesso nome di quelli che si vogliono caricare`);
        }
    }

    CreateUrl(fileName: string) {
        let ret = "";
        this.attachmentsUploads.forEach(element => {
            if (element.originalFileName == fileName)
                ret = `${this._baseUrl}/document/download-file/${element.fileName}/${element.originalFileName}`;
        });
        return ret;
    }

    setRoles() {
        this.roles = [];
        for (var n in Role) {
            if (typeof Role[n] === 'number') {
                this.roles.push({ id: <any>Role[n], name: n });
            }
        }
    }

    viewDocuments() {
        this.operatorDocumentsModal.operatorId = this.options.id;
        this.operatorDocumentsModal.loadData();
        this.operatorDocumentsModal.open(null);
    }


    loadData() {
        this._readVehicles();
        this._readActivityTypes();
        this.setRoles();
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
                    tap(e => this.vehicles = e)
                )
                .subscribe()
        );
    }

    protected _readActivityTypes() {
        this._subscriptions.push(
            this._activityTypesService.readActivityTypesList()
                .pipe(
                    tap(e => this.activityTypes = e)
                )
                .subscribe()
        );
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
