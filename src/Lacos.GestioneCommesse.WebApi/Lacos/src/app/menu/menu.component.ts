import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { Router, NavigationEnd } from '@angular/router';
import { filter, tap, take } from 'rxjs/operators';
import { SecurityService } from '../services/security/security.service';
import { UserService } from '../services/security/user.service';
import { Role, User } from '../services/security/models';
import { fromEvent } from 'rxjs';
import { TicketsService } from '../services/tickets/tickets.service';
import { TicketCounter } from '../services/tickets/models';
import { NewActivityCounter } from '../services/activities/models';
import { ActivitiesService } from '../services/activities/activities.service';
import { OperatorModel } from '../shared/models/operator.model';
import { OperatorsService } from '../services/operators.service';
import { MessagesService } from '../services/messages/messages.service';

@Component({
    selector: 'lacos-menu',
    templateUrl: 'menu.component.html',
    styleUrls: ['menu.component.scss']
})
export class MenuComponent extends BaseComponent implements OnInit {

    readonly dropDownMenuEntry = DropDownMenuEntry;
    readonly menu: Menu = new Menu(
        [
            new DropDownMenuEntry('Anagrafiche', [
                new MenuEntry(['/customers'], 'Clienti',
                    e => e.startsWith('/customers'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/suppliers'], 'Fornitori',
                    e => e.startsWith('/suppliers'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/checklist'], 'Checklist',
                    e => e.startsWith('/checklist'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/products'], 'Prodotti',
                    e => e.startsWith('/products'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/vehicles'], 'Mezzi di trasporto',
                    e => e.startsWith('/vehicles'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/activitytypes'], 'Tipologie Attività',
                    e => e.startsWith('/activitytypes'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/producttypes'], 'Tipi Prodotto',
                    e => e.startsWith('/producttypes'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/accountingtypes'], 'Voci Contabili',
                    e => e.startsWith('/accountingtypes'),
                    e => e.isAuthorized(Role.Administrator)
                )
            ]),
            new MenuEntry(['/tickets'], 'Tickets',
                e => e.startsWith('/tickets'),
                e => e.isAuthenticated()
            ),
            new DropDownMenuEntry('Commesse', [
                new MenuEntry(['/jobs'], 'Commesse',
                    e => e.startsWith('/jobs'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/jobs-completed'], 'Commesse Completate',
                    e => e.startsWith('/jobs-completed'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/jobs-suspended'], 'Commesse Sospese',
                    e => e.startsWith('/jobs-suspended'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/jobs-archive'], 'Archivio Storico Commesse',
                    e => e.startsWith('/jobs-archive'),
                    e => e.isAuthorized(Role.Administrator)
                )
            ]),
            new DropDownMenuEntry('Pianificazione', [
                new MenuEntry(['/jobs'], 'Commesse',
                    e => e.startsWith('/jobs'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/activities'], 'Attività',
                    e => e.startsWith('/activities'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/interventions'], 'Calendario',
                    e => e === '/interventions',
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/interventions-list'], 'Elenco Interventi',
                    e => e.startsWith('/interventions-list'),
                    e => e.isAuthorized(Role.Administrator)
                )
            ]),
            new MenuEntry(['/purchase-orders'], 'Ordini Acquisto',
                e => e.startsWith('/purchase-orders'),
                e => e.isAuthenticated()
            ),
            new DropDownMenuEntry('Utility Varie', [
                new MenuEntry(['/interventions-ko'], 'Interventi KO',
                    e => e.startsWith('/interventions-ko'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/jobs-progress-status'], 'Avanzamento Commesse',
                    e => e.startsWith('/jobs-progress-status'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/activities-from-product'], 'Ricerca Attività Prodotto',
                    e => e.startsWith('/activities-from-product'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/messages-list'], 'Commenti',
                    e => e.startsWith('/messages-list'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/interventionmessages-list'], 'Commenti Interventi da App',
                    e => e.startsWith('/interventionmessages-list'),
                    e => e.isAuthenticated()
                )
            ]),
            new DropDownMenuEntry('Gestione', [
                new MenuEntry(['/operators'], 'Operatori',
                    e => e.startsWith('/operators'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/users'], 'Utenti',
                    e => e.startsWith('/users'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/notification-operators'], 'Operatori Notifica',
                    e => e.startsWith('/notification-operators'),
                    e => e.isAuthorized(Role.Administrator)
                )
            ]),
            new DropDownMenuEntry('Help', [
                new MenuEntry(['/helperdocuments'], 'Guide utili',
                    e => e.startsWith('/helperdocuments'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/helpertypes'], 'Tipologie Guide',
                    e => e.startsWith('/helpertypes'),
                    e => e.isAuthorized(Role.Administrator)
                )
            ]),
            new MenuEntry(['/logout'], 'Logout',
                () => false,
                e => e.isAuthenticated()
            ),
            new MenuEntry(['/login'], 'Login',
                e => e.startsWith('/login'),
                e => !e.isAuthenticated()
            )
        ]
    );

    user: User;
    ticketsCounters: TicketCounter;
    newActivitiesCounter = new NewActivityCounter(0);
    currentOperator: OperatorModel;
    unreadCounter: number;
    private intervalId: number;

    constructor(
        private readonly _router: Router,
        private readonly _security: SecurityService,
        private readonly _user: UserService,
        private readonly _activityService: ActivitiesService,
        private readonly _ticketService: TicketsService,
        private readonly _messagesService: MessagesService,
        private readonly _operatorsService: OperatorsService
    ) {
        super();
    }

    ngOnInit() {
        this.ticketsCounters = new TicketCounter(0, 0);
        this.user = this._user.getUser();
        this._subscribeRouterEvents();
        this._subscribeSecurityEvents();
        this.intervalId = window.setInterval(() => {
            this._getTicketsCounters();
            this._getNewActivitiesCounter();
        }, 300000); // 300000 ms = 5 minuti
        this._getTicketsCounters();
        this._getNewActivitiesCounter();
        this._getCurrentOperator(this.user.id);
    }

    override ngOnDestroy(): void {
        if (this.intervalId) {
            clearInterval(this.intervalId);
        }
    }

    toggle(dropDown: DropDownMenuEntry) {
        if (dropDown.isOpen) {
            dropDown.close();
            return;
        }

        dropDown.open();

        setTimeout(() =>
            this._subscriptions.push(
                fromEvent(document, 'click')
                    .pipe(
                        take(1),
                        tap(() => dropDown.close())
                    )
                    .subscribe()
            )
        );
    }

    private _subscribeRouterEvents() {
        this._subscriptions.push(
            this._router.events
                .pipe(
                    filter(e => e instanceof NavigationEnd),
                    tap(e => this.menu.refresh((e as NavigationEnd).url, this._security))
                )
                .subscribe()
        );
    }

    private _subscribeSecurityEvents() {
        this._subscriptions.push(
            this._user.userChanged
                .pipe(
                    tap(e => this.user = e),
                    tap(() => this.menu.refresh(this._router.url, this._security))
                )
                .subscribe()
        );
    }

    private _getTicketsCounters() {
        this._subscriptions.push(
            this._ticketService.readTicketsCounters()
                .pipe(
                    tap(e => {
                        this.ticketsCounters = e;
                    })
                )
                .subscribe()
        );
    }

    private _getNewActivitiesCounter() {
        this._subscriptions.push(
            this._activityService.readNewActivitiesCounter()
                .pipe(
                    tap(e => {
                        this.newActivitiesCounter = e;
                    })
                )
                .subscribe()
        );
    }

    protected _getCurrentOperator(userId: number) {
        this._subscriptions.push(
            this._operatorsService.getOperatorByUserId(userId)
                .pipe(
                    tap(e => {
                        this.currentOperator = e;
                        this._getUnreadCounter();
                    })
                )
                .subscribe()
        );
    }

    _getUnreadCounter() {
        this._subscriptions.push(
            this._messagesService.getUnreadCounter(this.currentOperator.id)
                .pipe(
                    tap(e => {
                        this.unreadCounter = e;
                    })
                )
                .subscribe()
        );
    }
}

export class Menu {

    constructor(
        readonly entries: (MenuEntry | DropDownMenuEntry)[]
    ) {
    }

    refresh(url: string, security: SecurityService) {
        this.entries
            .forEach(e => e.refresh(url, security));
    }

}

export class MenuEntry {

    active: boolean;
    enabled: boolean;

    constructor(
        readonly urlCommands: any[],
        readonly name: string,
        protected readonly _isActive: (url: string) => boolean,
        protected readonly _isEnabled: (security: SecurityService) => boolean,
        readonly divider?: boolean
    ) {

    }

    refresh(url: string, security: SecurityService) {
        this.enabled = this._isEnabled(security);
        this.active = this._isActive(url);
    }
}

export class DropDownMenuEntry extends MenuEntry {

    readonly isDropDown = true;

    override active: boolean;
    override enabled: boolean;
    isOpen = false;

    constructor(
        name: string,
        readonly entries: MenuEntry[]
    ) {
        super([], name, () => entries.some(e => e.active), () => entries.some(e => e.enabled))
    }

    override refresh(url: string, security: SecurityService) {
        this.entries
            .forEach(e => e.refresh(url, security));

        super.refresh(url, security);
    }

    open() {
        this.isOpen = true;
    }

    close() {
        this.isOpen = false;
    }
}
