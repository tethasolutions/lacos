import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { Role } from '../services/security/models';

@Pipe({
    name: 'role'
})
export class RolePipe extends BaseComponent implements PipeTransform {

    transform(value: Role) {
        switch (value) {
            case Role.Administrator:
                return 'Amministratore';
            case Role.Operator:
                return 'Operatore';
            case Role.Customer:
                return 'Cliente';
            default:
                return value;
        }
    }
}

