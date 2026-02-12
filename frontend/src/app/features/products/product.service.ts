import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { API_BASE } from '../../core/http/api';
import type { ProductListItem, ProductUpsert } from '../../core/http/types';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);

  list(): Promise<ProductListItem[]> {
    return firstValueFrom(this.http.get<ProductListItem[]>(`${API_BASE}/products`));
  }

  create(payload: ProductUpsert) {
    return firstValueFrom(this.http.post(`${API_BASE}/products`, payload));
  }

  update(id: string, payload: ProductUpsert) {
    return firstValueFrom(this.http.put(`${API_BASE}/products/${id}`, payload));
  }

  remove(id: string) {
    return firstValueFrom(this.http.delete(`${API_BASE}/products/${id}`, { responseType: 'text' as const }));
  }
  activate(id: string) {
    return firstValueFrom(this.http.post(`${API_BASE}/products/${id}/activate`, {}, { responseType: 'text' as const }));
  }

}
