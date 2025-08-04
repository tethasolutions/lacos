import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-jobid-selector-dialog',
  templateUrl: './jobid-selector-dialog.component.html'
})
export class JobIdSelectorDialogComponent {
  @Input() jobIds: any[] = [];
  @Output() select = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();

  onSelect(jobId: any) {
    this.select.emit(jobId);
  }

  onCancel() {
    this.cancel.emit();
  }
}
