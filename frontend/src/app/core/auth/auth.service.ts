import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { API_BASE } from '../http/api';
import type { Me } from '../http/types';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly meSubject = new BehaviorSubject<Me | null>(null);
  private bootstrapped = false;

  me$ = this.meSubject.asObservable();

  get me(): Me | null {
    return this.meSubject.value;
  }

  get isAuthenticated(): boolean {
    return !!this.meSubject.value;
  }

  async bootstrap(): Promise<void> {
    if (this.bootstrapped) return;
    this.bootstrapped = true;
    await this.ensureCsrf();
    await this.refreshMe();
  }

  async ensureCsrf(): Promise<void> {
    try {
      await firstValueFrom(this.http.get(`${API_BASE}/auth/csrf`, { responseType: 'text' as const }));
    } catch {}
  }

  async refreshMe(): Promise<void> {
    try {
      const me = await firstValueFrom(this.http.get<Me>(`${API_BASE}/auth/me`));
      this.meSubject.next(me);
    } catch {
      this.meSubject.next(null);
    }
  }

  async login(email: string, password: string): Promise<Me> {
    await this.ensureCsrf();
    const me = await firstValueFrom(this.http.post<Me>(`${API_BASE}/auth/login`, { email, password }));
    this.meSubject.next(me);
    return me;
  }
  async register(email: string, fullName: string, password: string): Promise<Me> {
    await this.ensureCsrf();
    const me = await firstValueFrom(
      this.http.post<Me>(`${API_BASE}/auth/register`, { email, fullName, password })
    );
    this.meSubject.next(me);
    return me;
  }

  async logout(): Promise<void> {
    try {
      await this.ensureCsrf();
      await firstValueFrom(this.http.post(`${API_BASE}/auth/logout`, {}, { responseType: 'text' as const }));
    } catch {}
    this.meSubject.next(null);
  }

  clear(): void {
    this.meSubject.next(null);
  }
}
