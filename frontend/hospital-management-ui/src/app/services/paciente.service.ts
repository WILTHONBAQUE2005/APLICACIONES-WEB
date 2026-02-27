import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-base.service';
import { Paciente, PacientePayload } from '../models/paciente.model';

@Injectable({ providedIn: 'root' })
export class PacienteService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${API_BASE_URL}/pacientes`;

  obtenerTodos(): Observable<Paciente[]> {
    return this.http.get<Paciente[]>(this.endpoint);
  }

  crear(payload: PacientePayload): Observable<Paciente> {
    return this.http.post<Paciente>(this.endpoint, payload);
  }

  actualizar(id: number, payload: PacientePayload): Observable<void> {
    return this.http.put<void>(`${this.endpoint}/${id}`, payload);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}
