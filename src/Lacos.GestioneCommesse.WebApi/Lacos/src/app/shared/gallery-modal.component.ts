import { Component, Input } from '@angular/core';
import { ModalComponent } from './modal.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-gallery-modal',
  templateUrl: `gallery-modal.component.html`,
  styles: [`
    .img-fluid {
      max-width: 100%;
      height: auto;
    }
  `]
})

export class GalleryModalComponent extends ModalComponent<GalleryModalInput> {

  currentIndex: number;

  override open(options: GalleryModalInput): Observable<boolean> {
    this.currentIndex = options.selectedImage;
    return super.open(options);

  }

  protected override _canClose(): boolean {
    return true;
  }

  next(): void {
    this.currentIndex = (this.currentIndex + 1) % this.options.images.length;
  }

  prev(): void {
    this.currentIndex = (this.currentIndex - 1 + this.options.images.length) % this.options.images.length;
  }
}

export class GalleryModalInput {
  constructor(
    readonly images: string[],
    readonly selectedImage: number
  ) {

  }
}