import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../shared/base.component';
import { ActivityStatusEnum } from '../shared/enums/activity-status.enum';
import { compileClassMetadata } from '@angular/compiler';
import { ContactFiscalTypeEnum } from '../shared/enums/contact-fiscal-type.enum';

@Pipe({
    name: 'contactFiscalType'
})
export class ContactFiscalTypePipe extends BaseComponent implements PipeTransform {

    transform(value: ContactFiscalTypeEnum) {
        switch (value) {
            case ContactFiscalTypeEnum.Company:
                return 'Azienda';
            case ContactFiscalTypeEnum.PrivatePerson:
                return 'Privato';
            default:
                return value;
        }
    }

}

