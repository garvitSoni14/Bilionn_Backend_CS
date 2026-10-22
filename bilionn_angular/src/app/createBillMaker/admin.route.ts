// import { Routes } from '@angular/router';
// import { CreateBillMaker } from './signup';
// import { adminGuard } from '../guards/admin.guard';
// import { unsavedChangesGuard } from '../guards/unsaved-changes.guard';

// export const adminRoutes: Routes = [
//   {
//     path: 'admin/users/new',
//     component: CreateBillMaker,
//     canActivate: [adminGuard],
//     canDeactivate: [unsavedChangesGuard],
//     title: 'Add cashier | Bilionn',
//   },
// ];

import { Routes } from '@angular/router';
import { adminGuard } from '../guards/admin.guard';
import { unsavedChangesGuard } from '../guards/unsaved-changes.guard';
import { CreateBillMaker } from '../createBillMaker/signup';
import { CreateCategory } from '../catalog/category/create-category';
import { CreateItem } from '../catalog/item/create-item';

export const adminRoutes: Routes = [
  {
    path: 'admin',
    canActivateChild: [adminGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'categories' },
      {
        path: 'categories',
        component: CreateCategory,
        canDeactivate: [unsavedChangesGuard],
        title: 'Add category | Bilionn',
      },
      {
        path: 'items',
        component: CreateItem,
        canDeactivate: [unsavedChangesGuard],
        title: 'Add item | Bilionn',
      },
      {
        path: 'users/new',
        component: CreateBillMaker,
        canDeactivate: [unsavedChangesGuard],
        title: 'Add cashier | Bilionn',
      },
    ],
  },
];