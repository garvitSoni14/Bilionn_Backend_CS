import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ItemData, ItemResponse } from './item.model';

@Injectable({ providedIn: 'root' })
export class ItemService {
  private readonly http = inject(HttpClient);
  private readonly host = 'http://localhost:5109';
  private readonly apiUrl = `${this.host}/api`;

  list(): Observable<ItemResponse[]> {
    return this.http.get<ItemResponse[]>(`${this.apiUrl}/items`);
  }

  // multipart/form-data -> @ModelAttribute ItemRequest (name, price, categoryId, description, image)
  create(data: ItemData, image: File | null): Observable<ItemResponse> {
    const body = new FormData();
    body.append('name', data.name.trim());
    body.append('price', String(Number(data.price)));
    body.append('categoryId', data.categoryId);
    body.append('description', data.description.trim());
    if (image) body.append('image', image, image.name);
    return this.http.post<ItemResponse>(`${this.apiUrl}/admin/items`, body);
  }

  delete(itemId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/admin/items/${itemId}`);
  }

  imageSrc(imgUrl: string | null | undefined): string {
    return imgUrl ? `${this.host}${imgUrl}` : '';
  }
}
