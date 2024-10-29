import { Component, Input, ViewChild } from "@angular/core";
import { BaseComponent } from "./base.component";
import { ContextMenuComponent, ContextMenuSelectEvent } from "@progress/kendo-angular-menu";
import { Router } from "@angular/router";
import { CellClickEvent } from "@progress/kendo-angular-grid";
import { JobsService } from "../services/jobs/jobs.service";
import { JobModalComponent } from "../jobs/job-modal.component";
import { filter, switchMap, tap } from "rxjs";

@Component({
    selector: 'app-grid-context-menu',
    templateUrl: 'grid-context-menu.component.html'
})
export class GridContextMenuComponent extends BaseComponent {

    @ViewChild("gridMenu", {static: true}) gridMenu: ContextMenuComponent;
    @ViewChild('jobDetailModal', { static: true }) jobDetailModal: JobModalComponent;

    @Input() key: string;

    constructor (
        private readonly router: Router,
        private readonly _jobsService: JobsService
    )
    {
        super();
    }

    
private contextItem: any;

public onCellClick(
    e: CellClickEvent,
  ): void {
    if (e.type === "contextmenu") {
      const originalEvent = e.originalEvent;

      originalEvent.preventDefault();

      this.contextItem = e.dataItem;

      this.gridMenu.show({
        left: originalEvent.pageX,
        top: originalEvent.pageY,
      });
    }
  }

  public onSelect(event: ContextMenuSelectEvent): void {
    
    if (event.item.text === "Scheda Commessa") {
      this._subscriptions.push(
          this._jobsService.get(this.contextItem[this.key])
              .pipe(
                  switchMap(e => this.jobDetailModal.open(e)),
                  filter(e => e),
                  switchMap(() => this._jobsService.update(this.jobDetailModal.options)),
                  tap(() => window.location.reload())
              )
              .subscribe()
      );
    }
    if (event.item.text === "Commessa") {
        this.router.navigate(['/job-details'], {queryParams: {jobId: this.contextItem[this.key]}});
    }
    if (event.item.text === "Attività Commessa") {
        this.router.navigate(['/activities'], {queryParams: {jobId: this.contextItem[this.key]}});
    }
    if (event.item.text === "Ticket Commessa") {
        this.router.navigate(['/tickets'], {queryParams: {jobId: this.contextItem[this.key]}});
    }
    if (event.item.text === "Ordini d'acquisto Commessa") {
        this.router.navigate(['/purchase-orders'], {queryParams: {jobId: this.contextItem[this.key]}});
    }
    if (event.item.text === "Interventi Commessa") {
        this.router.navigate(['/interventions-list'], {queryParams: {jobId: this.contextItem[this.key]}});
    }
  }
}