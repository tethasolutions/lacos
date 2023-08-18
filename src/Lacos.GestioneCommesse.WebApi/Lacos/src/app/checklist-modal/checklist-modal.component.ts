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
import { CheckListModel } from '../shared/models/check-list.model';
import { CheckListService } from '../services/check-list.service';
import { ChecklistItemModalComponent } from '../checklist-item-modal/checklist-item-modal.component';
import { ProductTypeModel } from '../shared/models/product-type.model';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { CheckListItemModel } from '../shared/models/check-list-item.model';
import { ProductsService } from '../services/products.service';

@Component({
  selector: 'app-checklist-modal',
  templateUrl: './checklist-modal.component.html',
  styleUrls: ['./checklist-modal.component.scss']
})

export class ChecklistModalComponent extends ModalComponent<CheckListModel> {

  @ViewChild('form') form: NgForm;
  @ViewChild('checklistItemModal', { static: true }) checklistItemModal: ChecklistItemModalComponent;

  productTypes: Array<ProductTypeModel> = [];
  activityTypes: Array<ActivityTypeModel> = [];

  readonly role = Role;

  constructor(
      private readonly _messageBox: MessageBoxService,
      private readonly _checkListService: CheckListService,
      private readonly _productsService: ProductsService
  ) {
      super();
  }

  protected _canClose() {
      markAsDirty(this.form);

      if (this.form.invalid) {
          this._messageBox.error('Compilare correttamente tutti i campi');
      }

      return this.form.valid;
  }

  protected _readProductTypes() {
    this._subscriptions.push(
      this._productsService.readProductTypes()
        .pipe(
            tap(e => {
              this.productTypes = e;
            })
        )
        .subscribe()
    );
  }

  protected _readActivityTypes() {
    this._subscriptions.push(
      this._checkListService.readActivityTypes()
        .pipe(
            tap(e => {
              this.activityTypes = e;
            })
        )
        .subscribe()
    );
  }

  protected _readCheckListItems() {
    if (this.options.id == null) { return; }
    this._subscriptions.push(
      this._checkListService.readCheckListItems(this.options.id)
        .pipe(
            tap(e => {
              this.options.items = e;
            })
        )
        .subscribe()
    );
  }

  protected createNewItemCheckList() {
    const request = new CheckListItemModel();
    if (this.options.id == null) { 
      this.checklistItemModal.open(request)
        .pipe(
            filter(e => e),
            tap(e => {
              this.options.items.push(request);
              this._messageBox.success(`Voce checklist creata`);
            })
        )
        .subscribe()
    } else {
      this._subscriptions.push(
          this.checklistItemModal.open(request)
              .pipe(
                  filter(e => e),
                  switchMap(() => this._checkListService.createCheckListItem(request)),
                  tap(e => {
                    this._messageBox.success(`Voce checklist creata`);
                  }),
                  tap(() => this._readCheckListItems())
              )
              .subscribe()
      );
    }
  }

  protected editCheckListItem(dataItem: CheckListItemModel) {
    if (this.options.id == null) { 
      const request: CheckListItemModel = Object.assign(new CheckListItemModel(), JSON.parse(JSON.stringify(dataItem)));
      this.checklistItemModal.open(request)
        .pipe(
            filter(e => e),
            tap(e => {
              const indexItemToEdit = this.options.items.findIndex(x => x.tempId === request.tempId);
              if (indexItemToEdit >= 0) {
                this.options.items[indexItemToEdit] = request;
                this._messageBox.success(`Voce checklist aggiornata`);
              }
            })
        )
        .subscribe()
    } else {
      this._subscriptions.push(
        this._checkListService.getCheckListItemDetail(dataItem.id)
          .pipe(
              map(e => {
                return Object.assign(new CheckListItemModel(), e);
              }),
              switchMap(e => this.checklistItemModal.open(e)),
              filter(e => e),
              map(() => this.checklistItemModal.options),
              switchMap(e => this._checkListService.updateCheckListItem(e, dataItem.id)),
              map(() => this.checklistItemModal.options),
              tap(e => this._messageBox.success(`Voce checklist aggiornata`)),
              tap(() => this._readCheckListItems())
          )
        .subscribe()
      );
    }
  }

  protected deleteOperatorItem(dataItem: CheckListItemModel) {
    if (this.options.id == null) { 
      this._messageBox.confirm(`Sei sicuro di voler cancellare la voce "${dataItem.description}"?`, 'Conferma l\'azione').subscribe(result => {
          if (result == true) {
            this.options.items = this.options.items.filter(x => x.tempId !== dataItem.tempId);
              this._messageBox.success(`La voce "${dataItem.description}" cancellata con successo`);
          }
      });
    } else {
        this._messageBox.confirm(`Sei sicuro di voler cancellare la voce "${dataItem.description}"?`, 'Conferma l\'azione').subscribe(result => {
        if (result == true) {
          this._subscriptions.push(
            this._checkListService.deleteCheckListItem(dataItem.id)
              .pipe(
                tap(e => this._messageBox.success(`La voce "${dataItem.description}" cancellata con successo`)),
                tap(() => this._readCheckListItems())
              )
            .subscribe()
          );
        }
      });
    }
  }

  public loadData() {
    this._readProductTypes();
    this._readActivityTypes();
  }

}
