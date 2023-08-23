import { HttpClient } from '@angular/common/http';
import { UntypedFormControl, AbstractControl, NgForm, UntypedFormGroup, FormControl, FormGroup } from '@angular/forms';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import { map } from 'rxjs';

export function getToday() {
    const now = new Date();

    return new Date(now.getFullYear(), now.getMonth(), now.getDate());
}

export function markAsDirty(control: AbstractControl | NgForm) {
    if (control instanceof UntypedFormControl || control instanceof FormControl) {
        control.markAsDirty();
        return;
    }

    if (control instanceof NgForm || control instanceof UntypedFormGroup || control instanceof FormGroup) {
        Object.keys(control.controls)
            .forEach(e => markAsDirty(control.controls[e]));
    }
}

export function listEnum<T>(x: any) {
    return Object.keys(x)
        .map(e => +e)
        .filter(e => e >= 0) as any as T[];
}

export function readData(http: HttpClient, state: State, url: string) {
    const params = toDataSourceRequestString(state);
    const hasGroups = !!state.group?.length;

    return http.get<GridDataResult>(`${url}?${params}`)
        .pipe(
            map(e =>
                <GridDataResult>{
                    data: hasGroups ? translateDataSourceResultGroups(e.data) : e.data,
                    total: e.total
                }
            )
        );
}
