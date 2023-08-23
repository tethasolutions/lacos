import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { BaseComponent } from './base.component';
import { Dictionary, KeyValuePair } from '../services/common/models';
import { ColumnComponent, FilterService } from '@progress/kendo-angular-grid';
import { FilterDescriptor } from '@progress/kendo-data-query';

@Component({
    selector: 'app-dropdownlist-grid-column-filter',
    templateUrl: 'dropdownlist-grid-column-filter.component.html'
})
export class DropdownlistGridColumnFilterComponent<TKey extends string | number, TValue> extends BaseComponent implements OnChanges {

    @Input('data')
    inputData: Dictionary<TKey, TValue>;

    @Input()
    filterService: FilterService;

    @Input()
    column: ColumnComponent;

    @Input()
    filter: { filters: FilterDescriptor[] };

    data: KeyValuePair<TKey, TValue>[];
    value: TKey;

    constructor() {
        super();
    }

    ngOnChanges(changes: SimpleChanges) {
        if ((changes['column'] || changes['inputData']) && this.column && this.inputData) {
            const keyType = this.column.filter === 'numeric'
                ? 'number'
                : 'string';

            this.data = KeyValuePair.fromDictionary(this.inputData, keyType);

            const currentColumnFilter = this.filter.filters
                .find(e => e.field === this.column.field);

            if (currentColumnFilter) {
                this.value = currentColumnFilter.value;
            }
        }
    }

    onValueChange(value: string) {
        this.filterService.filter({
            filters: [{ field: this.column.field, operator: 'eq', value: value }],
            logic: 'and',
        });
    }

}
