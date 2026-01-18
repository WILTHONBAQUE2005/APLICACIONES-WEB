"use strict";

const API_URL = new URL("../backend/controllers/cliente.controller.php", window.location.href).toString();

const countries = {
  EC: { name: "Ecuador", prefix: "+593", exampleLocal: "98 282 2349", exampleFull: "+593 98 282 2349" },
  CO: { name: "Colombia", prefix: "+57",  exampleLocal: "300 123 4567", exampleFull: "+57 300 123 4567" },
  PE: { name: "Perú", prefix: "+51",  exampleLocal: "987 654 321",  exampleFull: "+51 987 654 321" },
  VE: { name: "Venezuela", prefix: "+58", exampleLocal: "412 123 4567", exampleFull: "+58 412 123 4567" },
  US: { name: "Estados Unidos", prefix: "+1",  exampleLocal: "202 555 0123",  exampleFull: "+1 202 555 0123" },
  ES: { name: "España", prefix: "+34", exampleLocal: "612 345 678",  exampleFull: "+34 612 345 678" },
};

const phoneRules = {
  EC: { len: 9,  groups: [2, 3, 4] }, // 98 282 2349
  CO: { len: 10, groups: [3, 3, 4] }, // 300 123 4567
  PE: { len: 9,  groups: [3, 3, 3] }, // 987 654 321
  VE: { len: 10, groups: [3, 3, 4] }, // 412 123 4567
  US: { len: 10, groups: [3, 3, 4] }, // 202 555 0123
  ES: { len: 9,  groups: [3, 3, 3] }, // 612 345 678
};

function normalizeLocalDigitsByCountry(iso, digits) {
  const rule = phoneRules[iso];
  if (!rule) return digits;
  if (digits.length === rule.len + 1 && digits.startsWith("0")) return digits.slice(1);
  return digits;
}

function formatLocalByCountry(iso, digits) {
  const rule = phoneRules[iso];
  if (!rule) return digits;

  const parts = [];
  let pos = 0;
  for (const g of rule.groups) {
    parts.push(digits.slice(pos, pos + g));
    pos += g;
  }
  return parts.join(" ").trim();
}

/* ===== Helpers DOM ===== */
const el = (id) => document.getElementById(id);

const tbody = el("tbody");
const searchInput = el("search");

const emptyState = el("emptyState");
const emptyTitle = el("emptyTitle");
const emptyDesc = el("emptyDesc");
const btnEmptyNew = el("btnEmptyNew");
const clientesTable = el("clientesTable");

const btnNew = el("btnNew");
const btnRefresh = el("btnRefresh");

/* Form modal */
const formModal = el("formModal");
const form = el("clienteForm");
const formTitle = el("formTitle");
const formHint = el("formHint");
const btnCancel = el("btnCancel");

const clienteId = el("clienteId");
const cedula = el("cedula");
const nombres = el("nombres");
const apellidos = el("apellidos");
const email = el("email");
const paisIso = el("paisIso");
const telefonoPrefix = el("telefonoPrefix");
const telefonoLocal = el("telefonoLocal");
const phoneExample = el("phoneExample");

/* Delete modal */
const deleteModal = el("deleteModal");
const deleteText = el("deleteText");
const deleteConfirm = el("deleteConfirm");
const btnDeleteConfirm = el("btnDeleteConfirm");

const toast = el("toast");

/* Estado global */
let state = {
  rows: [],
  deletingId: null,
};

let openMenuId = null;

/* ===== Utilidades ===== */
function showToast(msg) {
  toast.textContent = msg;
  toast.classList.add("show");
  window.clearTimeout(showToast._t);
  showToast._t = window.setTimeout(() => toast.classList.remove("show"), 2600);
}

function onlyDigits(s) {
  return (s || "").replace(/\D+/g, "");
}

function normalizeSpaces(s) {
  return (s || "").trim().replace(/\s+/g, " ");
}

