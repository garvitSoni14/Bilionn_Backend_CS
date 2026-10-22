import { inject } from '@angular/core';
import { CanActivateChildFn, Router } from '@angular/router';
import { AuthService } from '../interceptors/auth-interceptor/auth.service';

// Now i am had converted it to a child guard which will automatically be applied on the child routes like /categories, /items, ... 

export const adminGuard: CanActivateChildFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.hasRole('ADMIN')) return true;
  return router.createUrlTree([auth.isLoggedIn() ? '/dashboard' : '/login']);
};


// whEN it was a canactivate guard .. and we have to apply it on every child guard manually

// import { inject } from '@angular/core';
// import { CanActivateFn, Router } from '@angular/router';
// import { AuthService } from '../interceptors/auth-interceptor/auth.service';

// export const adminGuard: CanActivateFn = () => {
//   const auth = inject(AuthService);
//   const router = inject(Router);
//   return auth.currentUser()?.role === 'ADMIN' ? true : router.createUrlTree(['/login']);
// };