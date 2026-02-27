import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-base.service';
import { CitaMedica, CitaMedicaPayload } from '../models/cita-medica.model';

@Injectable({ providedIn: 'root' })
export class CitaMedicaService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${API_BASE_URL}/citasmedicas`;

  obtenerTodas(): Observable<CitaMedica[]> {
    return this.http.get<CitaMedica[]>(this.endpoint);
  }

  crear(payload: CitaMedicaPayload): Observable<CitaMedica> {
    return this.http.post<CitaMedica>(this.endpoint, payload);
  }

  actualizar(id: number, payload: CitaMedicaPayload): Observable<void> {
    return this.http.put<void>(`${this.endpoint}/${id}`, payload);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}
