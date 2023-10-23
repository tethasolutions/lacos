import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Router, NavigationEnd } from '@angular/router';
import { CheckListModel } from '../shared/models/check-list.model';
import { ChecklistModalComponent } from '../checklist-modal/checklist-modal.component';
import { CheckListService } from '../services/check-list.service';
import { ApiUrls } from '../services/common/api-urls';

@Component({
  selector: 'app-checklist',
  templateUrl: './checklist.component.html',
  styleUrls: ['./checklist.component.scss']
})
export class ChecklistComponent extends BaseComponent implements OnInit {

  @ViewChild('checklistModal', { static: true }) checklistModal: ChecklistModalComponent;

  pathImage = `${ApiUrls.baseUrl}/attachments/`;
  checklists: GridDataResult;

  stateGridChecklists: State = {
      skip: 0,
      take: 20,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };

  checklistSelezionato = new CheckListModel();

  constructor(
      private readonly _checkListService: CheckListService,
      private readonly _messageBox: MessageBoxService,
      private readonly _router: Router
  ) {
      super();
  }

  ngOnInit() {
      this._readChecklists();
  }

  dataStateChange(state: State) {
      this.stateGridChecklists = state;
      this._readChecklists();
  }

  protected _readChecklists() {
    this._subscriptions.push(
      this._checkListService.readCheckList(this.stateGridChecklists)
        .pipe(
            tap(e => {
              this.checklists = e;
            })
        )
        .subscribe()
    );
  }

  createCheckList() {
    const request = new CheckListModel();
    this._subscriptions.push(
        this.checklistModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._checkListService.createCheckList(request)),
                tap(e => {
                  this._messageBox.success(`Checklist creato`);
                }),
                tap(() => this._readChecklists())
            )
            .subscribe()
    );
  }

  editCheckList(checkList: CheckListModel) {
    this._subscriptions.push(
      this._checkListService.getCheckListDetail(checkList.id)
        .pipe(
            map(e => {
              return Object.assign(new CheckListModel(), e);
            }),
            switchMap(e => this.checklistModal.open(e)),
            filter(e => e),
            map(() => this.checklistModal.options),
            switchMap(e => this._checkListService.updateCheckList(e, checkList.id)),
            map(() => this.checklistModal.options),
            tap(e => this._messageBox.success(`Checklist aggiornato`)),
            tap(() => this._readChecklists())
        )
      .subscribe()
    );
  }

  deleteOperator(checkList: CheckListModel) {
    this._messageBox.confirm(`Sei sicuro di voler cancellare checklist "${checkList.description}"?`, 'Conferma l\'azione').subscribe(result => {
      if (result == true) {
        this._subscriptions.push(
          this._checkListService.deleteCheckList(checkList.id)
            .pipe(
              tap(e => this._messageBox.success(`Checklist "${checkList.description}" cancellato con successo`)),
              tap(() => this._readChecklists())
            )
          .subscribe()
        );
      }
    });
  }

}
