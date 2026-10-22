import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../interceptors/auth-interceptor/auth.service';

/**
 * Sidebar + main area, reused by the Category and Item pages.
 * Usage: <app-catalog-shell> ...page content... </app-catalog-shell>
 */
@Component({
  selector: 'app-catalog-shell',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './catalog-shell.html',
  styleUrl: './catalog-shell.css',
})
export class CatalogShell {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly isAdmin = computed(
    () => this.auth.currentUser()?.role?.toUpperCase().includes('ADMIN') ?? false,
  );

  signOut(): void {
    this.auth.logout().subscribe({
      next: () => void this.router.navigateByUrl('/login'),
      error: () => void this.router.navigateByUrl('/login'),
    });
  }
}
