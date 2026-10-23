import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CashierService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5109/api';

  createCashier(payload: unknown): Observable<unknown> {
    // return this.http.post(`${this.apiUrl}/admin/cashiers`, payload, { withCredentials: true });
        return this.http.post(`${this.apiUrl}/admin/cashiers`, payload);

  }
}