import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { ActivityStatusEnum } from '../shared/enums/activity-status.enum';
import { compileClassMetadata } from '@angular/compiler';
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
            default:
                return value;
        }
    }

}

