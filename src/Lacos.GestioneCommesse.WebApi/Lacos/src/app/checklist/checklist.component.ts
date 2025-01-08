import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Router, NavigationEnd } from '@angular/router';
import { CheckListModel, CopyChecklistModel } from '../shared/models/check-list.model';
import { ChecklistModalComponent } from '../checklist-modal/checklist-modal.component';
import { CheckListService } from '../services/check-list.service';
import { ApiUrls } from '../services/common/api-urls';
import { CopyChecklistModalComponent } from '../checklist-modal/copy-checklist-modal.component';

@Component({
  selector: 'app-checklist',
  templateUrl: './checklist.component.html',
  styleUrls: ['./checklist.component.scss']
})
export class ChecklistComponent extends BaseComponent implements OnInit {

  @ViewChild('checklistModal', { static: true }) checklistModal: ChecklistModalComponent;
  @ViewChild('copyChecklistModal', { static: true }) copyChecklistModal: CopyChecklistModalComponent;

  pathImage = `${ApiUrls.baseUrl}/attachments/`;
  checklists: GridDataResult;

  stateGridChecklists: State = {
      skip: 0,
      take: 30,
      filter: {
          filters: [],
          logic: 'and'
      },
      group: [],
      sort: []
  };

  checklistSelezionato = new CheckListModel();
  screenWidth: number;

  constructor(
      private readonly _checkListService: CheckListService,
      private readonly _messageBox: MessageBoxService,
      private readonly _router: Router
  ) {
      super();
  }

  ngOnInit() {
      this._readChecklists();
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

  copyChecklist(checkList: CheckListModel) {
        const options = new CopyChecklistModel(checkList.id, checkList.productTypeId, 0);
  
        this._subscriptions.push(
          this.copyChecklistModal.open(options)
            .pipe(
              filter(e => e),
              switchMap(() => this._checkListService.copyChecklist(options)),
              tap(() => {
                this._messageBox.success("Checklist copiata con successo");
                this._router.navigate(['/checklist']);
              })
            )
            .subscribe()
        );
      }
}
