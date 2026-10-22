import { Component, ElementRef, computed, effect, input, model, signal, viewChild } from '@angular/core';

/**
 * File picker with preview. The File itself is NOT part of the signal form
 * (files aren't form values), so the parent two-way binds it: [(file)]="image".
 */
@Component({
  selector: 'app-image-picker',
  template: `
    <div class="picker">
      <span class="picker-label">{{ label() }}</span>
      <div class="picker-box" [class.has-error]="error()">
        <div class="picker-preview">
          @if (previewUrl(); as url) {
            <img [src]="url" alt="Selected image preview" />
          } @else {
            <span class="picker-placeholder">▣</span>
          }
        </div>
        <div class="picker-actions">
          <label class="picker-button">
            Choose image
            <input #fileInput type="file" accept="image/*" hidden (change)="onPick($event)" />
          </label>
          @if (file()) {
            <button type="button" class="picker-clear" (click)="clear()">Remove</button>
          }
          <small class="picker-hint">{{ file()?.name || hint() }}</small>
        </div>
      </div>
      @if (error()) {
        <small class="picker-error" role="alert">{{ error() }}</small>
      }
    </div>
  `,
  styles: `
    .picker { display: grid; gap: 7px; margin-bottom: 18px; }
    .picker-label { color: #7b8272; font-size: 11px; font-weight: 600; }
    .picker-box { display: flex; align-items: center; gap: 14px; padding: 12px; border: 1px dashed #cfd5c3; border-radius: 10px; background: #fbfcf8; }
    .picker-box.has-error { border-color: #cf7968; }
    .picker-preview { display: grid; flex: 0 0 74px; width: 74px; height: 74px; place-items: center; overflow: hidden; border-radius: 9px; background: #edf1df; }
    .picker-preview img { width: 100%; height: 100%; object-fit: cover; }
    .picker-placeholder { color: #aab57a; font-size: 26px; }
    .picker-actions { display: grid; gap: 6px; min-width: 0; justify-items: start; }
    .picker-button { padding: 8px 12px; border-radius: 6px; background: #293d30; color: #fff; cursor: pointer; font-size: 11px; font-weight: 600; }
    .picker-button:hover { background: #3c4b36; }
    .picker-clear { padding: 0; border: 0; background: none; color: #b56b5d; cursor: pointer; font: inherit; font-size: 10px; }
    .picker-hint { max-width: 220px; overflow: hidden; color: #9a9e92; font-size: 10px; text-overflow: ellipsis; white-space: nowrap; }
    .picker-error { color: #b56b5d; font-size: 11px; }
  `,
})
export class ImagePicker {
  readonly label = input('Image (optional)');
  readonly hint = input('PNG or JPG, up to 1 MB');
  readonly maxMb = input(1);

  /** two-way bound from the parent: [(file)]="image" */
  readonly file = model<File | null>(null);

  readonly error = signal('');
  readonly previewUrl = signal<string | null>(null);

  private readonly fileInput = viewChild<ElementRef<HTMLInputElement>>('fileInput');
  readonly maxBytes = computed(() => this.maxMb() * 1024 * 1024);

  constructor() {
    // build / revoke the object URL whenever the file changes
    effect((onCleanup) => {
      const f = this.file();
      if (!f) {
        this.previewUrl.set(null);
        const el = this.fileInput()?.nativeElement;
        if (el) el.value = ''; // parent reset the form -> clear the native input too
        return;
      }
      const url = URL.createObjectURL(f);
      this.previewUrl.set(url);
      onCleanup(() => URL.revokeObjectURL(url));
    });
  }

  onPick(event: Event): void {
    const input = event.target as HTMLInputElement;
    const picked = input.files?.[0] ?? null;
    this.error.set('');
    if (!picked) return;

    if (!picked.type.startsWith('image/')) {
      this.error.set('Please choose an image file (PNG, JPG, WEBP…).');
      input.value = '';
      return;
    }
    if (picked.size > this.maxBytes()) {
      this.error.set(`Image is too large (max ${this.maxMb()} MB).`);
      input.value = '';
      return;
    }
    this.file.set(picked);
  }

  clear(): void {
    this.error.set('');
    this.file.set(null);
  }
}
