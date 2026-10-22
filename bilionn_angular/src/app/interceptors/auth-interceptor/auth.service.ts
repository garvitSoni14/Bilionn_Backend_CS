import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, firstValueFrom, finalize } from 'rxjs';
// import { jwtDecode } from 'jwt-decode';

export interface CurrentUser {
  // userId: number;
  userId: string;
  name: string;
  email: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5109/api';
  // private readonly userStorageKey = 'bilionn.user';
  // private readonly tokenCookieName = 'jwt';
  // readonly currentUser = signal<LoginResponse | null>(this.restoreUser());
  // Filled from the backend (login response or /me). Nothing is read from cookies or decoded.

  readonly currentUser = signal<CurrentUser | null>(null);

  // readonly currentUser = signal<JwtClaims | null>(this.readClaims());

  // constructor(private readonly http: HttpClient) {}

  // login(credentials: { email: string; password: string }): Observable<string> {

  // login(credentials: { email: string; password: string }): Observable<string> {
  //   return this.http
  //     .post(`${this.apiUrl}/login`, credentials, { responseType: 'text', withCredentials: true })
  //     .pipe(tap(() => this.currentUser.set(this.readClaims())));
  // }

  login(credentials: { email: string; password: string }): Observable<CurrentUser> {
    return this.http
      .post<CurrentUser>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(tap((user) => this.currentUser.set(user)));
  }

  logout(): Observable<void> {
    return this.http
      .post<void>(`${this.apiUrl}/auth/logout`, {})
      .pipe(finalize(() => this.currentUser.set(null)));
  }

  async restoreSession(): Promise<void> {
    try {
      const user = await firstValueFrom(this.http.get<CurrentUser>(`${this.apiUrl}/auth/me`));
      this.currentUser.set(user);
    } catch {
      this.currentUser.set(null);
    }
  }
  isLoggedIn(): boolean {
    return this.currentUser() !== null;
  }

  hasRole(...roles: string[]): boolean {
    const user = this.currentUser();
    return !!user && roles.includes(user.role);
  }

  //   return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
  //     tap((user) => {
  //       this.currentUser.set(user);
  //       sessionStorage.setItem(this.userStorageKey, JSON.stringify(user));
  //     }),
  //   );
  // }

  // logout(): Observable<void> {
  //   return this.http
  //     .post<void>(`${this.apiUrl}/logout`, {}, { withCredentials: true })
  //     .pipe(tap(() => this.currentUser.set(null)));
  // }

  // logout(): Observable<void> {

  //   return this.http.post<void>(`${this.apiUrl}/logout`, {}).pipe(
  //     tap(() => {
  //       this.currentUser.set(null);
  //       sessionStorage.removeItem(this.userStorageKey);
  //     }),
  //   );
  // }

  // This is the restoreSession method from session strg
  // async restoreSession(): Promise<void> {
  // try {
  //   // const user = await firstValueFrom(
  //   //   this.http.get<LoginResponse>(`${this.apiUrl}/me`, { withCredentials: true }),
  //   // );
  //    const user = await firstValueFrom(
  //     this.http.get<LoginResponse>(`${this.apiUrl}/me`),
  //   );
  //   this.currentUser.set(user);
  //   sessionStorage.setItem(this.userStorageKey, JSON.stringify(user));
  // } catch {
  //   this.currentUser.set(null);
  //   sessionStorage.removeItem(this.userStorageKey);
  // }
  // }

  // // Kept with the same name so my existing APP_INITIALIZER does not break.
  //   // It now just re-reads the cookie, with no HTTP call.
  // async restoreSession(): Promise<void> {
  //     this.currentUser.set(this.readClaims());
  //   }

  // isLoggedIn(): boolean {
  //   const c = this.currentUser();
  //   return !!c && c.exp * 1000 > Date.now();
  // }

  // hasRole(...roles: string[]): boolean {
  //   const c = this.currentUser();
  //   return this.isLoggedIn() && !!c && roles.includes(c.role);
  // }

  // private readClaims(): JwtClaims | null {
  //   const token = this.getCookie(this.tokenCookieName);
  //   if (!token) return null;
  //   try {
  //     const claims = jwtDecode<JwtClaims>(token);
  //     return claims.exp * 1000 > Date.now() ? claims : null;
  //   } catch {
  //     return null;
  //   }
  // }

  // private getCookie(name: string): string | null {
  //   const match = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'));
  //   return match ? decodeURIComponent(match[1]) : null;
  // }
}
