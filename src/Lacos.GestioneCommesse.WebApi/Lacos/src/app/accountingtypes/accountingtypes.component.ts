import { Component, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AccountingTypeModel } from '../shared/models/accounting-type.model';
import { AccountingTypesService } from '../services/accountingTypes.service';
import { AccountingTypeModalComponent } from './accountingtype-modal.component';

@Component({
  selector: 'app-accountingTypes',
  templateUrl: './accountingtypes.component.html'
})

export class AccountingTypesComponent extends BaseComponent implements OnInit {
  dataAccountingTypes: GridDataResult;
  stateGridAccountingTypes: State = {
      skip: 0,
      take: 30,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };
  
  screenWidth: number;
  
  @Input() accountingType = new AccountingTypeModel();
  @ViewChild('accountingTypeModal', { static: true }) accountingTypeModal: AccountingTypeModalComponent;

  constructor(
      private readonly _accountingTypesService: AccountingTypesService,
      private readonly _messageBox: MessageBoxService,
      private readonly _router: Router
  ) {
      super();
  }

  ngOnInit() {
      this._readAccountingTypes();
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
      this.stateGridAccountingTypes = state;
      this._readAccountingTypes();
  }

  protected _readAccountingTypes() {
    this._subscriptions.push(
      this._accountingTypesService.readAccountingTypes(this.stateGridAccountingTypes)
        .pipe(
            tap(e => {
              this.dataAccountingTypes = e;
            })
        )
        .subscribe()
    );
  }

  createAccountingType() {
    const request = new AccountingTypeModel();
    this._subscriptions.push(
        this.accountingTypeModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._accountingTypesService.createAccountingType(request)),
                tap(e => {
                  this._messageBox.success(`Tipo ${request.name} creato`);
                }),
                tap(() => this._readAccountingTypes())
            )
            .subscribe()
    );
  }

  editAccountingType(accountingType: AccountingTypeModel) {
    this._subscriptions.push(
      this._accountingTypesService.getAccountingTypeDetail(accountingType.id)
        .pipe(
            map(e => {
              return Object.assign(new AccountingTypeModel(), e);
            }),
            switchMap(e => this.accountingTypeModal.open(e)),
            filter(e => e),
            map(() => this.accountingTypeModal.options),
            switchMap(e => this._accountingTypesService.updateAccountingType(e, e.id)),
            map(() => this.accountingTypeModal.options),
            tap(e => this._messageBox.success(`Tipo ${accountingType.name} aggiornato`)),
            tap(() => this._readAccountingTypes())
        )
      .subscribe()
    );
  }

  deleteAccountingType(accountingType: AccountingTypeModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare il tipo ${accountingType.name}?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._accountingTypesService.deleteAccountingType(accountingType.id)
            .pipe(
              tap(e => this._messageBox.success(`Tipo ${accountingType.name} cancellato con successo`)),
              tap(() => this._readAccountingTypes())
            )
          .subscribe()
        );
      }
    });
  }

}
