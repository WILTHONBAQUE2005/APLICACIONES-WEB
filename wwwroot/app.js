
let dt;
let modalCliente;
let modalConfirm;

let ciudades = [];
let tipos = [];
let cantones = [];

const api = {
  clientes: "/api/clientes",
  ciudades: "/api/ciudades",
  cantones: "/api/cantones",
  tipos: "/api/tiposcliente",
};

const byId = (id) => document.getElementById(id);

function toast(message, variant = "primary") {
  const container = byId("toastContainer");
  const id = `t_${Date.now()}`;

  const html = `
    <div id="${id}" class="toast align-items-center text-bg-${variant} border-0" role="alert" aria-live="assertive" aria-atomic="true">
      <div class="d-flex">
        <div class="toast-body">${message}</div>
        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Cerrar"></button>
      </div>
    </div>`;

  container.insertAdjacentHTML("beforeend", html);
  const el = document.getElementById(id);
  const t = new bootstrap.Toast(el, { delay: 3200 });
  t.show();
  el.addEventListener("hidden.bs.toast", () => el.remove());
}

function showFormError(msg) {
  const box = byId("formMsg");
  box.textContent = msg;
  box.classList.remove("d-none");
}

function clearFormError() {
  const box = byId("formMsg");
  box.classList.add("d-none");
  box.textContent = "";
}

async function fetchJson(url, options) {
  const res = await fetch(url, options);
  if (!res.ok) {
    let msg = `Error ${res.status}`;
    try {
      const data = await res.json();
      if (data?.message) msg = data.message;
    } catch {}
    throw new Error(msg);
  }
  
  if (res.status === 204) return null;
  return await res.json();
}

function normalize(str) {
  const v = (str ?? "").trim();
  return v.length ? v : null;
}


async function loadCatalogos() {
  ciudades = await fetchJson(api.ciudades);
  tipos = await fetchJson(api.tipos);

  
  const selCiudad = byId("ciudadId");
  selCiudad.innerHTML = `<option value="">Seleccione...</option>` + ciudades
    .map(c => `<option value="${c.id}">${c.provincia}</option>`)
    .join("");

  
  const selTipo = byId("tipoClienteId");
  selTipo.innerHTML = `<option value="">Seleccione...</option>` + tipos
    .map(t => `<option value="${t.id}">${t.nombre}</option>`)
    .join("");

  
  selCiudad.addEventListener("change", async () => {
    const ciudadId = parseInt(selCiudad.value || "0", 10);
    await loadCantones(ciudadId, null);
  });

  
  const ciudadIdInicial = parseInt(selCiudad.value || "0", 10);
  await loadCantones(ciudadIdInicial, null);
}

async function loadCantones(ciudadId, selectedCantonId) {
  const selCanton = byId("cantonId");
  selCanton.innerHTML = `<option value="">Seleccione...</option>`;

  if (!ciudadId || ciudadId <= 0) return;

  cantones = await fetchJson(`${api.cantones}?ciudadId=${ciudadId}`);
  selCanton.innerHTML += cantones
    .map(x => `<option value="${x.id}">${x.nombre}</option>`)
    .join("");

  if (selectedCantonId) {
    selCanton.value = String(selectedCantonId);
  }
}


async function loadClientes() {
  const filtro = byId("filtroEstado").value;

  
  const includeInactivos = filtro !== "activos";
  const url = includeInactivos ? `${api.clientes}?includeInactivos=true` : api.clientes;
  let data = await fetchJson(url);

  if (filtro === "inactivos") data = data.filter(x => x.activo === false);

  dt.clear().rows.add(data).draw();
}


function openNuevo() {
  clearFormError();
  byId("modalTitulo").textContent = "Nuevo cliente";
  byId("idCliente").value = "";
  byId("cedula").value = "";
  byId("nombres").value = "";
  byId("apellidos").value = "";
  byId("fechaNacimiento").value = "";
  byId("telefono").value = "";
  byId("email").value = "";
  byId("direccion").value = "";
  byId("activo").checked = true;

  
  byId("ciudadId").value = "";
  byId("tipoClienteId").value = "";
  byId("cantonId").innerHTML = `<option value="">Seleccione...</option>`;

  modalCliente.show();
}

async function openEditar(id) {
  clearFormError();
  byId("modalTitulo").textContent = `Editar cliente #${id}`;
  byId("idCliente").value = id;

  const dto = await fetchJson(`${api.clientes}/${id}`);

  byId("cedula").value = dto.cedula ?? "";
  byId("nombres").value = dto.nombres ?? "";
  byId("apellidos").value = dto.apellidos ?? "";
  byId("fechaNacimiento").value = dto.fechaNacimiento ? dto.fechaNacimiento.substring(0, 10) : "";
  byId("telefono").value = dto.telefono ?? "";
  byId("email").value = dto.email ?? "";
  byId("direccion").value = dto.direccion ?? "";
  byId("activo").checked = dto.activo === true;

  byId("ciudadId").value = String(dto.ciudadId);
  byId("tipoClienteId").value = String(dto.tipoClienteId);

  await loadCantones(dto.ciudadId, dto.cantonId);

  modalCliente.show();
}

function buildPayload() {
  return {
    cedula: normalize(byId("cedula").value),
    nombres: (byId("nombres").value || "").trim(),
    apellidos: (byId("apellidos").value || "").trim(),
    fechaNacimiento: byId("fechaNacimiento").value ? byId("fechaNacimiento").value : null,
    telefono: normalize(byId("telefono").value),
    email: normalize(byId("email").value),
    direccion: normalize(byId("direccion").value),
    ciudadId: parseInt(byId("ciudadId").value || "0", 10),
    cantonId: parseInt(byId("cantonId").value || "0", 10),
    tipoClienteId: parseInt(byId("tipoClienteId").value || "0", 10),
    activo: byId("activo").checked
  };
}


