import { Component, ViewChild } from '@angular/core';
import { ModalFormComponent } from '../shared/modal.component';
import { markAsDirty } from '../services/common/functions';
import { MessageBoxService } from '../services/common/message-box.service';
import { Role } from '../services/security/models';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { MaintenancePriceListItemModalComponent } from './maintenance-price-list-item-modal.component';
import { MaintenancePriceListItemModel, MaintenancePriceListModel } from '../shared/models/maintenance-price-list.model';
import { MaintenancePriceListService } from '../services/maintenance-price-list.service';

@Component({
  selector: 'app-maintenance-price-list-modal',
  templateUrl: './maintenance-price-list-modal.component.html'
})

export class MaintenancePriceListModalComponent extends ModalFormComponent<MaintenancePriceListModel> {

  @ViewChild('maintenancePriceListItemModal', { static: true }) maintenancePriceListItemModal: MaintenancePriceListItemModalComponent;

  readonly role = Role;

  constructor(
      messageBox: MessageBoxService,
      private readonly _maintenancePriceListService: MaintenancePriceListService
  ) {
      super(messageBox);
  }
  
  override open(maintenancePriceListModel : MaintenancePriceListModel){
    const result = super.open(maintenancePriceListModel);
    this.loadData();
    return result;
  }

  protected _canClose() {
      markAsDirty(this.form);

      if (this.form.invalid) {
          this._messageBox.error('Compilare correttamente tutti i campi');
      }

      return this.form.valid;
  }

  protected _readMaintenancePriceListItems() {
    if (this.options.id == null) { return; }
    this._subscriptions.push(
      this._maintenancePriceListService.readMaintenancePriceListItems(this.options.id)
        .pipe(
            tap(e => {
              this.options.items = e;
            })
        )
        .subscribe()
    );
  }

  protected createNewItem() {
    const request = new MaintenancePriceListItemModel();
    if (this.options.id == null) { 
      this.maintenancePriceListItemModal.open(request)
        .pipe(
            filter(e => e),
            tap(e => {
              this.options.items.push(request);
              this._messageBox.success(`Voce listino creata`);
            })
        )
        .subscribe()
    } else {
      request.maintenancePriceListId = this.options.id;
      this._subscriptions.push(
          this.maintenancePriceListItemModal.open(request)
              .pipe(
                  filter(e => e),
                  switchMap(() => this._maintenancePriceListService.createMaintenancePriceListItem(request)),
                  tap(e => {
                    this._messageBox.success(`Voce listino creata`);
                  }),
                  tap(() => this._readMaintenancePriceListItems())
              )
              .subscribe()
      );
    }
  }

  protected editItem(dataItem: MaintenancePriceListItemModel) {
    if (this.options.id == null) { 
      const request: MaintenancePriceListItemModel = Object.assign(new MaintenancePriceListItemModel(), JSON.parse(JSON.stringify(dataItem)));
      this.maintenancePriceListItemModal.open(request)
        .pipe(
            filter(e => e),
            tap(e => {
              const indexItemToEdit = this.options.items.findIndex(x => x.tempId === request.tempId);
              if (indexItemToEdit >= 0) {
                this.options.items[indexItemToEdit] = request;
                this._messageBox.success(`Voce listino aggiornata`);
              }
            })
        )
        .subscribe()
    } else {
      this._subscriptions.push(
        this._maintenancePriceListService.getMaintenancePriceListItemDetail(dataItem.id)
          .pipe(
              map(e => {
                return Object.assign(new MaintenancePriceListItemModel(), e);
              }),
              switchMap(e => this.maintenancePriceListItemModal.open(e)),
              filter(e => e),
              map(() => this.maintenancePriceListItemModal.options),
              switchMap(e => this._maintenancePriceListService.updateMaintenancePriceListItem(e, dataItem.id)),
              map(() => this.maintenancePriceListItemModal.options),
              tap(e => this._messageBox.success(`Voce listino aggiornata`)),
              tap(() => this._readMaintenancePriceListItems())
          )
        .subscribe()
      );
    }
  }

  protected deleteItem(dataItem: MaintenancePriceListItemModel) {
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
            this._maintenancePriceListService.deleteMaintenancePriceListItem(dataItem.id)
              .pipe(
                tap(e => this._messageBox.success(`La voce "${dataItem.description}" cancellata con successo`)),
                tap(() => this._readMaintenancePriceListItems())
              )
            .subscribe()
          );
        }
      });
    }
  }

  public loadData() {
    this._readMaintenancePriceListItems();
  }

}
