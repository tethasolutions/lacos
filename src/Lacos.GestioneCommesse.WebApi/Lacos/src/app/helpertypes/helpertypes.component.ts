import { Component, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { HelperTypesService } from '../services/helper/helperTypes.service';
import { HelperTypeModel } from '../services/helper/models';
import { HelperTypeModalComponent } from './helpertype-modal.component';

@Component({
  selector: 'app-helpertypes',
  templateUrl: './helpertypes.component.html'
})

export class HelperTypesComponent extends BaseComponent implements OnInit {
  dataHelperTypes: GridDataResult;
  stateGridHelperTypes: State = {
      skip: 0,
      take: 30,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };
  
  @ViewChild('helperTypeModal', { static: true }) helperTypeModal: HelperTypeModalComponent;

  screenWidth: number;
  
  constructor(
      private readonly _service: HelperTypesService,
      private readonly _messageBox: MessageBoxService,
  ) {
      super();
  }

  ngOnInit() {
      this._readHelperTypes();
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
      this.stateGridHelperTypes = state;
      this._readHelperTypes();
  }

  protected _readHelperTypes() {
    this._subscriptions.push(
      this._service.read(this.stateGridHelperTypes)
        .pipe(
            tap(e => {
              this.dataHelperTypes = e;
            })
        )
        .subscribe()
    );
  }

  createHelperType() {
    const helperType = new HelperTypeModel(0,null);

    this._subscriptions.push(
        this.helperTypeModal.open(helperType)
            .pipe(
                filter(e => e),
                switchMap(() => this._service.create(helperType)),
                tap(e => {
                  this._messageBox.success(`Tipo ${helperType.type} creato`);
                }),
                tap(() => this._readHelperTypes())
            )
            .subscribe()
    );
  }

  editHelperType(helperType: HelperTypeModel) {
    this._subscriptions.push(
      this._service.get(helperType.id)
        .pipe(
            map(e => {
              return Object.assign(new HelperTypeModel(e.id, e.type), e);
            }),
            switchMap(e => this.helperTypeModal.open(e)),
            filter(e => e),
            map(() => this.helperTypeModal.options),
            switchMap(e => this._service.update(e)),
            tap(e => this._messageBox.success(`Tipo ${e.type} aggiornato`)),
            tap(() => this._readHelperTypes())
        )
      .subscribe()
    );
  }

  deleteHelperType(helperType: HelperTypeModel) {
    this._messageBox.info('Eliminazione elemento non attiva');
    this._messageBox.confirm(`Sei sicuro di voler cancellare il tipo ${helperType.type}?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._service.delete(helperType.id)
            .pipe(
              tap(e => this._messageBox.success(`Tipo ${helperType.type} cancellato con successo`)),
              tap(() => this._readHelperTypes())
            )
          .subscribe()
        );
      }
    });
  }

}
