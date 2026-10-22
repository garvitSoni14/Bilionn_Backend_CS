import { maxLength, pattern, required, SchemaPathTree } from '@angular/forms/signals';

// 1. DATA shape (the image file is handled separately, files aren't form values)
export interface CategoryData {
  name: string;
  description: string;
  bgColor: string; // '#rrggbb'
}

// Matches backend CategoryResponse
export interface CategoryResponse {
  categoryId: string;
  name: string;
  description: string;
  bgColor: string;
  imgUrl: string;
  items: number;
  createdAt: string;
  updatedAt: string;
}

// 2. Empty starting data
export function createCategoryData(): CategoryData {
  return { name: '', description: '', bgColor: '#a6b94c' };
}

// 3. RULES
export function validateCategory(path: SchemaPathTree<CategoryData>): void {
  required(path.name, { message: 'Category name is required.' });
  maxLength(path.name, 60, { message: 'Name is too long (max 60).' });

  maxLength(path.description, 250, { message: 'Description is too long (max 250).' });

  required(path.bgColor, { message: 'Pick a colour for this category.' });
  pattern(path.bgColor, /^#[0-9a-fA-F]{6}$/, { message: 'Colour must look like #A6B94C.' });
}
