import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { MaintenancePriceListModalComponent } from './maintenance-price-list-modal.component';
import { MaintenancePriceListModel } from '../shared/models/maintenance-price-list.model';
import { MaintenancePriceListService } from '../services/maintenance-price-list.service';

@Component({
  selector: 'app-maintenance-price-list',
  templateUrl: './maintenance-price-list.component.html'
})
export class MaintenancePriceListComponent extends BaseComponent implements OnInit {

  @ViewChild('maintenancePriceListModal', { static: true }) maintenancePriceListModal: MaintenancePriceListModalComponent;

  maintenancePriceLists: GridDataResult;

  stateGridMaintenancePriceLists: State = {
      skip: 0,
      take: 30,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };

  maintenancePriceListSelezionato = new MaintenancePriceListModel();
  screenWidth: number;

  constructor(
      private readonly _maintenancePriceListService: MaintenancePriceListService,
      private readonly _messageBox: MessageBoxService
  ) {
      super();
  }

  ngOnInit() {
      this._readMaintenancePriceLists();
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


  dataStateChange(state: State) {
      this.stateGridMaintenancePriceLists = state;
      this._readMaintenancePriceLists();
  }

  protected _readMaintenancePriceLists() {
    this._subscriptions.push(
      this._maintenancePriceListService.readMaintenancePriceList(this.stateGridMaintenancePriceLists)
        .pipe(
            tap(e => {
              this.maintenancePriceLists = e;
            })
        )
        .subscribe()
    );
  }

  createMaintenancePriceList() {
    const request = new MaintenancePriceListModel();
    this._subscriptions.push(
        this.maintenancePriceListModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._maintenancePriceListService.createMaintenancePriceList(request)),
                tap(e => {
                  this._messageBox.success(`Listino creato`);
                }),
                tap(() => this._readMaintenancePriceLists())
            )
            .subscribe()
    );
  }

  editMaintenancePriceList(maintenancePriceList: MaintenancePriceListModel) {
    this._subscriptions.push(
      this._maintenancePriceListService.getMaintenancePriceListDetail(maintenancePriceList.id)
        .pipe(
            map(e => {
              return Object.assign(new MaintenancePriceListModel(), e);
            }),
            switchMap(e => this.maintenancePriceListModal.open(e)),
            filter(e => e),
            map(() => this.maintenancePriceListModal.options),
            switchMap(e => this._maintenancePriceListService.updateMaintenancePriceList(e, maintenancePriceList.id)),
            map(() => this.maintenancePriceListModal.options),
            tap(e => this._messageBox.success(`Listino aggiornato`)),
            tap(() => this._readMaintenancePriceLists())
        )
      .subscribe()
    );
  }

  deleteMaintenancePriceList(maintenancePriceList: MaintenancePriceListModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare Listino "${maintenancePriceList.description}"?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._maintenancePriceListService.deleteMaintenancePriceList(maintenancePriceList.id)
            .pipe(
              tap(e => this._messageBox.success(`Listino "${maintenancePriceList.description}" cancellato con successo`)),
              tap(() => this._readMaintenancePriceLists())
            )
          .subscribe()
        );
      }
    });
  }

}
