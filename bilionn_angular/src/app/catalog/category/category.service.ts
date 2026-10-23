import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CategoryData, CategoryResponse } from './category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly host = 'http://localhost:5109';
  private readonly apiUrl = `${this.host}/api`;

  list(): Observable<CategoryResponse[]> {
    return this.http.get<CategoryResponse[]>(`${this.apiUrl}/categories`);
  }

  // Backend expects multipart/form-data with @ModelAttribute CategoryRequest
  // fields: name, description, bgColor, img.  Do NOT set Content-Type manually.
  create(data: CategoryData, image: File | null): Observable<CategoryResponse> {
    const body = new FormData();
    body.append('name', data.name.trim());
    body.append('description', data.description.trim());
    body.append('bgColor', data.bgColor);
    if (image) body.append('img', image, image.name);
    return this.http.post<CategoryResponse>(`${this.apiUrl}/admin/categories`, body);
  }

  delete(categoryId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/admin/categories/${categoryId}`);
  }

  /** imgUrl from the API is relative ("/api/v1.0/categories/{id}/image") */
  imageSrc(imgUrl: string | null | undefined): string {
    return imgUrl ? `${this.host}${imgUrl}` : '';
  }
}
