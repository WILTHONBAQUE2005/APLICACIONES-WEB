import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet, Router } from '@angular/router';
import { AsyncPipe, NgIf } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../shared/toast.service';

@Component({
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet, AsyncPipe, NgIf],
  templateUrl: './app-shell.component.html'
})
export class AppShellComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toasts = inject(ToastService);

  me$ = this.auth.me$;

  async logout() {
    await this.auth.logout();
    this.toasts.info('Sesión cerrada');
    await this.router.navigateByUrl('/login');
  }
}
