import { maxLength, pattern, required, validate, SchemaPathTree } from '@angular/forms/signals';

export interface ItemData {
  name: string;
  price: string; // string in the input, converted to a plain value when sent
  categoryId: string;
  description: string;
}

// Matches backend ItemResponse
export interface ItemResponse {
  itemId: string;
  name: string;
  price: number;
  categoryId: string;
  categoryName: string;
  description: string;
  imgUrl: string;
  createdAt: string;
  updatedAt: string;
}

export function createItemData(): ItemData {
  return { name: '', price: '', categoryId: '', description: '' };
}

export function validateItem(path: SchemaPathTree<ItemData>): void {
  required(path.name, { message: 'Item name is required.' });
  maxLength(path.name, 100, { message: 'Name is too long (max 100).' });

  required(path.price, { message: 'Price is required.' });
  pattern(path.price, /^\d{1,8}(\.\d{1,2})?$/, {
    message: 'Enter a valid price (max 2 decimals).',
  });
  validate(path.price, ({ value }) => {
    const v = value();
    if (v && Number(v) <= 0) {
      return { kind: 'min', message: 'Price must be greater than 0.' };
    }
    return null;
  });

  required(path.categoryId, { message: 'Please choose a category.' });

  maxLength(path.description, 500, { message: 'Description is too long (max 500).' });
}
