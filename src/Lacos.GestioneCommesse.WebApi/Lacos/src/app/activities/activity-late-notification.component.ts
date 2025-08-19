import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-activity-late-notification',
  templateUrl: 'activity-late-notification.component.html'
})

export class ActivityLateNotificationComponent {
  @Input() id!: number;
  @Input() jobCode!: string;
  @Input() shortDescription!: string;
  @Input() expirationDate!: string | Date;
  @Input() userId!: string;
  @Output() closed = new EventEmitter<number>();

  close() {
    // Salva nel localStorage la chiusura (userId + jobId)
    localStorage.setItem(this.getStorageKey(), 'closed');
    this.closed.emit(this.id);
  }

  getStorageKey(): string {
    return `activityLateClosed_${this.userId}_${this.id}`;
  }
}
