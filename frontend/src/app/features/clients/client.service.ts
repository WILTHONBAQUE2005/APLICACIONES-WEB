import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { API_BASE } from '../../core/http/api';
import type { ClientListItem, ClientUpsert } from '../../core/http/types';

@Injectable({ providedIn: 'root' })
export class ClientService {
  private readonly http = inject(HttpClient);

  list(): Promise<ClientListItem[]> {
    return firstValueFrom(this.http.get<ClientListItem[]>(`${API_BASE}/clients`));
  }

  create(payload: ClientUpsert) {
    return firstValueFrom(this.http.post(`${API_BASE}/clients`, payload));
  }

  update(id: string, payload: ClientUpsert) {
    return firstValueFrom(this.http.put(`${API_BASE}/clients/${id}`, payload));
  }

  remove(id: string) {
    return firstValueFrom(this.http.delete(`${API_BASE}/clients/${id}`, { responseType: 'text' as const }));
  }
  activate(id: string) {
    return firstValueFrom(this.http.post(`${API_BASE}/clients/${id}/activate`, {}, { responseType: 'text' as const }));
  }

}
