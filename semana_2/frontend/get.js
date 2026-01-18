
const API_URL = '../backend/controllers/cliente.controller.php';

async function apiRequest(method, url = API_URL, body = null) {
  const options = {
    method,
    headers: { 'Content-Type': 'application/json' },
  };

  if (body !== null) {
    options.body = JSON.stringify(body);
  }

  const res = await fetch(url, options);
  let data = null;
  try {
    data = await res.json();
  } catch {
    data = { ok: false, message: 'Respuesta no JSON', data: null, error: 'NON_JSON' };
  }

  if (!res.ok) {
    const msg = (data && data.message) ? data.message : `HTTP ${res.status}`;
    throw new Error(msg);
  }

  return data;
}

async function listarClientes() {
  return apiRequest('GET');
}

async function crearCliente(cliente) {
  return apiRequest('POST', API_URL, cliente);
}

async function actualizarCliente(id, cliente) {
  return apiRequest('PUT', `${API_URL}?id=${encodeURIComponent(id)}`, cliente);
}

async function eliminarCliente(id) {
  return apiRequest('DELETE', `${API_URL}?id=${encodeURIComponent(id)}`);
}
