import { Pipe, PipeTransform } from '@angular/core';
import { BaseComponent } from '../base.component';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({
    name: 'domsanitizer'
})
export class DomSanitizerPipe extends BaseComponent implements PipeTransform {

    constructor(private sanitizer: DomSanitizer) {
        super();
        
    }
    transform(value: string) {
        return this.sanitizer.bypassSecurityTrustHtml(value);
    }

}
