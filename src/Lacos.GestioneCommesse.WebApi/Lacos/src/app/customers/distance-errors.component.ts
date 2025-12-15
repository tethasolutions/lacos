import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { AddressesService } from '../services/addresses.service';
import { AddressModalComponent } from '../address-modal/address-modal.component';
import { AddressModel, AddressReadModel } from '../shared/models/address.model';

@Component({
  selector: 'app-distance-errors',
  templateUrl: './distance-errors.component.html'
})

export class DistanceErrorsComponent extends BaseComponent implements OnInit {

  @ViewChild('addressModal', { static: true }) addressModal: AddressModalComponent;

  dataAddresses: GridDataResult;
  stateGrid: State = {
    skip: 0,
    take: 30,
    filter: {
      filters: [],
      logic: 'and'
    },
    group: [],
    sort: [{
      field: 'distanceKm', dir: 'desc'
    }]
  };

  screenWidth: number;

  constructor(
    private readonly _addressService: AddressesService,
    private readonly _messageBox: MessageBoxService
  ) {
    super();
  }

  ngOnInit() {
    this._readDistanceErrors();
    this.updateScreenSize();
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
    this.stateGrid = state;
    this._readDistanceErrors();
  }

  protected _readDistanceErrors() {
    this._subscriptions.push(
      this._addressService.readDistanceErrors(this.stateGrid)
        .pipe(
          tap(e => {
            this.dataAddresses = e;
          })
        )
        .subscribe()
    );
  }

  editAddress(address: AddressReadModel) {
    this._subscriptions.push(
      this._addressService.getAddress(address.id)
        .pipe(
          map(e => {
            return Object.assign(new AddressModel(), e);
          }),
          switchMap(e => this.addressModal.open(e)),
          filter(e => e),
          map(() => this.addressModal.options),
          switchMap(e => this._addressService.updateAddress(e, address.id)),
          map(() => this.addressModal.options),
          tap(e => this._messageBox.success(`Indirizzo ${address.description} aggiornato`)),
          tap(() => this._readDistanceErrors())
        )
        .subscribe()
    );
  }

  openMap(address: AddressReadModel) {
    const addressForMap = encodeURIComponent(address.fullAddressForDistance);
    const url = `https://www.openstreetmap.org/search?query=${addressForMap}`;
    window.open(url, '_blank');
  }
  
  openGoogleMap(address: AddressReadModel) {
    const addressForMap = encodeURIComponent(address.fullAddressForDistance);
    const url = `https://www.google.it/maps/place/${addressForMap.replace(/ /g, '+')}`;
    window.open(url, '_blank');
  }
}
