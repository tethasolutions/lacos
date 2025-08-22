import { Component, Input, ViewChild } from "@angular/core";
import { BaseComponent } from "./base.component";
import { ContextMenuComponent, ContextMenuSelectEvent } from "@progress/kendo-angular-menu";
import { Router } from "@angular/router";
import { CellClickEvent } from "@progress/kendo-angular-grid";
import { JobsService } from "../services/jobs/jobs.service";
import { JobModalComponent } from "../jobs/job-modal.component";
import { filter, switchMap, tap } from "rxjs";
import { CopyToJobModalComponent } from "./copy-to-job-modal.component";
import { ActivitiesService } from "../services/activities/activities.service";
import { MessageBoxService } from "../services/common/message-box.service";
import { UserService } from "../services/security/user.service";
import { Role, User } from "../services/security/models";
import { CopyToJobModel } from "./models/copy-to-job.model";
import { PurchaseOrdersService } from "../services/purchase-orders/purchase-orders.service";

@Component({
  selector: 'app-grid-context-menu',
  templateUrl: 'grid-context-menu.component.html'
})
export class GridContextMenuComponent extends BaseComponent {

  @ViewChild("gridMenu", { static: true }) gridMenu: ContextMenuComponent;
  @ViewChild('jobDetailModal', { static: true }) jobDetailModal: JobModalComponent;
  @ViewChild('copyToJobModal', { static: true }) copyToJobModal: CopyToJobModalComponent;

  @Input() key: string;

  showJobIdDialog = false;
  jobIds: string[] = [];
  private jobIdResolve: ((value: any) => void) | null = null;

  constructor(
    private readonly router: Router,
    private readonly _jobsService: JobsService,
    private readonly _activityService: ActivitiesService,
    private readonly _purchaseOrderService: PurchaseOrdersService,
    private readonly _user: UserService,
    private readonly _messageboxService: MessageBoxService
  ) {
    super();
  }

  private contextItem: any;
  public isActivityGrid: boolean = false;
  public isPurchaseOrderGrid: boolean = false;
  user: User;
  isOperator: boolean = false;
  jobCodesList: string[] = [];
  private _currentValueArray: string[] = [];

  ngOnInit() {
    this.user = this._user.getUser();
    this.isOperator = this.user?.role === Role.Operator;;
  }

  getSelectedJobId(): Promise<any> {
    const jobCodesRaw = this.contextItem['jobCodes'];
    const valueRaw = this.contextItem[this.key];

    if (!valueRaw) {
      return Promise.resolve(null);
    }

    if (!jobCodesRaw) {
      return valueRaw;
    }

    const jobCodesArr = typeof jobCodesRaw === 'string'
      ? jobCodesRaw.split(',').map(x => x.trim()).filter(Boolean)
      : Array.isArray(jobCodesRaw) ? jobCodesRaw : [jobCodesRaw];

    const valueArr = typeof valueRaw === 'string'
      ? valueRaw.split(',').map(x => x.trim())
      : Array.isArray(valueRaw) ? valueRaw : [valueRaw];

    if (jobCodesArr.length > 1) {
      this.jobCodesList = jobCodesArr;
      this._currentValueArray = valueArr;
      this.showJobIdDialog = true;
      return new Promise<any>((resolve) => {
        this.jobIdResolve = resolve;
      });
    }
    return Promise.resolve(valueArr[0]);
  }

  onJobIdSelected(selectedJobCode: string) {
    this.showJobIdDialog = false;
    if (this.jobIdResolve) {
      const idx = this.jobCodesList.findIndex(code => code === selectedJobCode);
      const selectedValue = this._currentValueArray[idx] ?? null;
      this.jobIdResolve(selectedValue);
      this.jobIdResolve = null;
    }
  }


  onJobIdDialogCancel() {
    this.showJobIdDialog = false;
    if (this.jobIdResolve) {
      this.jobIdResolve(null);
      this.jobIdResolve = null;
    }
  }

  public onCellClick(
    e: CellClickEvent,
    isActivityGrid: boolean = false,
    isPurchaseOrderGrid: boolean = false
  ): void {
    if (e.type === "contextmenu") {
      const originalEvent = e.originalEvent;

      originalEvent.preventDefault();

      this.contextItem = e.dataItem;
      this.isActivityGrid = isActivityGrid;
      this.isPurchaseOrderGrid = isPurchaseOrderGrid;

      this.gridMenu.show({
        left: originalEvent.pageX,
        top: originalEvent.pageY,
      });
    }
  }

  public async onSelect(event: ContextMenuSelectEvent): Promise<void> {
    const jobId = await this.getSelectedJobId();
    if (!jobId) return;

    if (event.item.text === "Scheda Commessa") {
      this._subscriptions.push(
        this._jobsService.get(jobId)
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
      this.router.navigate(['/job-details'], { queryParams: { jobId: jobId } });
    }
    if (event.item.text === "Voci Contabili Commessa") {
      this.router.navigate(['/job-accountings'], { queryParams: { jobId: jobId } });
    }
    if (event.item.text === "Attività Commessa") {
      this.router.navigate(['/activities'], { queryParams: { jobId: jobId } });
    }
    if (event.item.text === "Ticket Commessa") {
      this.router.navigate(['/tickets'], { queryParams: { jobId: jobId } });
    }
    if (event.item.text === "Ordini d'acquisto Commessa") {
      this.router.navigate(['/purchase-orders'], { queryParams: { jobId: jobId } });
    }
    if (event.item.text === "Interventi Commessa") {
      this.router.navigate(['/interventions-list'], { queryParams: { jobId: jobId } });
    }
    if (event.item.text === "Copia Attività") {
      const options = new CopyToJobModel(this.contextItem["id"], 0);

      this._subscriptions.push(
        this.copyToJobModal.open(options)
          .pipe(
            filter(e => e),
            switchMap(() => this._activityService.copyActivityToJob(options)),
            tap(() => {
              this._messageboxService.success("Attività copiata con successo");
              this.router.navigate(['/job-details'], { queryParams: { jobId: options.jobId } });
            })
          )
          .subscribe()
      );
    }
    if (event.item.text === "Copia Ordine d'acquisto") {
      const options = new CopyToJobModel(this.contextItem["id"], 0);

      this._subscriptions.push(
        this.copyToJobModal.open(options)
          .pipe(
            filter(e => e),
            switchMap(() => this._purchaseOrderService.copyPurchaseOrderToJob(options)),
            tap(() => {
              this._messageboxService.success("Ordine d'acquisto copiato con successo");
              this.router.navigate(['/job-details'], { queryParams: { jobId: options.jobId } });
            })
          )
          .subscribe()
      );
    }
  }
}