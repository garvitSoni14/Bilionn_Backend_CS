import { Routes } from '@angular/router';
import { Dashboard } from './dashboard';
import { authGuard } from '../guards/auth.guard';

export const dashboardRoutes: Routes = [
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard],
    title: 'Dashboard | Bilionn',
  },
];