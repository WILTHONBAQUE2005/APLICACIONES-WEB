import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { ToastService } from '../../shared/toast.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const toasts = inject(ToastService);

  const cloned = req.clone({ withCredentials: true });

  return next(cloned).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse) {
        const status = err.status;
        if (status === 401) {
          auth.clear();
          if (!req.url.includes('/auth/login')) router.navigateByUrl('/login');
          toasts.error('Sesión no válida. Inicia sesión otra vez.');
        } else if (status === 403) {
          toasts.error('No tienes permisos para esta acción.');
        } else if (status === 0) {
          toasts.error('No se pudo conectar con el servidor.');
        } else if (status >= 400) {
          const msg = (err.error && (err.error.title || err.error.detail)) ? `${err.error.title || ''} ${err.error.detail || ''}`.trim() : 'Ocurrió un error.';
          toasts.error(msg);
        }
      }
      return throwError(() => err);
    })
  );
};
