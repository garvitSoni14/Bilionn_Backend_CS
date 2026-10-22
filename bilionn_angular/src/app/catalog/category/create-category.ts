import { Component, HostListener, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { FormField, form, submit } from '@angular/forms/signals';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { firstValueFrom } from 'rxjs';

import { CatalogShell } from '../shared/catalog-shell/catalog-shell';
import { ImagePicker } from '../shared/image-picker';
import { ValidationMessage } from '../shared/validation-message';
import { CategoryService } from './category.service';
import {
  CategoryResponse,
  createCategoryData,
  CategoryData,
  validateCategory,
} from './category.model';

@Component({
  selector: 'app-create-category',
  imports: [
    RouterLink,
    FormField,
    MatFormField,
    MatLabel,
    MatInput,
    CatalogShell,
    ImagePicker,
    ValidationMessage,
  ],
  templateUrl: './create-category.html',
  styleUrl: '../shared/catalog-page.css'
})
export class CreateCategory {
  private readonly categoryService = inject(CategoryService);

  readonly presetColors = ['#a6b94c', '#d8a85f', '#d8826d', '#8b9ca0', '#84977d', '#c6b778', '#9db7d6', '#cfa7c8'];
  readonly todayLabel = new Intl.DateTimeFormat('en-IN', {
    weekday: 'long', month: 'long', day: 'numeric', year: 'numeric',
  }).format(new Date());

  // ---- form (signal forms) ----
  readonly model = signal<CategoryData>(createCategoryData());
  readonly categoryForm = form(this.model, (path) => validateCategory(path));
  readonly image = signal<File | null>(null);

  // ---- ui state ----
  readonly submitting = signal(false);
  readonly successMessage = signal('');
  readonly errorMessage = signal('');
  readonly adminLoginRequired = signal(false);

  // ---- existing categories ----
  readonly categories = signal<CategoryResponse[]>([]);
  readonly listLoading = signal(true);
  readonly listError = signal('');
  readonly deletingId = signal<string | null>(null);

  constructor() {
    this.loadCategories();
  }

  loadCategories(): void {
    this.listLoading.set(true);
    this.listError.set('');
    this.categoryService.list().subscribe({
      next: (rows) => {
        this.categories.set(rows ?? []);
        this.listLoading.set(false);
      },
      error: () => {
        this.listError.set('Could not load categories. Check that the backend is running.');
        this.listLoading.set(false);
      },
    });
  }

  pickColor(color: string): void {
    const field = this.categoryForm.bgColor();
    field.value.set(color);
    field.markAsDirty();
  }

  async onSubmit(event: SubmitEvent): Promise<void> {
    event.preventDefault();
    this.successMessage.set('');
    this.errorMessage.set('');
    this.adminLoginRequired.set(false);

    await submit(this.categoryForm, async () => {
      this.submitting.set(true);
      try {
        const created = await firstValueFrom(
          this.categoryService.create(this.model(), this.image()),
        );
        this.successMessage.set(`Category “${created.name}” created successfully.`);
        this.categoryForm().reset(createCategoryData());
        this.image.set(null);
        this.loadCategories();
      } catch (error) {
        this.errorMessage.set(this.describe(error, 'We could not create the category. Check the details and try again.'));
      } finally {
        this.submitting.set(false);
      }
      return null;
    });
  }

  async remove(category: CategoryResponse): Promise<void> {
    const warning = category.items > 0
      ? `“${category.name}” has ${category.items} item(s). Delete it anyway?`
      : `Delete category “${category.name}”?`;
    if (!confirm(warning)) return;

    this.successMessage.set('');
    this.errorMessage.set('');
    this.deletingId.set(category.categoryId);
    try {
      await firstValueFrom(this.categoryService.delete(category.categoryId));
      this.categories.update((rows) => rows.filter((c) => c.categoryId !== category.categoryId));
      this.successMessage.set(`Category “${category.name}” deleted.`);
    } catch (error) {
      this.errorMessage.set(this.describe(error, 'Could not delete this category. It may still be used by items.'));
    } finally {
      this.deletingId.set(null);
    }
  }

  imageSrc(url: string): string {
    return this.categoryService.imageSrc(url);
  }

  hideBrokenImage(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }

  // used by unsavedChangesGuard
  hasUnsavedChanges(): boolean {
    return (this.categoryForm().dirty() || this.image() !== null) && !this.submitting();
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
