import { Component, computed, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../interceptors/auth-interceptor/auth.service';

interface OrderItem {
  itemId: string;
  name: string;
  price: number;
  quantity: number;
}

interface Order {
  orderId: string;
  customerName: string;
  grandTotal: number;
  paymentMethod: string;
  createdAt: string;
  items: OrderItem[];
  paymentDetails?: { status?: string };
}

interface DashboardSummary {
  todaySales: number;
  todayOrderCount: number;
  recentOrders: Order[];
}

interface Breakdown {
  name: string;
  count: number;
  amount: number;
  percent: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './dashboard.css',
})
export class Dashboard {
  private readonly http = inject(HttpClient);     
  private readonly router = inject(Router);
  readonly auth = inject(AuthService);
  private readonly apiUrl = 'http://localhost:5109/api/';

  readonly summary = signal<DashboardSummary | null>(null);
  readonly orders = signal<Order[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal('');
  readonly todayLabel = new Intl.DateTimeFormat('en-IN', {
    weekday: 'long', month: 'long', day: 'numeric', year: 'numeric',
  }).format(new Date());
  readonly isAdmin = computed(() => this.auth.currentUser()?.role?.toUpperCase().includes('ADMIN') ?? false);

  readonly dailySales = computed(() => {
    const rows: { label: string; dateKey: string; amount: number }[] = [];
    const totals = new Map<string, number>();
    for (let offset = 6; offset >= 0; offset -= 1) {
      const day = new Date();
      day.setDate(day.getDate() - offset);
      const key = `${day.getFullYear()}-${day.getMonth()}-${day.getDate()}`;
      rows.push({ label: new Intl.DateTimeFormat('en-IN', { weekday: 'short' }).format(day), dateKey: key, amount: 0 });
      totals.set(key, 0);
    }
    for (const order of this.orders()) {
      const date = new Date(order.createdAt);
      if (Number.isNaN(date.getTime())) continue;
      const key = `${date.getFullYear()}-${date.getMonth()}-${date.getDate()}`;
      if (totals.has(key)) totals.set(key, (totals.get(key) ?? 0) + this.number(order.grandTotal));
    }
    return rows.map((row) => ({ ...row, amount: totals.get(row.dateKey) ?? 0 }));
  });
  readonly maxDailySales = computed(() => Math.max(1, ...this.dailySales().map((day) => day.amount)));
  readonly totalSales = computed(() => this.orders().reduce((sum, order) => sum + this.number(order.grandTotal), 0));
  readonly averageOrder = computed(() => this.orders().length ? this.totalSales() / this.orders().length : 0);

  readonly paymentBreakdown = computed<Breakdown[]>(() => {
    const totals = new Map<string, { count: number; amount: number }>();
    for (const order of this.orders()) {
      const method = this.label(order.paymentMethod || 'OTHER');
      const current = totals.get(method) ?? { count: 0, amount: 0 };
      current.count += 1;
      current.amount += this.number(order.grandTotal);
      totals.set(method, current);
    }
    const totalAmount = [...totals.values()].reduce((sum, row) => sum + row.amount, 0);
    return [...totals.entries()].map(([name, row]) => ({
      name, ...row, percent: totalAmount ? Math.round((row.amount / totalAmount) * 100) : 0,
    }));
  });
  readonly paymentGradient = computed(() => {
    const colors = ['#263c30', '#a6b94c', '#d8a85f', '#d8826d', '#8b9ca0'];
    const values = this.paymentBreakdown();
    if (!values.length || !this.totalSales()) return 'conic-gradient(#e8ebe4 0 100%)';
    let cursor = 0;
    const sections = values.map((entry, index) => {
      const start = cursor;
      cursor += (entry.amount / this.totalSales()) * 100;
      return `${colors[index % colors.length]} ${start}% ${cursor}%`;
    });
    return `conic-gradient(${sections.join(', ')})`;
  });
  readonly statusBreakdown = computed<Breakdown[]>(() => {
    const counts = new Map<string, number>();
    for (const order of this.orders()) {
      const status = this.label(order.paymentDetails?.status || 'UNKNOWN');
      counts.set(status, (counts.get(status) ?? 0) + 1);
    }
    const total = this.orders().length;
    return [...counts.entries()].map(([name, count]) => ({
      name, count, amount: 0, percent: total ? Math.round((count / total) * 100) : 0,
    }));
  });
  readonly topProducts = computed(() => {
    const quantities = new Map<string, number>();
    for (const order of this.orders()) {
      for (const item of order.items ?? []) quantities.set(item.name, (quantities.get(item.name) ?? 0) + this.number(item.quantity));
    }
    return [...quantities.entries()].map(([name, quantity]) => ({ name, quantity }))
      .sort((a, b) => b.quantity - a.quantity).slice(0, 5);
  });
  readonly maxProductQuantity = computed(() => Math.max(1, ...this.topProducts().map((product) => product.quantity)));

  constructor() {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    forkJoin({
      // summary: this.http.get<DashboardSummary>(`${this.apiUrl}/dashboard`, { withCredentials: true }),
            summary: this.http.get<DashboardSummary>(`${this.apiUrl}/dashboard`),

      // orders: this.http.get<Order[]>(`${this.apiUrl}/orders/latest`, { withCredentials: true }),
            orders: this.http.get<Order[]>(`${this.apiUrl}/orders/latest`),

    }).subscribe({
      next: ({ summary, orders }) => {
        this.summary.set(summary);
        this.orders.set(orders ?? []);
        this.loading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(error.status === 0
          ? 'Could not connect to the store server. Check that the backend is running.'
          : error.status === 401 || error.status === 403
            ? 'Your session may have expired. Sign in again to view the dashboard.'
            : 'Dashboard data could not be loaded. Please try again.');
        this.loading.set(false);
      },
    });
  }

  signOut(): void {
    this.auth.logout().subscribe({
      next: () => void this.router.navigateByUrl('/login'),
      error: () => void this.router.navigateByUrl('/login'),
    });
  }

  currency(value: number | null | undefined): string {
    return new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 0 }).format(this.number(value));
  }

  barHeight(value: number, max: number): number {
    return value <= 0 ? 4 : Math.max(10, (value / max) * 100);
  }

  productWidth(value: number): number {
    return Math.max(6, (value / this.maxProductQuantity()) * 100);
  }

  orderTime(value: string): string {
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? '—' : new Intl.DateTimeFormat('en-IN', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' }).format(date);
  }

  shortOrderId(value: string): string {
    return value?.length > 12 ? `${value.slice(0, 12)}…` : value;
  }

  label(value: string): string {
    return value.toLowerCase().replaceAll('_', ' ').replace(/\b\w/g, (letter) => letter.toUpperCase());
  }

  private number(value: unknown): number {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
  }
}
