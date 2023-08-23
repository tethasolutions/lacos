import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { BaseComponent } from './base.component';
import { Dictionary, KeyValuePair } from '../services/common/models';
import { ColumnComponent, FilterService } from '@progress/kendo-angular-grid';
import { FilterDescriptor } from '@progress/kendo-data-query';

@Component({
    selector: 'app-multiselect-grid-column-filter',
    templateUrl: 'multiselect-grid-column-filter.component.html'
})
export class MultiselectGridColumnFilterComponent<TKey extends string | number, TValue> extends BaseComponent implements OnChanges {

    @Input('data')
    inputData: Dictionary<TKey, TValue>;

    @Input()
    filterService: FilterService;

    @Input()
    column: ColumnComponent;

    @Input()
    filter: { filters: FilterDescriptor[] };

    data: KeyValuePair<TKey, TValue>[];
    value: TKey[];

    constructor() {
        super();
    }

    ngOnChanges(changes: SimpleChanges) {
        if ((changes['column'] || changes['inputData']) && this.column && this.inputData) {
            const keyType = this.column.filter === 'numeric'
                ? 'number'
                : 'string';

            this.data = KeyValuePair.fromDictionary(this.inputData, keyType);

            const currentColumnFilters = this.filter.filters
                .filter(e => e.field === this.column.field);

            if (currentColumnFilters.length) {
                this.value = currentColumnFilters.map(e => e.value);
            }
        }
    }

    onValueChange(value: TKey[]) {
        this.filterService.filter({
            filters: value.map(e => ({ field: this.column.field, operator: 'eq', value: e })),
            logic: 'or',
        });
    }

}
