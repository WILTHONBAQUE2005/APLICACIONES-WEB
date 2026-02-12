import { Routes } from '@angular/router';
import { LoginPageComponent } from './pages/login/login-page.component';
import { AppShellComponent } from './pages/shell/app-shell.component';
import { ClientsPageComponent } from './pages/clients/clients-page.component';
import { ProductsPageComponent } from './pages/products/products-page.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: 'login', component: LoginPageComponent },
  {
    path: 'app',
    component: AppShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'clients' },
      { path: 'clients', component: ClientsPageComponent },
      { path: 'products', component: ProductsPageComponent },
    ],
  },
  { path: '**', component: NotFoundComponent },
];
