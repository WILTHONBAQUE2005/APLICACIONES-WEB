export interface CitaMedica {
  citaId: number;
  pacienteId: number;
  doctorId: number;
  fechaHora: string;
  motivo: string;
  estado: string;
  pacienteNombreCompleto: string;
  doctorNombreCompleto: string;
  especialidadDoctor: string;
}

export interface CitaMedicaPayload {
  pacienteId: number;
  doctorId: number;
  fechaHora: string;
  motivo: string;
  estado: string;
}
