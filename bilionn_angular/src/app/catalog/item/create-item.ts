import { Component, HostListener, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { FormField, form, submit } from '@angular/forms/signals';
import { MatFormField, MatLabel, MatPrefix } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { firstValueFrom } from 'rxjs';

import { CatalogShell } from '../shared/catalog-shell/catalog-shell';
import { ImagePicker } from '../shared/image-picker';
import { ValidationMessage } from '../shared/validation-message';
import { CategoryService } from '../category/category.service';
import { CategoryResponse } from '../category/category.model';
import { ItemService } from './item.service';
import { ItemData, ItemResponse, createItemData, validateItem } from './item.model';

@Component({
  selector: 'app-create-item',
  imports: [
    RouterLink,
    FormField,
    MatFormField,
    MatLabel,
    MatPrefix,
    MatInput,
    CatalogShell,
    ImagePicker,
    ValidationMessage,
  ],
  templateUrl: './create-item.html',
  styleUrl: '../shared/catalog-page.css',
})
export class CreateItem {
  private readonly itemService = inject(ItemService);
  private readonly categoryService = inject(CategoryService);

  readonly todayLabel = new Intl.DateTimeFormat('en-IN', {
    weekday: 'long', month: 'long', day: 'numeric', year: 'numeric',
  }).format(new Date());

  // ---- form (signal forms) ----
  readonly model = signal<ItemData>(createItemData());
  readonly itemForm = form(this.model, (path) => validateItem(path));
  readonly image = signal<File | null>(null);

  // ---- ui state ----
  readonly submitting = signal(false);
  readonly successMessage = signal('');
  readonly errorMessage = signal('');
  readonly adminLoginRequired = signal(false);

  // ---- lookups + list ----
  readonly categories = signal<CategoryResponse[]>([]);
  readonly categoriesLoaded = signal(false);
  readonly items = signal<ItemResponse[]>([]);
  readonly listLoading = signal(true);
  readonly listError = signal('');
  readonly deletingId = signal<string | null>(null);

  readonly noCategories = computed(() => this.categoriesLoaded() && this.categories().length === 0);

  constructor() {
    this.loadCategories();
    this.loadItems();
  }

  loadCategories(): void {
    this.categoryService.list().subscribe({
      next: (rows) => {
        this.categories.set(rows ?? []);
        this.categoriesLoaded.set(true);
      },
      error: () => this.categoriesLoaded.set(true),
    });
  }

  loadItems(): void {
    this.listLoading.set(true);
    this.listError.set('');
    this.itemService.list().subscribe({
      next: (rows) => {
        this.items.set(rows ?? []);
        this.listLoading.set(false);
      },
      error: () => {
        this.listError.set('Could not load items. Check that the backend is running.');
        this.listLoading.set(false);
      },
    });
  }

  async onSubmit(event: SubmitEvent): Promise<void> {
    event.preventDefault();
    this.successMessage.set('');
    this.errorMessage.set('');
    this.adminLoginRequired.set(false);

    await submit(this.itemForm, async () => {
      this.submitting.set(true);
      try {
        const created = await firstValueFrom(this.itemService.create(this.model(), this.image()));
        this.successMessage.set(`Item “${created.name}” added to ${created.categoryName}.`);
        this.itemForm().reset(createItemData());
        this.image.set(null);
        this.loadItems();
        this.loadCategories(); // keeps counts fresh
      } catch (error) {
        this.errorMessage.set(this.describe(error, 'We could not create the item. Check the details and try again.'));
      } finally {
        this.submitting.set(false);
      }
      return null;
    });
  }

  async remove(item: ItemResponse): Promise<void> {
    if (!confirm(`Delete item “${item.name}”?`)) return;
    this.successMessage.set('');
    this.errorMessage.set('');
    this.deletingId.set(item.itemId);
    try {
      await firstValueFrom(this.itemService.delete(item.itemId));
      this.items.update((rows) => rows.filter((i) => i.itemId !== item.itemId));
      this.successMessage.set(`Item “${item.name}” deleted.`);
    } catch (error) {
      this.errorMessage.set(this.describe(error, 'Could not delete this item.'));
    } finally {
      this.deletingId.set(null);
    }
  }

  currency(value: number | null | undefined): string {
    return new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 2 }).format(Number(value) || 0);
  }

  imageSrc(url: string): string {
    return this.itemService.imageSrc(url);
  }

  hideBrokenImage(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }

  hasUnsavedChanges(): boolean {
    return (this.itemForm().dirty() || this.image() !== null) && !this.submitting();
  }

  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: BeforeUnloadEvent): void {
    if (this.hasUnsavedChanges()) event.preventDefault();
  }

  private describe(error: unknown, fallback: string): string {
    const http = error as HttpErrorResponse;
    if (http.status === 401 || http.status === 403) {
      this.adminLoginRequired.set(true);
      return 'Sign in as an administrator in this browser, then try again.';
    }
    if (http.status === 0) return 'Could not reach the server. Check that the backend is running.';
    if (http.status === 413) return 'The image is too large for the server.';
    return http.error?.message ?? http.error?.detail ?? fallback;
  }
}
