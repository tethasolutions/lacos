import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../base.component';
import { TicketStatus, ticketStatusNames } from 'src/app/services/tickets/models';

@Pipe({
    name: 'ticketStatus'
})
export class TicketStatusPipe extends BaseComponent implements PipeTransform {

    transform(value: TicketStatus) {
        return ticketStatusNames[value];
    }

}
