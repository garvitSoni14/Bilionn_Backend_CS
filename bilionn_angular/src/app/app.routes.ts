import { Routes } from '@angular/router';
import { loginRoutes } from './login/login.route';
import { adminRoutes } from './createBillMaker/admin.route';
import { dashboardRoutes } from './dashboard/dashboard.route';

export const routes: Routes = [
  ...loginRoutes,
  ...adminRoutes,
  ...dashboardRoutes,  

  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: '**', redirectTo: 'login' }
];








//All Routes were in one place...

// import { Routes } from '@angular/router';
// import { adminGuard } from './guards/admin.guard';
// import { CreateBillMaker } from './createBillMaker/signup';
// import { Login } from './login/login';
// import { Dashboard } from './dashboard/dashboard';
// import { authGuard } from './guards/auth.guard';
// import { unsavedChangesGuard } from './guards/unsaved-changes.guard';

// export const routes: Routes = [
//   { path: 'login', component: Login, title: 'Sign in | Bilionn' },
//   // { path: 'dashboard', component: Dashboard, title: 'Dashboard | Bilionn' },

//   // Now only logged in Cashier Or Admin can access the dashboard route
//   // { path: 'dashboard', component: Dashboard, canActivate: [authGuard], title: 'Dashboard | Bilionn' },

//   // Now along with CanActivate, we are also using CanDeactivate Guard to prevent unsaved changes reset...
//   {
//     path: 'admin/users/new',
//     component: CreateBillMaker,
//     canActivate: [adminGuard],
//     canDeactivate: [unsavedChangesGuard],
//     title: 'Add cashier | Bilionn',
//   },
//   {
//     path: 'dashboard',
//     component: Dashboard,
//     canActivate: [authGuard],
//     title: 'Dashboard | Bilionn',
//   },
  
//   { path: '', pathMatch: 'full', redirectTo: 'login' },
//   { path: '**', redirectTo: 'login' }
// ];

// // This got resolved with the currentUser() store mechanism....
// // the dashboard route has no guard, and route guards run in the browser using currentUser()
// // (in memory), not the cookie. Deleting the cookie doesn't touch that in-memory state, and
// // since you typed the URL in the address bar (a full page reload), everything in memory is wiped
// // anyway. Either way, nothing blocks the route itself.
