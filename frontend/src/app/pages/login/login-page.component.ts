import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../shared/toast.service';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, RouterLink],
  templateUrl: './login-page.component.html'
})
export class LoginPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toasts = inject(ToastService);

  mode: 'login' | 'register' = 'login';
  busy = false;

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  registerForm = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirm: ['', [Validators.required, Validators.minLength(6)]]
  });

  toggleMode() {
    this.mode = this.mode === 'login' ? 'register' : 'login';
  }

  async submit() {
    if (this.form.invalid || this.busy) return;
    this.busy = true;

    try {
      const v = this.form.getRawValue();
      await this.auth.login(v.email || '', v.password || '');
      this.toasts.success('Bienvenido');
      await this.router.navigateByUrl('/app/clients');
    } finally {
      this.busy = false;
    }
  }

  async register() {
    if (this.registerForm.invalid || this.busy) return;

    const v = this.registerForm.getRawValue();
    if ((v.password || '') !== (v.confirm || '')) {
      this.toasts.error('Las contraseñas no coinciden');
      return;
    }

    this.busy = true;
    try {
      await this.auth.register(v.email || '', v.fullName || '', v.password || '');
      this.toasts.success('Cuenta creada');
      await this.router.navigateByUrl('/app/clients');
    } finally {
      this.busy = false;
    }
  }
}
