import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type Toast = {
  id: string;
  kind: 'success' | 'error' | 'info';
  message: string;
};

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly subject = new BehaviorSubject<Toast[]>([]);
  toasts$ = this.subject.asObservable();

  success(message: string) { this.push('success', message); }
  error(message: string) { this.push('error', message); }
  info(message: string) { this.push('info', message); }

  dismiss(id: string) {
    this.subject.next(this.subject.value.filter(t => t.id !== id));
  }

  private push(kind: Toast['kind'], message: string) {
    const id = crypto.randomUUID();
    const toast: Toast = { id, kind, message };
    this.subject.next([toast, ...this.subject.value].slice(0, 4));
    setTimeout(() => this.dismiss(id), 3200);
  }
}
