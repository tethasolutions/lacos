import { UntypedFormControl, AbstractControl, NgForm, UntypedFormGroup, FormControl, FormGroup } from '@angular/forms';

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
