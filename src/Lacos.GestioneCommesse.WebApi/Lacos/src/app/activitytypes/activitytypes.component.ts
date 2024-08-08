import { Component, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, 
  GridComponent,
  CancelEvent,
  EditEvent,
  RemoveEvent,
  SaveEvent,
  AddEvent, } from '@progress/kendo-angular-grid';
import { MessageBoxService } from '../services/common/message-box.service';
import { BaseComponent } from '../shared/base.component';
import { State } from '@progress/kendo-data-query';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { Router, NavigationEnd } from '@angular/router';
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { ActivityTypesService } from '../services/activityTypes.service';
import { ActivityTypeModel } from '../shared/models/activity-type.model';
import { ActivityTypeModalComponent } from '../activitytype-modal/activitytype-modal.component';

@Component({
  selector: 'app-activityTypes',
  templateUrl: './activitytypes.component.html',
  styleUrls: ['./activitytypes.component.scss']
})

export class ActivityTypesComponent extends BaseComponent implements OnInit {
  dataActivityTypes: GridDataResult;
  stateGridActivityTypes: State = {
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
  
  @Input() activityType = new ActivityTypeModel();
  @ViewChild('activityTypeModal', { static: true }) activityTypeModal: ActivityTypeModalComponent;

  constructor(
      private readonly _activityTypesService: ActivityTypesService,
      private readonly _messageBox: MessageBoxService,
      private readonly _router: Router
  ) {
      super();
  }

  ngOnInit() {
      this._readActivityTypes();
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
      this.stateGridActivityTypes = state;
      this._readActivityTypes();
  }

  protected _readActivityTypes() {
    this._subscriptions.push(
      this._activityTypesService.readActivityTypes(this.stateGridActivityTypes)
        .pipe(
            tap(e => {
              this.dataActivityTypes = e;
            })
        )
        .subscribe()
    );
  }

  createActivityType() {
    const request = new ActivityTypeModel();
    this._subscriptions.push(
        this.activityTypeModal.open(request)
            .pipe(
                filter(e => e),
                switchMap(() => this._activityTypesService.createActivityType(request)),
                tap(e => {
                  this._messageBox.success(`Tipo ${request.name} creato`);
                }),
                tap(() => this._readActivityTypes())
            )
            .subscribe()
    );
  }

  editActivityType(activityType: ActivityTypeModel) {
    this._subscriptions.push(
      this._activityTypesService.getActivityTypeDetail(activityType.id)
        .pipe(
            map(e => {
              return Object.assign(new ActivityTypeModel(), e);
            }),
            switchMap(e => this.activityTypeModal.open(e)),
            filter(e => e),
            map(() => this.activityTypeModal.options),
            switchMap(e => this._activityTypesService.updateActivityType(e, e.id)),
            map(() => this.activityTypeModal.options),
            tap(e => this._messageBox.success(`Tipo ${activityType.name} aggiornato`)),
            tap(() => this._readActivityTypes())
        )
      .subscribe()
    );
  }

  deleteActivityType(activityType: ActivityTypeModel) {
    this._messageBox.info('Eliminazione elemento non attiva');
    // this._messageBox.confirm(`Sei sicuro di voler cancellare il tipo ${activityType.name}?`, 'Conferma l\'azione').subscribe(result => {
    //   if (result == true) {
    //     this._subscriptions.push(
    //       this._activityTypesService.deleteActivityType(activityType.id)
    //         .pipe(
    //           tap(e => this._messageBox.success(`Tipo ${activityType.name} cancellato con successo`)),
    //           tap(() => this._readActivityTypes())
    //         )
    //       .subscribe()
    //     );
    //   }
    //});
  }

}
