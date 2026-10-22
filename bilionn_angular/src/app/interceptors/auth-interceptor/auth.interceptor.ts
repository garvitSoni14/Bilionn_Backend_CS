import { HttpInterceptorFn } from '@angular/common/http';

const API_URL = 'http://localhost:5109';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Only touch requests to our own backend, never third-party URLs
  if (!req.url.startsWith(API_URL)) {
    return next(req);
  }

  // Requests are immutable, so clone with the change
  const authReq = req.clone({ withCredentials: true });
  return next(authReq);
};

/// Login:    AuthService.login() → POST /login → backend sets the encrypted HttpOnly "auth" cookie
//           and returns { userId, name, email, role } → stored in the currentUser signal.
// Requests: this interceptor adds withCredentials: true → the browser attaches the cookie.
// Refresh:  the app initializer calls restoreSession() → GET /me → currentUser is filled again.
// Logout:   POST /logout → backend clears the cookie → currentUser = null.