import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { Router, NavigationEnd } from '@angular/router';
import { filter, tap, take } from 'rxjs/operators';
import { SecurityService } from '../services/security/security.service';
import { UserService } from '../services/security/user.service';
import { Role, User } from '../services/security/models';
import { fromEvent } from 'rxjs';

@Component({
    selector: 'lacos-menu',
    templateUrl: 'menu.component.html'
})
export class MenuComponent extends BaseComponent implements OnInit {

    readonly dropDownMenuEntry = DropDownMenuEntry;
    readonly menu: Menu = new Menu(
        [
            new DropDownMenuEntry('Anagrafiche', [
                new MenuEntry(['/users'], 'Utenti',
                    e => e.startsWith('/users'),
                    e => e.isAuthorized(Role.Administrator)
                ),
                new MenuEntry(['/customers'], 'Clienti',
                    e => e.startsWith('/customers'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/operators'], 'Operatori',
                    e => e.startsWith('/operators'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/checklist'], 'Checklist',
                    e => e.startsWith('/checklist'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/products'], 'Prodotti',
                    e => e.startsWith('/products'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/vehicles'], 'Mezzi di trasporto',
                    e => e.startsWith('/vehicles'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/activitytypes'], 'Tipologie Attività',
                    e => e.startsWith('/activitytypes'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/producttypes'], 'Tipi Prodotto',
                    e => e.startsWith('/producttypes'),
                    e => e.isAuthenticated()
                )
            ]),
            new MenuEntry(['/jobs'], 'Commesse',
                e => e.startsWith('/jobs'),
                e => e.isAuthenticated()
            ),
            new MenuEntry(['/tickets'], 'Tickets',
                e => e.startsWith('/tickets'),
                e => e.isAuthenticated()
            ),
            new DropDownMenuEntry('Interventi', [
                new MenuEntry(['/interventions'], 'Calendario',
                    e => e.startsWith('/interventions'),
                    e => e.isAuthenticated()
                ),
                new MenuEntry(['/interventions-list'], 'Elenco Interventi',
                    e => e.startsWith('/interventions-list'),
                    e => e.isAuthenticated()
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

    constructor(
        private readonly _router: Router,
        private readonly _security: SecurityService,
        private readonly _user: UserService
    ) {
        super();
    }

    ngOnInit() {
        this.user = this._user.getUser();
        this._subscribeRouterEvents();
        this._subscribeSecurityEvents();
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
