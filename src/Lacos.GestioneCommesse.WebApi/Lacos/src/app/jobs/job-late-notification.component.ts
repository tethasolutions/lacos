import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-job-late-notification',
  templateUrl: 'job-late-notification.component.html'
})

export class JobLateNotificationComponent {
  @Input() jobId!: number;
  @Input() jobCode!: string;
  @Input() mandatoryDate!: string | Date;
  @Input() userId!: string;
  @Output() closed = new EventEmitter<number>();

  close() {
    // Salva nel localStorage la chiusura (userId + jobId)
    localStorage.setItem(this.getStorageKey(), 'closed');
    this.closed.emit(this.jobId);
  }

  getStorageKey(): string {
    return `jobLateClosed_${this.userId}_${this.jobId}`;
  }
}
