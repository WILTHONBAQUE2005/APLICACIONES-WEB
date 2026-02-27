export interface Paciente {
  pacienteId: number;
  nombre: string;
  apellido: string;
  fechaNacimiento: string;
  telefono: string;
}

export interface PacientePayload {
  nombre: string;
  apellido: string;
  fechaNacimiento: string;
  telefono: string;
}
