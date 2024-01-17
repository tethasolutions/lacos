import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { CustomerFiscalTypeEnum } from '../shared/enums/customer-fiscal-type.enum';

@Pipe({
    name: 'customerFiscalType'
})
export class CustomerFiscalTypePipe extends BaseComponent implements PipeTransform {

    transform(value: CustomerFiscalTypeEnum) {
        switch (value) {
            case CustomerFiscalTypeEnum.Company:
                return 'Azienda';
            case CustomerFiscalTypeEnum.PrivatePerson:
                return 'Privato';
            case CustomerFiscalTypeEnum.PublicAdministration:
                return 'Pubblica Amministrazione';
            case CustomerFiscalTypeEnum.Foreign:
                return 'Estero';
            default:
                return value;
        }
    }

}