function escapeHtml(s) {
  return String(s ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

/* ===== Validación cédula (EC) ===== */
function isValidCedulaEC(ced) {
  if (!/^\d{10}$/.test(ced)) return false;

  const prov = parseInt(ced.slice(0, 2), 10);
  const third = parseInt(ced[2], 10);
  if (prov < 1 || prov > 24) return false;
  if (third >= 6) return false;

  const coeff = [2, 1, 2, 1, 2, 1, 2, 1, 2];
  let sum = 0;
  for (let i = 0; i < 9; i++) {
    let p = parseInt(ced[i], 10) * coeff[i];
    if (p >= 10) p -= 9;
    sum += p;
  }
  const mod = sum % 10;
  const check = mod === 0 ? 0 : 10 - mod;
  return check === parseInt(ced[9], 10);
}

function validateCedulaField() {
  const iso = paisIso.value;
  const digits = onlyDigits(cedula.value);
  cedula.value = digits;

  if (iso === "EC") {
    if (!isValidCedulaEC(digits)) {
      cedula.setCustomValidity("Cédula de Ecuador inválida (10 dígitos válidos).");
    } else {
      cedula.setCustomValidity("");
    }
  } else {
    if (!/^\d{6,20}$/.test(digits)) {
      cedula.setCustomValidity("Documento inválido (solo dígitos, 6 a 20).");
    } else {
      cedula.setCustomValidity("");
    }
  }
}

function validatePhoneField() {
  const iso = paisIso.value;

  let digits = onlyDigits(telefonoLocal.value);
  digits = normalizeLocalDigitsByCountry(iso, digits);

  const rule = phoneRules[iso];

  if (!digits) {
    telefonoLocal.setCustomValidity("Teléfono obligatorio (solo dígitos).");
    return;
  }

  if (rule) {
    if (digits.length !== rule.len) {
      telefonoLocal.setCustomValidity(`Teléfono inválido para ${iso}. Debe tener ${rule.len} dígitos (sin prefijo).`);
    } else {
      telefonoLocal.setCustomValidity("");
    }
    telefonoLocal.value = formatLocalByCountry(iso, digits);
    return;
  }

  // fallback general
  if (digits.length < 6 || digits.length > 15) {
    telefonoLocal.setCustomValidity("Teléfono inválido (6 a 15 dígitos).");
  } else {
    telefonoLocal.setCustomValidity("");
  }
  telefonoLocal.value = digits;
}

function updatePhoneUI() {
  const iso = paisIso.value;
  const c = countries[iso];

  telefonoPrefix.value = c ? c.prefix : "";
  telefonoLocal.placeholder = c ? `Ej.: ${c.exampleLocal}` : "Ej.: 987 654 321";
  phoneExample.textContent = c ? c.exampleFull : "+000 987 654 321";

  validateCedulaField();
  validatePhoneField();
}

/* ===== API ===== */
async function apiFetch(url, options = {}) {
  const res = await fetch(url, {
    cache: options.cache ?? "no-store",
    headers: { "Content-Type": "application/json", ...(options.headers || {}) },
    ...options,
  });

  let data = null;
  try {
    data = await res.json();
  } catch (_) {}

  if (!res.ok) {
    const msg = data?.error ? data.error : `Error HTTP ${res.status}`;
    const err = new Error(msg);
    err.status = res.status;
    err.payload = data;
    throw err;
  }
  return data;
}

/* ===== Iconos ===== */
function iconEditSvg() {
  return `
  <svg class="ico" viewBox="0 0 24 24" fill="none" aria-hidden="true">
    <path d="M12 20h9" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L8 18l-4 1 1-4L16.5 3.5Z"
          stroke="currentColor" stroke-width="2" stroke-linejoin="round"/>
  </svg>`;
}

function iconTrashSvg() {
  return `
  <svg class="ico" viewBox="0 0 24 24" fill="none" aria-hidden="true">
    <path d="M3 6h18" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
    <path d="M8 6V4h8v2" stroke="currentColor" stroke-width="2" stroke-linejoin="round"/>
    <path d="M6 6l1 15h10l1-15" stroke="currentColor" stroke-width="2" stroke-linejoin="round"/>
    <path d="M10 11v6M14 11v6" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
  </svg>`;
}

/* ===== Menú ⋯ ===== */
function closeAllMenus() {
  document.querySelectorAll(".dropdown.open").forEach(d => d.classList.remove("open"));
  document.querySelectorAll(".kebab[aria-expanded='true']").forEach(b => b.setAttribute("aria-expanded", "false"));
  openMenuId = null;
}

function toggleMenu(id) {
  if (openMenuId === id) {
    closeAllMenus();
    return;
  }

  closeAllMenus();

  const dd = document.querySelector(`.dropdown[data-menu-for="${id}"]`);
  const kb = document.querySelector(`.kebab[data-menu-btn="${id}"]`);
  if (!dd || !kb) return;

  dd.classList.add("open");
  kb.setAttribute("aria-expanded", "true");
  openMenuId = id;

  const firstItem = dd.querySelector("button.menu-item");
  if (firstItem) firstItem.focus();
}

/* ===== Render ===== */
function renderTable(list) {
  const hasRows = Array.isArray(list) && list.length > 0;

  if (emptyState) emptyState.hidden = hasRows;
  if (clientesTable) clientesTable.style.display = hasRows ? "" : "none";

  tbody.innerHTML = "";

  if (!hasRows) return;

  for (const r of list) {
    const tr = document.createElement("tr");

    const emailSafe = escapeHtml(r.email);
    const nombresSafe = escapeHtml(r.nombres);
    const apellidosSafe = escapeHtml(r.apellidos);

    tr.innerHTML = `
      <td>${r.id}</td>
      <td><span class="pill">${escapeHtml(r.cedula)}</span></td>
      <td><span class="truncate" title="${nombresSafe}">${nombresSafe}</span></td>
      <td><span class="truncate" title="${apellidosSafe}">${apellidosSafe}</span></td>
      <td><span class="truncate mono" title="${emailSafe}">${emailSafe}</span></td>
      <td>
        <span class="pill pill-country" data-iso="${escapeHtml(r.pais_iso)}">
          ${escapeHtml(r.pais_iso)}
        </span>
      </td>
      <td class="mono">
        <span class="truncate" title="${escapeHtml(r.telefono)}">${escapeHtml(r.telefono)}</span>
      </td>
      <td class="right">
        <div class="menu-wrap">
          <button
            class="kebab"
            type="button"
            aria-haspopup="true"
            aria-expanded="false"
            title="Acciones"
            data-action="menu"
            data-id="${r.id}"
            data-menu-btn="${r.id}"
          >⋯</button>

          <div class="dropdown" role="menu" aria-label="Acciones" data-menu-for="${r.id}">
            <button class="menu-item" type="button" role="menuitem" data-action="edit" data-id="${r.id}">
              ${iconEditSvg()} Editar
            </button>
            <button class="menu-item danger" type="button" role="menuitem" data-action="delete" data-id="${r.id}">
              ${iconTrashSvg()} Eliminar
            </button>
          </div>
        </div>
      </td>
    `;

    tbody.appendChild(tr);
  }
}

function applySearch() {
  const q = (searchInput.value || "").trim().toLowerCase();
  if (!q) return renderTable(state.rows);

  const filtered = state.rows.filter(r => {
    const hay = [r.cedula, r.nombres, r.apellidos, r.email, r.pais_iso, r.telefono].join(" ").toLowerCase();
    return hay.includes(q);
  });

  renderTable(filtered);
}

async function load() {
  const data = await apiFetch(API_URL, { cache: "no-store" });
  const arr = data?.data;
  state.rows = Array.isArray(arr) ? arr : [];
  applySearch();
}

/* ===== Modales ===== */
function openFormModal() {
  formModal.setAttribute("aria-hidden", "false");
  setTimeout(() => cedula.focus(), 0);
}

function closeFormModal() {
  formModal.setAttribute("aria-hidden", "true");
}

function resetForm(mode = "create") {
  form.reset();
  form.classList.remove("was-validated");
  clienteId.value = "";

  for (const inp of form.querySelectorAll("input, select")) {
    inp.classList.remove("touched");
  }

  paisIso.value = "EC";
  updatePhoneUI();

  if (mode === "create") {
    formTitle.textContent = "Registrar cliente";
    formHint.textContent = "Todos los campos son obligatorios.";
  } else {
    formTitle.textContent = "Editar cliente";
    formHint.textContent = "Guarda cambios. No se permiten duplicados.";
  }

  cedula.setCustomValidity("");
  telefonoLocal.setCustomValidity("");
}

function fillForm(row) {
  clienteId.value = row.id;
  cedula.value = row.cedula;
  nombres.value = row.nombres;
  apellidos.value = row.apellidos;
  email.value = row.email;
  paisIso.value = row.pais_iso;

  updatePhoneUI();

  const prefix = countries[row.pais_iso]?.prefix || "";
  let allDigits = onlyDigits(row.telefono);
  const prefixDigits = onlyDigits(prefix);

  if (prefixDigits && allDigits.startsWith(prefixDigits)) {
    allDigits = allDigits.slice(prefixDigits.length);
  }

  telefonoLocal.value = formatLocalByCountry(row.pais_iso, allDigits);

  formTitle.textContent = "Editar cliente";
  formHint.textContent = "Guarda cambios. No se permiten duplicados.";
}

/* Delete modal */
function openDeleteModal(row) {
  state.deletingId = row.id;
  deleteConfirm.value = "";
  btnDeleteConfirm.disabled = true;

  deleteText.textContent =
    `Vas a eliminar al cliente #${row.id} (${row.nombres} ${row.apellidos}, ${row.email}). Esta acción no se puede deshacer.`;

  deleteModal.setAttribute("aria-hidden", "false");
  deleteConfirm.focus();
}

function closeDeleteModal() {
  deleteModal.setAttribute("aria-hidden", "true");
  state.deletingId = null;
}

async function handleDeleteConfirm() {
  const id = state.deletingId;
  if (!id) return;

  await apiFetch(`${API_URL}?id=${encodeURIComponent(id)}`, { method: "DELETE" });
  showToast("Cliente eliminado.");
  closeDeleteModal();
  await load();
}

/* ===== Eventos UI ===== */
btnNew.addEventListener("click", () => {
  closeAllMenus();
  resetForm("create");
  openFormModal();
});

btnRefresh.addEventListener("click", () => load().catch(() => showToast("No se pudo cargar la data.")));
searchInput.addEventListener("input", applySearch);

paisIso.addEventListener("change", updatePhoneUI);

cedula.addEventListener("input", validateCedulaField);
telefonoLocal.addEventListener("input", validatePhoneField);

// touched (mostrar invalid solo tras interacción)
for (const inp of [cedula, nombres, apellidos, email, paisIso, telefonoLocal]) {
  inp.addEventListener("blur", () => inp.classList.add("touched"));
}

btnCancel.addEventListener("click", closeFormModal);

// Cerrar modal form al click backdrop o botón X
formModal.addEventListener("click", (e) => {
  if (e.target.hasAttribute("data-close-form")) closeFormModal();
});

// Delegación tabla
tbody.addEventListener("click", (e) => {
  const btn = e.target.closest("[data-action]");
  if (!btn) return;

  const action = btn.dataset.action;
  const id = parseInt(btn.dataset.id || "0", 10);

  if (action === "menu") {
    e.stopPropagation();
    toggleMenu(id);
    return;
  }

  const row = state.rows.find(r => r.id === id);
  if (!row) return;

  closeAllMenus();

  if (action === "edit") {
    resetForm("edit");
    fillForm(row);
    openFormModal();
  } else if (action === "delete") {
    openDeleteModal(row);
  }
});

// Modal eliminar
deleteModal.addEventListener("click", (e) => {
  if (e.target.hasAttribute("data-close")) closeDeleteModal();
});

deleteConfirm.addEventListener("input", () => {
  btnDeleteConfirm.disabled = deleteConfirm.value.trim().toUpperCase() !== "ELIMINAR";
});

btnDeleteConfirm.addEventListener("click", () => handleDeleteConfirm().catch(err => showToast(err.message || "Error")));

document.addEventListener("click", (e) => {
  if (!e.target.closest(".menu-wrap")) closeAllMenus();
});

document.addEventListener("keydown", (e) => {
  if (e.key === "Escape") closeAllMenus();
});

// Botón estado vacío
if (btnEmptyNew) {
  btnEmptyNew.addEventListener("click", () => {
    const q = (searchInput.value || "").trim();
    if (q) {
      searchInput.value = "";
      applySearch();
    } else {
      btnNew.click();
    }
  });
}

// Submit form
form.addEventListener("submit", async (e) => {
  e.preventDefault();

  form.classList.add("was-validated");

  nombres.value = normalizeSpaces(nombres.value);
  apellidos.value = normalizeSpaces(apellidos.value);
  email.value = (email.value || "").trim().toLowerCase();

  validateCedulaField();
  validatePhoneField();

  if (!form.checkValidity()) {
    form.reportValidity();
    return;
  }

  const payload = {
    cedula: onlyDigits(cedula.value),
    nombres: nombres.value,
    apellidos: apellidos.value,
    email: email.value,
    pais_iso: paisIso.value,
    telefono_local: onlyDigits(telefonoLocal.value),
  };

  const id = clienteId.value ? parseInt(clienteId.value, 10) : null;

  try {
    closeAllMenus();
    if (!id) {
      await apiFetch(API_URL, { method: "POST", body: JSON.stringify(payload) });
      showToast("Cliente creado.");
    } else {
      await apiFetch(`${API_URL}?id=${encodeURIComponent(id)}`, { method: "PUT", body: JSON.stringify(payload) });
      showToast("Cliente actualizado.");
    }

    closeFormModal();
    await load();
  } catch (err) {
    showToast(err.message || "Error");
  }
});

/* Init */
updatePhoneUI();
load().catch(() => showToast("No se pudo cargar la data."));
