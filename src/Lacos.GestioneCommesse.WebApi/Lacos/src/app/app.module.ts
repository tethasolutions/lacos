import { APP_INITIALIZER, LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoaderComponent } from './shared/loader.component';
import { ValidationMessageComponent } from './shared/validation-message.component';
import { UsersComponent } from './security/users.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './security/login.component';
import { LogoutComponent } from './security/logout.component';
import { UserModalComponent } from './security/user-modal.component';
import { registerLocaleData } from '@angular/common';
import localeIt from '@angular/common/locales/it';
import localeExtraIt from '@angular/common/locales/extra/it';
import '@progress/kendo-angular-intl/locales/it/all';
import '@angular/common/locales/global/it';
import { ExcelModule, GridModule, PDFModule } from '@progress/kendo-angular-grid';
import { PDFExportModule } from '@progress/kendo-angular-pdf-export';
import { InputsModule, NumericTextBoxModule, SwitchModule } from '@progress/kendo-angular-inputs';
import { DropDownButtonModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { EditorModule } from '@progress/kendo-angular-editor';
import { IntlModule } from '@progress/kendo-angular-intl';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { TooltipsModule } from "@progress/kendo-angular-tooltip";
import { StorageService } from './services/common/storage.service';
import { UserService } from './services/security/user.service';
import { VehiclesService } from './services/vehicles.service';
import { SecurityService, refreshUserData } from './services/security/security.service';
import { HeadersInterceptor } from './services/interceptors/headers.interceptor';
import { ResponseInterceptor } from './services/interceptors/response.interceptor';
import { MessageBoxService } from './services/common/message-box.service';
import { Router } from '@angular/router';
import { LoaderService } from './services/common/loader.service';
import { LoaderInterceptor } from './services/interceptors/loader.interceptor';
import { AuthGuard } from './services/guards/auth.guard';
import { MenuComponent } from './menu/menu.component';
import { BooleanPipe } from './pipes/boolean.pipe';
import { CustomersComponent } from './customers/customers.component';
import { CustomerService } from './services/customer.service';
import { CustomerModalComponent } from './customer-modal/customer-modal.component';
import { AddressesService } from './services/addresses.service';
import { AddressModalComponent } from './address-modal/address-modal.component';
import { AddressesModalComponent } from './addresses-modal/addresses-modal.component';
import { VehiclesComponent } from './vehicles/vehicles.component';
import { VehicleModalComponent } from './vehicle-modal/vehicle-modal.component';
import { OperatorsService } from './services/operators.service';
import { OperatorsComponent } from './operators/operators.component';
import { OperatorModalComponent } from './operator-modal/operator-modal.component';
import { OperatorDocumentsModalComponent } from './operator-documents-modal/operator-documents-modal.component';
import { OperatorDocumentModalComponent } from './operator-document-modal/operator-document-modal.component';
import { UploadsModule } from "@progress/kendo-angular-upload";
import { ChecklistComponent } from './checklist/checklist.component';
import { ChecklistModalComponent } from './checklist-modal/checklist-modal.component';
import { ChecklistItemModalComponent } from './checklist-item-modal/checklist-item-modal.component';
import { CheckListService } from './services/check-list.service';
import { ProductsService } from './services/products.service';
import { ProductsComponent } from './products/products.component';
import { ProductModalComponent } from './product-modal/product-modal.component';
import { ProductQrCodeModalComponent } from './product-qr-code-modal/product-qr-code-modal.component';
import { ActivityTypesService } from './services/activityTypes.service';
import { ActivityTypesComponent } from './activitytypes/activitytypes.component';
import { ActivityTypeModalComponent } from './activitytype-modal/activitytype-modal.component';
import { ProductTypesComponent } from './producttypes/producttypes.component';
import { ProductTypeModalComponent } from './producttype-modal/producttype-modal.component';
import { ProductTypesService } from './services/productTypes.service';
import { CustomerFiscalTypePipe } from './pipes/customer-fiscal-type.pipe';
import { JobsComponent } from './jobs/jobs.component';
import { JobsService } from './services/jobs/jobs.service';
import { JobModalComponent } from './jobs/job-modal.component';
import { JobStatusPipe } from './shared/pipes/job-status.pipe';
import { DropdownlistGridColumnFilterComponent } from './shared/dropdownlist-grid-column-filter.component';
import { MultiselectGridColumnFilterComponent } from './shared/multiselect-grid-column-filter.component';
import { ActivitiesComponent } from './activities/activities.component';
import { ActivitiesService } from './services/activities/activities.service';
import { ActivityComponent } from './activities/activity.component';
import { ActivityStatusPipe } from './shared/pipes/activity-status.pipe';
import { ActivityModalComponent } from './activities/activity-modal.component';
import { ActivityProductsComponent } from './activities/activity-products.component';
import { StringsPipe } from './shared/pipes/strings.pipe';
import { ActivityProductModalComponent } from './activities/activity-product-modal.component';
import { ActivityProductsService } from './services/activity-products/activity-products.service';
import { InterventionModalComponent } from './interventions/intervention-modal.component';
import { InterventionsService } from './services/interventions/interventions.service';
import { InterventionsCalendarComponent } from './interventions/interventions-calendar.component';
import { SchedulerModule } from '@progress/kendo-angular-scheduler';
import { OperatorAvatarComponent } from './shared/operator-avatar.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ContextMenuModule } from '@progress/kendo-angular-menu';
import { InterventionsComponent } from './interventions/interventions.component';
import { InterventionsGridComponent } from './interventions/interventions-grid.component';
import { InterventionStatusPipe } from './shared/pipes/intervention-status.pipe';
import { InterventionsListComponent } from './interventions/interventions-list.component';
import { BarcodesModule } from '@progress/kendo-angular-barcodes';
import { TicketsComponent } from './ticket/tickets.component';
import { TicketModalComponent } from './ticket/ticket-modal.component';
import { TicketsService } from './services/tickets/tickets.service';
import { TicketStatusPipe } from './shared/pipes/ticket-status.pipe';
import { SupplierModalComponent } from './supplier-modal/supplier-modal.component';
import { SuppliersComponent } from './suppliers/suppliers.component';
import { SupplierService } from './services/supplier.service';
import { UploadModule } from "@progress/kendo-angular-upload";
import { UploadInterceptor } from './services/interceptors/upload.iterceptor';
import { InterventionsProductsGridComponent } from './interventions/interventions-products-grid.component';
import { InterventionProductChecklistItemsModalComponent } from './interventions/intervention-product-checklist-items-modal.component';
import { JobCopyModalComponent } from './jobs/job-copy-modal.component';
import { PurchaseOrdersComponent } from './purchase-order/purchase-orders.component';
import { PurchaseOrderModalComponent } from './purchase-order/purchase-order-modal.component';
import { PurchaseOrderItemModalComponent } from './purchase-order/purchase-order-item-modal.component';
import { PurchaseOrdersService } from './services/purchase-orders/purchase-orders.service';
import { PurchaseOrderStatusPipe } from './shared/pipes/purchase-order-status.pipe';
import { RolePipe } from './pipes/role.pipe';
import { JobsCompletedComponent } from './jobs/jobs-completed.component';
import { JobsAttachmentsModalComponent } from './jobs/jobs-attachments-modal.component';
import { InterventionNotesModalComponent } from './interventions/intervention-notes-modal.component';
import { MessagesService } from './services/messages/messages.service';
import { MessageModalComponent } from './messages/message-modal.component';
import { JobDetailsComponent } from './jobs/job-details.component';
import { GalleryModalComponent } from './shared/gallery-modal.component';
import { GridContextMenuComponent } from './shared/grid-context-menu.component';
import { InterventionsKoComponent } from './interventions/interventions-ko.component';
import { ActivitiesFromProductComponent } from './activities/activities-from-product.component';
import { InterventionsSingleProductGridComponent } from './interventions/interventions-singleproduct-grid.component';

registerLocaleData(localeIt, 'it', localeExtraIt);

@NgModule({
    declarations: [
        BooleanPipe,
        RolePipe,
        AppComponent,
        LoaderComponent,
        ValidationMessageComponent,
        HomeComponent,
        UsersComponent,
        LoginComponent,
        LogoutComponent,
        UserModalComponent,
        MenuComponent,
        CustomersComponent,
        CustomerModalComponent,
        AddressModalComponent,
        AddressesModalComponent,
        VehiclesComponent,
        VehicleModalComponent,
        OperatorsComponent,
        OperatorModalComponent,
        OperatorDocumentsModalComponent,
        OperatorDocumentModalComponent,
        ChecklistComponent,
        ChecklistModalComponent,
        ChecklistItemModalComponent,
        ProductsComponent,
        ProductModalComponent,
        ProductQrCodeModalComponent,
        ActivityTypesComponent,
        ActivityTypeModalComponent,
        ProductTypesComponent,
        ProductTypeModalComponent,
        CustomerFiscalTypePipe,
        ActivityStatusPipe,
        JobsComponent,
        JobsCompletedComponent,
        JobModalComponent,
        JobCopyModalComponent,
        JobStatusPipe,
        JobDetailsComponent,
        DropdownlistGridColumnFilterComponent,
        MultiselectGridColumnFilterComponent,
        ActivitiesComponent,
        ActivitiesFromProductComponent,
        ActivityComponent,
        ActivityModalComponent,
        ActivityProductsComponent,
        JobsAttachmentsModalComponent,
        StringsPipe,
        ActivityProductModalComponent,
        InterventionModalComponent,
        InterventionNotesModalComponent,
        InterventionsCalendarComponent,
        OperatorAvatarComponent,
        InterventionsComponent,
        InterventionsGridComponent,
        InterventionStatusPipe,
        InterventionsListComponent,
        InterventionsProductsGridComponent,
        InterventionsSingleProductGridComponent,
        InterventionProductChecklistItemsModalComponent,
        InterventionsKoComponent,
        TicketsComponent,
        TicketStatusPipe,
        TicketModalComponent,
        SuppliersComponent,
        SupplierModalComponent,
        PurchaseOrdersComponent,
        PurchaseOrderModalComponent,
        PurchaseOrderItemModalComponent,
        PurchaseOrderStatusPipe,
        MessageModalComponent,
        GalleryModalComponent,
        GridContextMenuComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        FormsModule,
        IntlModule,
        AppRoutingModule,
        NotificationModule,
        GridModule,
        ContextMenuModule,
        DialogsModule,
        DropDownsModule,
        NumericTextBoxModule,
        DateInputsModule,
        TooltipsModule,
        DropDownButtonModule,
        SwitchModule,
        PDFExportModule,
        ExcelModule,
        InputsModule,
        PDFModule,
        UploadsModule,
        SchedulerModule,
        LayoutModule,
        BarcodesModule,
        EditorModule,
        UploadModule
    ],
    providers: [
        {
            provide: LOCALE_ID, useValue: 'it'
        },
        StorageService,
        UserService,
        VehiclesService,
        SecurityService,
        { provide: APP_INITIALIZER, useFactory: refreshUserData, multi: true, deps: [SecurityService, UserService] },
        { provide: HTTP_INTERCEPTORS, useClass: HeadersInterceptor, multi: true, deps: [UserService] },
        MessageBoxService,
        { provide: HTTP_INTERCEPTORS, useClass: ResponseInterceptor, multi: true, deps: [Router, UserService, MessageBoxService] },
        LoaderService,
        { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true, deps: [LoaderService] },
        
        { provide: HTTP_INTERCEPTORS, useClass: UploadInterceptor, multi: true },
        AuthGuard,
        CustomerService,
        SupplierService,
        AddressesService,
        OperatorsService,
        CheckListService,
        ProductsService,
        ActivityTypesService,
        ProductTypesService,
        JobsService,
        ActivitiesService,
        ActivityProductsService,
        InterventionsService,
        TicketsService,
        SupplierService,
        PurchaseOrdersService,
        MessagesService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
