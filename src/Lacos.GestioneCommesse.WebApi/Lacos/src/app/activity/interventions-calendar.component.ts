import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../shared/base.component';

@Component({
    selector: 'app-interventions-calendar',
    templateUrl: 'interventions-calendar.component.html'
})
export class InterventionsCalendarComponent extends BaseComponent implements OnInit {

    constructor() {
        super();
    }

    ngOnInit() { }

}