function validateClientSide(p) {
  if (!p.nombres) return "Nombres es obligatorio.";
  if (!p.apellidos) return "Apellidos es obligatorio.";
  if (!p.ciudadId) return "Debe seleccionar una ciudad.";
  if (!p.cantonId) return "Debe seleccionar un cantón.";
  if (!p.tipoClienteId) return "Debe seleccionar un tipo.";

  if (p.cedula && !/^\d{10}$/.test(p.cedula)) return "La cédula debe tener 10 dígitos.";
  if (p.telefono && !/^0\d{8,9}$/.test(p.telefono.replace(/\s|-/g, ""))) return "Teléfono no válido (0XXXXXXXXX).";
  if (p.email && !/^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(p.email)) return "Email no válido.";

  return null;
}

async function guardar() {
  clearFormError();
  const id = byId("idCliente").value ? parseInt(byId("idCliente").value, 10) : null;
  const payload = buildPayload();

  const localError = validateClientSide(payload);
  if (localError) {
    showFormError(localError);
    return;
  }

  try {
    if (!id) {
      await fetchJson(api.clientes, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });
      toast("Cliente creado correctamente.", "success");
    } else {
      await fetchJson(`${api.clientes}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });
      toast("Cliente actualizado correctamente.", "success");
    }

    modalCliente.hide();
    await loadClientes();
  } catch (e) {
    showFormError(e.message || "Error al guardar.");
  }
}


let pendingAction = null;

function confirmAction(title, text, btnVariant, btnText, fn) {
  byId("confirmTitulo").textContent = title;
  byId("confirmTexto").textContent = text;

  const btn = byId("btnConfirmarAccion");
  btn.className = `btn btn-${btnVariant}`;
  btn.textContent = btnText;

  pendingAction = fn;
  modalConfirm.show();
}

async function toggleActivo(id, activoActual, nombres) {
  if (activoActual) {
    confirmAction(
      "Desactivar cliente",
      `Se desactivará a: ${nombres}. Podrás reactivarlo luego.`,
      "danger",
      "Desactivar",
      async () => {
        await fetchJson(`${api.clientes}/${id}`, { method: "DELETE" });
        toast("Cliente desactivado.", "secondary");
        await loadClientes();
      }
    );
  } else {
    confirmAction(
      "Reactivar cliente",
      `Se reactivará a: ${nombres}.`,
      "success",
      "Reactivar",
      async () => {
        await fetchJson(`${api.clientes}/${id}/activar`, { method: "PUT" });
        toast("Cliente reactivado.", "success");
        await loadClientes();
      }
    );
  }
}


document.addEventListener("DOMContentLoaded", async () => {
  modalCliente = new bootstrap.Modal(byId("modalCliente"));
  modalConfirm = new bootstrap.Modal(byId("modalConfirm"));

  byId("btnNuevo").addEventListener("click", openNuevo);
  byId("btnGuardar").addEventListener("click", guardar);
  byId("filtroEstado").addEventListener("change", loadClientes);

  byId("btnConfirmarAccion").addEventListener("click", async () => {
    if (!pendingAction) return;
    const action = pendingAction;
    pendingAction = null;
    modalConfirm.hide();
    try {
      await action();
    } catch (e) {
      toast(e.message || "Error en la acción.", "danger");
    }
  });

  await loadCatalogos();

  
  dt = $("#tablaClientes").DataTable({
    language: {
      url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json"
    },
    pageLength: 10,
    scrollX: true,
    autoWidth: false,
    columnDefs: [{ targets: '_all', className: 'text-nowrap' }],
    lengthMenu: [10, 25, 50, 100],
    columns: [
      { data: "id" },
      { data: "cedula", defaultContent: "" },
      { data: "nombres" },
      { data: "apellidos" },
      { data: "telefono", defaultContent: "" },
      { data: "email", defaultContent: "" },
      { data: "ciudad" },
      { data: "canton" },
      { data: "tipoCliente" },
      {
        data: "activo",
        render: (v) => v
          ? '<span class="badge text-bg-success">Activo</span>'
          : '<span class="badge text-bg-secondary">Inactivo</span>'
      },
      {
        data: null,
        orderable: false,
        searchable: false,
        render: (row) => {
          const toggleText = row.activo ? "Desactivar" : "Reactivar";
          const toggleClass = row.activo ? "btn-outline-danger" : "btn-outline-success";
          return `
            <div class="d-flex gap-2 justify-content-end">
              <button class="btn btn-sm btn-outline-primary btn-edit" data-id="${row.id}">Editar</button>
              <button class="btn btn-sm ${toggleClass} btn-toggle" data-id="${row.id}">${toggleText}</button>
            </div>
          `;
        }
      }
    ]
  });

  
  document.querySelector("#tablaClientes tbody").addEventListener("click", async (ev) => {
    const btnEdit = ev.target.closest(".btn-edit");
    const btnToggle = ev.target.closest(".btn-toggle");

    if (btnEdit) {
      const id = parseInt(btnEdit.dataset.id, 10);
      try {
        await openEditar(id);
      } catch (e) {
        toast(e.message || "Error al cargar.", "danger");
      }
      return;
    }

    if (btnToggle) {
      const id = parseInt(btnToggle.dataset.id, 10);
      const row = dt.row(btnToggle.closest("tr")).data();
      const nombres = `${row.nombres} ${row.apellidos}`;
      try {
        await toggleActivo(id, row.activo, nombres);
      } catch (e) {
        toast(e.message || "Error en acción.", "danger");
      }
    }
  });

  await loadClientes();
});
