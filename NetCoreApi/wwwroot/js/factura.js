(() => {
  const $ = (id) => document.getElementById(id);

  const statusEl = $("status");
  const clienteSelect = $("clienteSelect");
  const facturaIdInput = $("facturaId");

  const money = new Intl.NumberFormat(undefined, { style: "currency", currency: "USD" });
  const shortDate = (iso) => {
    try {
      const d = new Date(iso);
      return d.toLocaleDateString(undefined, { year: "numeric", month: "2-digit", day: "2-digit" });
    } catch {
      return String(iso ?? "");
    }
  };

  const setStatus = (msg) => { statusEl.textContent = msg || ""; };
  const fetchJson = async (url) => {
    const res = await fetch(url, { headers: { "Accept": "application/json" } });
    if (!res.ok) {
      const text = await res.text().catch(() => "");
      throw new Error(text || `Error HTTP ${res.status}`);
    }
    return res.json();
  };

  const loadClientes = async () => {
    try {
      const list = await fetchJson("/api/Clientes");
      clienteSelect.innerHTML = "";
      const opt0 = document.createElement("option");
      opt0.value = "";
      opt0.textContent = "Consumidor final (sin seleccionar)";
      clienteSelect.appendChild(opt0);

      for (const c of list) {
        const opt = document.createElement("option");
        opt.value = String(c.id ?? "");
        opt.textContent = `${c.nombres ?? "Cliente"} (ID: ${c.id ?? "-"})`;
        clienteSelect.appendChild(opt);
      }

      setStatus(list.length ? "Clientes cargados." : "No hay clientes registrados. Se usará consumidor final.");
    } catch (e) {
      clienteSelect.innerHTML = "<option value=''>Consumidor final (sin seleccionar)</option>";
      setStatus("No se pudieron cargar clientes. Se usará consumidor final.");
    }
  };

  const renderFactura = (f) => {
    const emp = f.empresa || {};
    const cli = f.cliente || {};
    const tot = f.totales || {};

    $("empresaNombre").textContent = emp.nombre || "—";
    $("empresaDireccion").textContent = emp.direccion || "—";
    $("empresaTelefono").textContent = emp.telefono || "—";
    $("empresaEmail").textContent = emp.email || "—";

    $("facturaNumero").textContent = f.numero || "—";
    $("facturaFecha").textContent = f.fechaEmision ? shortDate(f.fechaEmision) : "—";
    $("facturaPago").textContent = f.formaPago || "—";
    $("facturaMoneda").textContent = f.moneda || "—";

    $("clienteNombre").textContent = cli.nombres || "—";
    $("clienteDireccion").textContent = cli.direccion || "—";
    $("clienteTelefono").textContent = cli.telefono || "—";
    $("clienteEmail").textContent = cli.email || "—";

    const body = $("detalleBody");
    body.innerHTML = "";
    const detalles = Array.isArray(f.detalles) ? f.detalles : [];
    if (!detalles.length) {
      body.innerHTML = "<tr><td colspan='4' class='muted'>Sin detalles</td></tr>";
    } else {
      for (const d of detalles) {
        const tr = document.createElement("tr");
        const importe = Number(d.importe ?? 0);
        tr.innerHTML = `
          <td>
            <div style="font-weight:800">${escapeHtml(d.descripcion ?? "")}</div>
            ${d.sku ? `<div class="muted" style="margin-top:2px">SKU: ${escapeHtml(d.sku)}</div>` : ""}
          </td>
          <td class="right">${Number(d.cantidad ?? 0).toFixed(2)}</td>
          <td class="right">${money.format(Number(d.precioUnitario ?? 0))}</td>
          <td class="right">${money.format(importe)}</td>
        `;
        body.appendChild(tr);
      }
    }

    $("facturaObs").textContent = f.observaciones || "—";
    $("subtotal").textContent = money.format(Number(tot.subtotal ?? 0));
    $("impuesto").textContent = money.format(Number(tot.impuesto ?? 0));
    $("total").textContent = money.format(Number(tot.total ?? 0));
  };

  const escapeHtml = (s) => String(s)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");

  const buildQuery = () => {
    const clienteId = clienteSelect.value ? Number(clienteSelect.value) : null;
    return clienteId ? `?clienteId=${encodeURIComponent(clienteId)}` : "";
  };

  const loadDemo = async () => {
    setStatus("Generando factura…");
    try {
      const f = await fetchJson(`/api/Facturas/demo${buildQuery()}`);
      renderFactura(f);
      setStatus("Factura lista.");
    } catch (e) {
      setStatus(`No se pudo generar la factura. ${e.message}`);
    }
  };

  const loadById = async () => {
    const id = Number(facturaIdInput.value || 0);
    if (!id || id < 1) {
      setStatus("Ingresa un ID válido (mayor a 0).");
      return;
    }
    setStatus("Buscando factura…");
    try {
      const f = await fetchJson(`/api/Facturas/${id}${buildQuery()}`);
      renderFactura(f);
      setStatus("Factura lista.");
    } catch (e) {
      setStatus(`No se pudo obtener la factura. ${e.message}`);
    }
  };

  $("btnDemo").addEventListener("click", loadDemo);
  $("btnBuscar").addEventListener("click", loadById);
  $("btnPrint").addEventListener("click", () => window.print());

  // Init
  loadClientes().then(loadDemo);
})();
