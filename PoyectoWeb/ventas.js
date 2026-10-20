const API_BASE_URL_VENTAS = "https://localhost:7067/api";

if (!localStorage.getItem('authToken')) {
    window.location.href = 'index.html';
}

const permisosPorRolVentas = {
    1: { nombre: "Administrador", paginasPermitidas: ["dashboard.html", "inventario.html", "comercial.html", "cuarentena.html", "usuarios.html", "ventas.html"] },
    2: { nombre: "Logística", paginasPermitidas: ["dashboard.html", "inventario.html"] },
    3: { nombre: "Calidad", paginasPermitidas: ["dashboard.html", "cuarentena.html"] },
    4: { nombre: "Almacén MP", paginasPermitidas: ["dashboard.html", "inventario.html"] },
    5: { nombre: "Almacén PT", paginasPermitidas: ["dashboard.html", "inventario.html"] },
    6: { nombre: "Ventas", paginasPermitidas: ["dashboard.html", "ventas.html"] }
};

// === VARIABLES GLOBALES ===
let productosDisponibles = [];
let contadorFilas = 0;
let contadorFilasEditar = 0;

function obtenerRolIdVentas() {
    const rolId = localStorage.getItem('usuarioRolId');
    const rolNombre = String(localStorage.getItem('usuarioRolNombre') || "").toLowerCase();
    const rol = String(localStorage.getItem('usuarioRol') || "").toLowerCase();
    if (rolId && rolId !== "") return parseInt(rolId);
    if (rol === "6" || rol === "ventas" || rol === "ven") return 6;
    if (rolNombre === "6" || rolNombre === "ventas" || rolNombre === "ven") return 6;
    if (rol === "1" || rol === "administrador" || rol === "adm") return 1;
    return 1;
}

(function protegerAccesoVentas() {
    const rolId = obtenerRolIdVentas();
    const paginaActual = window.location.pathname.split('/').pop() || 'ventas.html';
    const permisos = permisosPorRolVentas[rolId];
    if (permisos && !permisos.paginasPermitidas.includes(paginaActual)) {
        window.location.href = 'dashboard.html';
    }
})();

function getAuthHeadersVentas() {
    const token = localStorage.getItem("authToken");
    return { "Content-Type": "application/json", "Authorization": token ? `Bearer ${token}` : "" };
}

function obtenerIdUsuarioDesdeToken() {
    const token = localStorage.getItem("authToken");
    if (!token) return null;
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        const payload = JSON.parse(jsonPayload);
        return payload.idUsuario || payload.IdUsuario || payload.id || payload.sub || payload.nameid || localStorage.getItem("usuarioId");
    } catch (error) {
        return localStorage.getItem("usuarioId");
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const rolId = obtenerRolIdVentas();
    const nombreUsuario = localStorage.getItem('usuarioNombre') || 'Usuario';

    document.getElementById('user-name').textContent = nombreUsuario;
    document.getElementById('user-role').textContent = permisosPorRolVentas[rolId]?.nombre || "Rol Desconocido";
    document.getElementById('user-avatar').textContent = nombreUsuario.trim().charAt(0).toUpperCase();

    document.querySelectorAll('.sidebar-menu li[data-roles]').forEach(item => {
        const roles = item.getAttribute('data-roles').split(',').map(r => parseInt(r.trim()));
        item.style.display = roles.includes(rolId) ? 'block' : 'none';
    });

    document.querySelectorAll('.sidebar-menu li[data-roles]').forEach(item => {
        const link = item.querySelector('a');
        if (link && link.getAttribute('href') === 'ventas.html') {
            item.classList.add('active');
        } else {
            item.classList.remove('active');
        }
    });

    cargarVentas();
    cargarClientesParaSelect();
    cargarProductosParaSelect();

    // Modal Nueva Venta
    const modal = document.getElementById("modal-venta");
    document.getElementById("btn-nueva-venta")?.addEventListener("click", () => {
        if (modal) {
            modal.classList.remove("hidden");
            document.getElementById("form-venta")?.reset();
            document.getElementById("tabla-detalles-venta").innerHTML = "";
            document.getElementById("total-general").textContent = "Bs. 0.00";
            contadorFilas = 0;
            agregarFilaProducto();
        }
    });
    document.getElementById("btn-cerrar-modal-venta")?.addEventListener("click", () => { if (modal) modal.classList.add("hidden"); });
    document.getElementById("btn-cancelar-venta")?.addEventListener("click", () => { if (modal) modal.classList.add("hidden"); });
    document.getElementById("btn-agregar-producto")?.addEventListener("click", () => { agregarFilaProducto(); });
    document.getElementById("form-venta")?.addEventListener("submit", async (e) => { e.preventDefault(); await registrarVenta(); });

    // Modal Detalle
    document.getElementById("btn-cerrar-detalle")?.addEventListener("click", () => { document.getElementById("modal-detalle-venta").classList.add("hidden"); });

    // Modal Editar
    document.getElementById("btn-cerrar-editar")?.addEventListener("click", () => { document.getElementById("modal-editar-venta").classList.add("hidden"); });
    document.getElementById("btn-cancelar-editar")?.addEventListener("click", () => { document.getElementById("modal-editar-venta").classList.add("hidden"); });
    document.getElementById("btn-agregar-producto-editar")?.addEventListener("click", () => { agregarFilaProductoEditar(); });
    document.getElementById("form-editar-venta")?.addEventListener("submit", async (e) => { e.preventDefault(); await guardarEdicionVenta(); });

    // Modal Historial
    document.getElementById("btn-cerrar-historial")?.addEventListener("click", () => { document.getElementById("modal-historial").classList.add("hidden"); });
});

// === CARGAR DATOS ===
async function cargarVentas() {
    const rolId = obtenerRolIdVentas();
    try {
        const response = await fetch(`${API_BASE_URL_VENTAS}/Ventas`, { headers: getAuthHeadersVentas() });
        if (response.ok) {
            const ventas = await response.json();
            const tbody = document.getElementById("tabla-ventas");
            if (tbody) {
                tbody.innerHTML = "";
                if (ventas.length === 0) {
                    tbody.innerHTML = "<tr><td colspan='4' style='text-align:center; padding: 20px;'>No hay ventas registradas</td></tr>";
                    return;
                }
                
                ventas.forEach(v => {
                    const tr = document.createElement("tr");
                    const idVenta = v.idVenta || 0;
                    const cliente = v.nombreCliente || 'Cliente';
                    const estado = v.estado || 'Completada';
                    const estadoColor = estado === 'Completada' ? '#22c55e' : (estado === 'Eliminada' ? '#ef4444' : '#f59e0b');
                    
                    tr.innerHTML = `
                        <td><strong>#${String(idVenta).padStart(4, '0')}</strong></td>
                        <td>${cliente}</td>
                        <td><span style="background: ${estadoColor}20; color: ${estadoColor}; padding: 6px 12px; border-radius: 20px; font-weight: 600; font-size: 12px;">${estado}</span></td>
                        <td>
                            <div style="display: flex; gap: 8px;">
                                <button onclick="verDetalleVenta(${idVenta})" style="background: #0ea5e9; color: white; border: none; width: 36px; height: 36px; border-radius: 8px; cursor: pointer; display: inline-flex; align-items: center; justify-content: center;" title="Ver detalle">
                                    <i class="fa-solid fa-eye"></i>
                                </button>
                                <button onclick="editarVenta(${idVenta})" style="background: #3b82f6; color: white; border: none; width: 36px; height: 36px; border-radius: 8px; cursor: pointer; display: inline-flex; align-items: center; justify-content: center;" title="Editar">
                                    <i class="fa-solid fa-pen"></i>
                                </button>
                                ${rolId === 1 ? `
                                    <button onclick="verHistorialVenta(${idVenta})" style="background: #f59e0b; color: white; border: none; width: 36px; height: 36px; border-radius: 8px; cursor: pointer; display: inline-flex; align-items: center; justify-content: center;" title="Ver historial (Solo Admin)">
                                        <i class="fa-solid fa-clock-rotate-left"></i>
                                    </button>
                                ` : ''}
                                <button onclick="eliminarVenta(${idVenta})" style="background: #ef4444; color: white; border: none; width: 36px; height: 36px; border-radius: 8px; cursor: pointer; display: inline-flex; align-items: center; justify-content: center;" title="Eliminar">
                                    <i class="fa-solid fa-trash"></i>
                                </button>
                            </div>
                        </td>
                    `;
                    tbody.appendChild(tr);
                });
            }
        }
    } catch (error) { 
        console.error("Error al cargar ventas:", error); 
    }
}

async function cargarClientesParaSelect() {
    try {
        const response = await fetch(`${API_BASE_URL_VENTAS}/Clientes`, { headers: getAuthHeadersVentas() });
        if (response.ok) {
            const clientes = await response.json();
            const select = document.getElementById("venta-cliente");
            const selectEditar = document.getElementById("editar-cliente");
            if (select) {
                select.innerHTML = '<option value="">Seleccione un cliente</option>';
                clientes.forEach(c => {
                    const option = document.createElement("option");
                    option.value = c.idCliente || c.IdCliente || c.id;
                    const nombre = c.nombre || c.Nombre || c.razonSocial || c.RazonSocial || '';
                    const apellido = c.apellido || c.Apellido || '';
                    option.textContent = `${nombre} ${apellido}`.trim() || `Cliente ${c.idCliente || c.IdCliente}`;
                    select.appendChild(option.cloneNode(true));
                    if (selectEditar) selectEditar.appendChild(option);
                });
            }
        }
    } catch (error) { console.error("Error al cargar clientes:", error); }
}

async function cargarProductosParaSelect() {
    try {
        const response = await fetch(`${API_BASE_URL_VENTAS}/Productos`, { headers: getAuthHeadersVentas() });
        if (response.ok) {
            productosDisponibles = await response.json();
        }
    } catch (error) { console.error("Error al cargar productos:", error); }
}

// === SISTEMA DE MÚLTIPLES PRODUCTOS (NUEVA VENTA) ===
function agregarFilaProducto() {
    contadorFilas++;
    const tbody = document.getElementById("tabla-detalles-venta");
    if (!tbody) return;

    const tr = document.createElement("tr");
    tr.id = `fila-producto-${contadorFilas}`;
    tr.style.borderBottom = "1px solid rgba(255, 255, 255, 0.05)";

    let opcionesProductos = '<option value="">Seleccione un producto</option>';
    productosDisponibles.forEach(p => {
        const id = p.idProducto || p.IdProducto || p.id;
        const nombre = p.nombre || p.Nombre || 'Producto';
        const stock = p.stockActual || p.StockActual || 0;
        const precio = p.precioVenta || p.PrecioVenta || 0;
        opcionesProductos += `<option value="${id}" data-precio="${precio}" data-stock="${stock}" data-nombre="${nombre}">${nombre} - Stock: ${stock} - Bs. ${parseFloat(precio).toFixed(2)}</option>`;
    });

    tr.innerHTML = `
        <td style="padding: 8px;">
            <select class="select-producto" required style="width: 100%; padding: 8px 10px; background: rgba(15, 23, 42, 0.8); border: 1px solid rgba(255, 255, 255, 0.1); border-radius: 6px; color: var(--text-main); font-size: 13px;">
                ${opcionesProductos}
            </select>
        </td>
        <td style="padding: 8px; text-align: center;">
            <input type="number" class="input-cantidad" min="1" value="1" required style="width: 70px; padding: 8px; background: rgba(15, 23, 42, 0.8); border: 1px solid rgba(255, 255, 255, 0.1); border-radius: 6px; color: var(--text-main); font-size: 13px; text-align: center;">
        </td>
        <td style="padding: 8px; text-align: right;">
            <span class="precio-unitario" style="font-size: 13px; color: var(--text-muted);">Bs. 0.00</span>
        </td>
        <td style="padding: 8px; text-align: right;">
            <span class="subtotal-fila" style="font-size: 13px; font-weight: 600; color: var(--accent-color);">Bs. 0.00</span>
        </td>
        <td style="padding: 8px; text-align: center;">
            <button type="button" class="btn-eliminar-fila" style="background: #ef4444; color: white; border: none; width: 28px; height: 28px; border-radius: 6px; cursor: pointer; display: inline-flex; align-items: center; justify-content: center;">
                <i class="fa-solid fa-trash" style="font-size: 12px;"></i>
            </button>
        </td>
    `;

    tbody.appendChild(tr);

    const selectProducto = tr.querySelector('.select-producto');
    const inputCantidad = tr.querySelector('.input-cantidad');
    const btnEliminar = tr.querySelector('.btn-eliminar-fila');

    selectProducto.addEventListener('change', () => {
        const option = selectProducto.options[selectProducto.selectedIndex];
        const precio = parseFloat(option.dataset.precio) || 0;
        const stock = parseInt(option.dataset.stock) || 0;
        tr.querySelector('.precio-unitario').textContent = `Bs. ${precio.toFixed(2)}`;
        inputCantidad.max = stock;
        if (parseInt(inputCantidad.value) > stock) {
            inputCantidad.value = stock;
            Swal.fire({ icon: 'warning', title: 'Stock insuficiente', text: `Solo hay ${stock} unidades disponibles.`, timer: 2000, showConfirmButton: false });
        }
        calcularSubtotalFila(tr);
        calcularTotalGeneral();
    });

    inputCantidad.addEventListener('input', () => {
        const option = selectProducto.options[selectProducto.selectedIndex];
        const stock = parseInt(option.dataset.stock) || 0;
        const cantidad = parseInt(inputCantidad.value) || 0;
        if (cantidad > stock && stock > 0) {
            inputCantidad.value = stock;
            Swal.fire({ icon: 'warning', title: 'Stock insuficiente', text: `Solo hay ${stock} unidades disponibles.`, timer: 2000, showConfirmButton: false });
        }
        calcularSubtotalFila(tr);
        calcularTotalGeneral();
    });

    btnEliminar.addEventListener('click', () => {
        const filas = tbody.querySelectorAll('tr');
        if (filas.length <= 1) {
            Swal.fire({ icon: 'warning', title: 'Mínimo 1 producto', text: 'Debe haber al menos un producto en la venta.', timer: 2000, showConfirmButton: false });
            return;
        }
        tr.remove();
        calcularTotalGeneral();
    });
}

function calcularSubtotalFila(tr) {
    const selectProducto = tr.querySelector('.select-producto');
    const inputCantidad = tr.querySelector('.input-cantidad');
    const spanSubtotal = tr.querySelector('.subtotal-fila');
    if (!selectProducto || !inputCantidad || !spanSubtotal) return;
    const option = selectProducto.options[selectProducto.selectedIndex];
    const precio = parseFloat(option?.dataset.precio) || 0;
    const cantidad = parseInt(inputCantidad.value) || 0;
    spanSubtotal.textContent = `Bs. ${(precio * cantidad).toFixed(2)}`;
}

function calcularTotalGeneral() {
    const tbody = document.getElementById("tabla-detalles-venta");
    if (!tbody) return;
    let total = 0;
    tbody.querySelectorAll('tr').forEach(fila => {
        const spanSubtotal = fila.querySelector('.subtotal-fila');
        if (spanSubtotal) {
            total += parseFloat(spanSubtotal.textContent.replace('Bs. ', '').replace(',', '.')) || 0;
        }
    });
    document.getElementById("total-general").textContent = `Bs. ${total.toFixed(2)}`;
}

// === REGISTRAR VENTA ===
async function registrarVenta() {
    const idCliente = parseInt(document.getElementById("venta-cliente").value);
    const tipoPago = document.getElementById("venta-tipo-pago").value;
    const observaciones = document.getElementById("venta-observaciones")?.value || "";
    
    if (!idCliente) { Swal.fire({ icon: 'warning', title: 'Cliente requerido', text: 'Seleccione un cliente.' }); return; }
    if (!tipoPago) { Swal.fire({ icon: 'warning', title: 'Tipo de pago requerido', text: 'Seleccione un tipo de pago.' }); return; }

    let idUsuario = obtenerIdUsuarioDesdeToken();
    if (!idUsuario || idUsuario === "" || idUsuario === "undefined") {
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo identificar al usuario.' });
        return;
    }

    const tbody = document.getElementById("tabla-detalles-venta");
    const filas = tbody.querySelectorAll('tr');
    const detalles = [];
    let totalGeneral = 0;

    for (const fila of filas) {
        const selectProducto = fila.querySelector('.select-producto');
        const inputCantidad = fila.querySelector('.input-cantidad');
        const idProducto = parseInt(selectProducto.value);
        const cantidad = parseInt(inputCantidad.value);
        const option = selectProducto.options[selectProducto.selectedIndex];
        const precioUnitario = parseFloat(option?.dataset.precio) || 0;
        const subtotal = precioUnitario * cantidad;
        
        if (!idProducto || !cantidad || cantidad <= 0) {
            Swal.fire({ icon: 'warning', title: 'Datos incompletos', text: 'Complete todos los productos.' });
            return;
        }
        detalles.push({ idProducto, cantidad, precioUnitario, subtotal });
        totalGeneral += subtotal;
    }

    if (detalles.length === 0) { Swal.fire({ icon: 'warning', title: 'Sin productos', text: 'Agregue al menos un producto.' }); return; }

    const fechaVenta = new Date().toISOString();
    const ventaDto = {
        IdUsuario: parseInt(idUsuario), IdCliente: idCliente, FechaVenta: fechaVenta,
        TipoPago: tipoPago, Total: parseFloat(totalGeneral.toFixed(2)), Estado: true,
        idUsuario: parseInt(idUsuario), idCliente: idCliente, fechaVenta: fechaVenta,
        tipoPago: tipoPago, total: parseFloat(totalGeneral.toFixed(2)), estado: true,
        observaciones: observaciones, detalles: detalles
    };

    try {
        const response = await fetch(`${API_BASE_URL_VENTAS}/Ventas`, { method: "POST", headers: getAuthHeadersVentas(), body: JSON.stringify(ventaDto) });
        const responseText = await response.text();
        let errData = {};
        try { errData = JSON.parse(responseText); } catch (e) {}
        
        if (response.ok) { 
            Swal.fire({ icon: 'success', title: 'Venta Registrada', text: `Venta por Bs. ${totalGeneral.toFixed(2)} registrada.`, timer: 2000, showConfirmButton: false });
            cargarVentas();
            document.getElementById("modal-venta").classList.add("hidden");
        } else { 
            Swal.fire({ icon: 'error', title: 'Error', text: errData.mensaje || errData.message || responseText || "No se pudo registrar." }); 
        }
    } catch (error) { 
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar.' });
    }
}

// === VER DETALLE DE VENTA ===
async function verDetalleVenta(idVenta) {
    try {
        const response = await fetch(`${API_BASE_URL_VENTAS}/Ventas/${idVenta}`, { headers: getAuthHeadersVentas() });
        if (response.ok) {
            const venta = await response.json();
            const idVentaNum = venta.idVenta || idVenta;
            const cliente = venta.nombreCliente || 'Cliente';
            const fecha = venta.fechaVenta ? new Date(venta.fechaVenta).toLocaleString('es-BO') : '-';
            const tipoPago = venta.tipoPago || '-';
            const estado = venta.estado || 'Completada';
            const estadoColor = estado === 'Completada' ? '#22c55e' : (estado === 'Eliminada' ? '#ef4444' : '#f59e0b');
            
            const detalles = venta.detalles || [];
            let productosHTML = '';
            let totalCalculado = 0;
            
            if (Array.isArray(detalles) && detalles.length > 0) {
                detalles.forEach(d => {
                    const nombreProducto = d.nombreProducto || 'Producto';
                    const cantidad = d.cantidad || 1;
                    const precioUnitario = parseFloat(d.precioUnitario) || 0;
                    const subtotal = parseFloat(d.subtotal) || 0;
                    totalCalculado += subtotal;
                    
                    productosHTML += `
                        <tr>
                            <td style="padding: 12px; border-bottom: 1px solid rgba(255,255,255,0.1);">${nombreProducto}</td>
                            <td style="padding: 12px; text-align: center; border-bottom: 1px solid rgba(255,255,255,0.1);">${cantidad}</td>
                            <td style="padding: 12px; text-align: right; border-bottom: 1px solid rgba(255,255,255,0.1);">Bs. ${precioUnitario.toFixed(2)}</td>
                            <td style="padding: 12px; text-align: right; font-weight: 600; border-bottom: 1px solid rgba(255,255,255,0.1);">Bs. ${subtotal.toFixed(2)}</td>
                        </tr>
                    `;
                });
            } else {
                const total = parseFloat(venta.total || 0);
                productosHTML = `<tr><td colspan="3" style="padding: 12px; text-align: center; color: var(--text-muted);">No hay detalles de productos</td><td style="padding: 12px; text-align: right; font-weight: 600;">Bs. ${total.toFixed(2)}</td></tr>`;
                totalCalculado = total;
            }
            
            const contenido = `
                <div style="background: rgba(15, 23, 42, 0.6); border: 1px solid rgba(255,255,255,0.1); border-radius: 12px; padding: 20px; margin-bottom: 20px;">
                    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 20px;">
                        <div><div style="font-size: 12px; color: var(--text-muted); margin-bottom: 4px;">Cliente</div><div style="font-size: 16px; font-weight: 600;">${cliente}</div></div>
                        <div><div style="font-size: 12px; color: var(--text-muted); margin-bottom: 4px;">Fecha</div><div style="font-size: 16px; font-weight: 600;">${fecha}</div></div>
                        <div><div style="font-size: 12px; color: var(--text-muted); margin-bottom: 4px;">Tipo de Pago</div><div style="font-size: 16px; font-weight: 600;">${tipoPago}</div></div>
                        <div><div style="font-size: 12px; color: var(--text-muted); margin-bottom: 4px;">Estado</div><div><span style="background: ${estadoColor}20; color: ${estadoColor}; padding: 6px 12px; border-radius: 20px; font-weight: 600; font-size: 12px;">${estado}</span></div></div>
                    </div>
                </div>
                <h3 style="margin-bottom: 15px; display: flex; align-items: center; gap: 8px;"><i class="fa-solid fa-cart-shopping" style="color: var(--accent-color);"></i> Productos</h3>
                <div style="background: rgba(15, 23, 42, 0.6); border: 1px solid rgba(255,255,255,0.1); border-radius: 12px; overflow: hidden;">
                    <table style="width: 100%; border-collapse: collapse;">
                        <thead><tr style="background: rgba(2, 132, 199, 0.2);">
                            <th style="padding: 12px; text-align: left; font-size: 13px; color: var(--text-muted);">Producto</th>
                            <th style="padding: 12px; text-align: center; font-size: 13px; color: var(--text-muted);">Cant.</th>
                            <th style="padding: 12px; text-align: right; font-size: 13px; color: var(--text-muted);">Precio</th>
                            <th style="padding: 12px; text-align: right; font-size: 13px; color: var(--text-muted);">Subtotal</th>
                        </tr></thead>
                        <tbody>${productosHTML}</tbody>
                        <tfoot><tr style="background: rgba(34, 197, 94, 0.1);">
                            <td colspan="3" style="padding: 15px; text-align: right; font-size: 16px; font-weight: 700; color: #22c55e;">TOTAL:</td>
                            <td style="padding: 15px; text-align: right; font-size: 20px; font-weight: 700; color: #22c55e;">Bs. ${totalCalculado.toFixed(2)}</td>
                        </tr></tfoot>
                    </table>
                </div>
            `;
            document.getElementById("detalle-titulo").textContent = `Detalle de Venta #${String(idVentaNum).padStart(4, '0')}`;
            document.getElementById("detalle-contenido").innerHTML = contenido;
            document.getElementById("modal-detalle-venta").classList.remove("hidden");
        } else {
            Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el detalle de la venta.' });
        }
    } catch (error) {
        console.error("Error al cargar detalle:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar con el servidor.' });
    }
}

// === EDITAR VENTA ===
async function editarVenta(idVenta) {
    try {
        const response = await fetch(`${API_BASE_URL_VENTAS}/Ventas/${idVenta}`, { headers: getAuthHeadersVentas() });
        if (response.ok) {
            const venta = await response.json();
            document.getElementById("editar-venta-id").value = idVenta;
            document.getElementById("editar-id-venta").textContent = String(idVenta).padStart(4, '0');
            document.getElementById("editar-cliente").value = venta.idCliente || venta.IdCliente;
            document.getElementById("editar-tipo-pago").value = venta.tipoPago || venta.TipoPago || '';
            document.getElementById("editar-observaciones").value = venta.observaciones || venta.Observaciones || '';
            
            const tbody = document.getElementById("tabla-detalles-editar");
            tbody.innerHTML = "";
            contadorFilasEditar = 0;
            
            const detalles = venta.detalles || [];
            if (detalles.length > 0) {
                detalles.forEach(d => { agregarFilaProductoEditar(d.idProducto, d.cantidad, d.precioUnitario, d.subtotal); });
            } else {
                agregarFilaProductoEditar();
            }
            calcularTotalGeneralEditar();
            document.getElementById("modal-editar-venta").classList.remove("hidden");
        } else {
            Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar la venta.' });
        }
    } catch (error) {
        console.error("Error al cargar venta:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar con el servidor.' });
    }
}

function agregarFilaProductoEditar(idProducto = null, cantidad = 1, precioUnitario = 0, subtotal = 0) {
    contadorFilasEditar++;
    const tbody = document.getElementById("tabla-detalles-editar");
    if (!tbody) return;

    const tr = document.createElement("tr");
    tr.id = `fila-editar-${contadorFilasEditar}`;
    tr.style.borderBottom = "1px solid rgba(255, 255, 255, 0.05)";

    let opcionesProductos = '<option value="">Seleccione un producto</option>';
    productosDisponibles.forEach(p => {
        const id = p.idProducto || p.IdProducto || p.id;
        const nombre = p.nombre || p.Nombre || 'Producto';
        const stock = p.stockActual || p.StockActual || 0;
        const precio = p.precioVenta || p.PrecioVenta || 0;
        const selected = id === idProducto ? 'selected' : '';
        opcionesProductos += `<option value="${id}" data-precio="${precio}" data-stock="${stock}" data-nombre="${nombre}" ${selected}>${nombre} - Stock: ${stock} - Bs. ${parseFloat(precio).toFixed(2)}</option>`;
    });

    tr.innerHTML = `
        <td style="padding: 8px;">
            <select class="select-producto-editar" required style="width: 100%; padding: 8px 10px; background: rgba(15, 23, 42, 0.8); border: 1px solid rgba(255, 255, 255, 0.1); border-radius: 6px; color: var(--text-main); font-size: 13px;">
                ${opcionesProductos}
            </select>
        </td>
        <td style="padding: 8px; text-align: center;">
            <input type="number" class="input-cantidad-editar" min="1" value="${cantidad}" required style="width: 70px; padding: 8px; background: rgba(15, 23, 42, 0.8); border: 1px solid rgba(255, 255, 255, 0.1); border-radius: 6px; color: var(--text-main); font-size: 13px; text-align: center;">
        </td>
        <td style="padding: 8px; text-align: right;">
            <span class="precio-unitario-editar" style="font-size: 13px; color: var(--text-muted);">Bs. ${parseFloat(precioUnitario).toFixed(2)}</span>
        </td>
        <td style="padding: 8px; text-align: right;">
            <span class="subtotal-fila-editar" style="font-size: 13px; font-weight: 600; color: var(--accent-color);">Bs. ${parseFloat(subtotal).toFixed(2)}</span>
        </td>
        <td style="padding: 8px; text-align: center;">
            <button type="button" class="btn-eliminar-fila-editar" style="background: #ef4444; color: white; border: none; width: 28px; height: 28px; border-radius: 6px; cursor: pointer; display: inline-flex; align-items: center; justify-content: center;">
                <i class="fa-solid fa-trash" style="font-size: 12px;"></i>
            </button>
        </td>
    `;

    tbody.appendChild(tr);

    const selectProducto = tr.querySelector('.select-producto-editar');
    const inputCantidad = tr.querySelector('.input-cantidad-editar');
    const btnEliminar = tr.querySelector('.btn-eliminar-fila-editar');

    selectProducto.addEventListener('change', () => {
        const option = selectProducto.options[selectProducto.selectedIndex];
        const precio = parseFloat(option.dataset.precio) || 0;
        tr.querySelector('.precio-unitario-editar').textContent = `Bs. ${precio.toFixed(2)}`;
        calcularSubtotalFilaEditar(tr);
        calcularTotalGeneralEditar();
    });

    inputCantidad.addEventListener('input', () => {
        calcularSubtotalFilaEditar(tr);
        calcularTotalGeneralEditar();
    });

    btnEliminar.addEventListener('click', () => {
        const filas = tbody.querySelectorAll('tr');
        if (filas.length <= 1) {
            Swal.fire({ icon: 'warning', title: 'Mínimo 1 producto', text: 'Debe haber al menos un producto.', timer: 2000, showConfirmButton: false });
            return;
        }
        tr.remove();
        calcularTotalGeneralEditar();
    });
}

function calcularSubtotalFilaEditar(tr) {
    const selectProducto = tr.querySelector('.select-producto-editar');
    const inputCantidad = tr.querySelector('.input-cantidad-editar');
    const spanSubtotal = tr.querySelector('.subtotal-fila-editar');
    if (!selectProducto || !inputCantidad || !spanSubtotal) return;
    const option = selectProducto.options[selectProducto.selectedIndex];
    const precio = parseFloat(option?.dataset.precio) || 0;
    const cantidad = parseInt(inputCantidad.value) || 0;
    spanSubtotal.textContent = `Bs. ${(precio * cantidad).toFixed(2)}`;
}

function calcularTotalGeneralEditar() {
    const tbody = document.getElementById("tabla-detalles-editar");
    if (!tbody) return;
    let total = 0;
    tbody.querySelectorAll('tr').forEach(fila => {
        const spanSubtotal = fila.querySelector('.subtotal-fila-editar');
        if (spanSubtotal) {
            total += parseFloat(spanSubtotal.textContent.replace('Bs. ', '').replace(',', '.')) || 0;
        }
    });
    document.getElementById("editar-total-general").textContent = `Bs. ${total.toFixed(2)}`;
}

async function guardarEdicionVenta() {
    const idVenta = parseInt(document.getElementById("editar-venta-id").value);
    const idCliente = parseInt(document.getElementById("editar-cliente").value);
    const tipoPago = document.getElementById("editar-tipo-pago").value;
    const observaciones = document.getElementById("editar-observaciones")?.value || "";
    
    if (!idCliente || !tipoPago) {
        Swal.fire({ icon: 'warning', title: 'Datos incompletos', text: 'Complete cliente y tipo de pago.' });
        return;
    }

    let idUsuario = obtenerIdUsuarioDesdeToken();
    if (!idUsuario || idUsuario === "" || idUsuario === "undefined") {
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo identificar al usuario.' });
        return;
    }

    const tbody = document.getElementById("tabla-detalles-editar");
    const filas = tbody.querySelectorAll('tr');
    const detalles = [];
    let totalGeneral = 0;

    for (const fila of filas) {
        const selectProducto = fila.querySelector('.select-producto-editar');
        const inputCantidad = fila.querySelector('.input-cantidad-editar');
        const idProducto = parseInt(selectProducto.value);
        const cantidad = parseInt(inputCantidad.value);
        const option = selectProducto.options[selectProducto.selectedIndex];
        const precioUnitario = parseFloat(option?.dataset.precio) || 0;
        const subtotal = precioUnitario * cantidad;
        
        if (!idProducto || !cantidad || cantidad <= 0) {
            Swal.fire({ icon: 'warning', title: 'Datos incompletos', text: 'Complete todos los productos.' });
            return;
        }
        detalles.push({ idProducto, cantidad, precioUnitario, subtotal });
        totalGeneral += subtotal;
    }

    if (detalles.length === 0) {
        Swal.fire({ icon: 'warning', title: 'Sin productos', text: 'Agregue al menos un producto.' });
        return;
    }

    const fechaVenta = new Date().toISOString();
    const ventaDto = {
        IdUsuario: parseInt(idUsuario), IdCliente: idCliente, FechaVenta: fechaVenta,
        TipoPago: tipoPago, Total: parseFloat(totalGeneral.toFixed(2)), Estado: true,
        observaciones: observaciones, detalles: detalles
    };

    try {
        const response = await fetch(`${API_BASE_URL_VENTAS}/Ventas/${idVenta}`, { method: "PUT", headers: getAuthHeadersVentas(), body: JSON.stringify(ventaDto) });
        const responseText = await response.text();
        let errData = {};
        try { errData = JSON.parse(responseText); } catch (e) {}
        
        if (response.ok) { 
            Swal.fire({ icon: 'success', title: 'Venta Actualizada', text: `Venta #${String(idVenta).padStart(4, '0')} actualizada.`, timer: 2000, showConfirmButton: false });
            cargarVentas();
            document.getElementById("modal-editar-venta").classList.add("hidden");
        } else { 
            Swal.fire({ icon: 'error', title: 'Error', text: errData.mensaje || errData.message || responseText || "No se pudo actualizar." }); 
        }
    } catch (error) { 
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar.' });
    }
}

// === VER HISTORIAL (Solo Admin) ===
async function verHistorialVenta(idVenta) {
    const rolId = obtenerRolIdVentas();
    if (rolId !== 1) {
        Swal.fire({ icon: 'warning', title: 'Acceso denegado', text: 'Solo el administrador puede ver el historial.', confirmButtonColor: '#3b82f6' });
        return;
    }

    try {
        // 1. Cargar historial
        const response = await fetch(`${API_BASE_URL_VENTAS}/Ventas/${idVenta}/historial`, { headers: getAuthHeadersVentas() });
        if (!response.ok) {
            Swal.fire({ icon: 'info', title: 'Sin historial', text: 'No hay historial disponible.' });
            return;
        }
        const historial = await response.json();
        
        // 2. Cargar TODOS los usuarios (el admin tiene permiso)
        let usuariosMap = {};
        try {
            const usuariosRes = await fetch(`${API_BASE_URL_VENTAS}/Usuarios`, { headers: getAuthHeadersVentas() });
            if (usuariosRes.ok) {
                const usuarios = await usuariosRes.json();
                usuarios.forEach(u => {
                    const uid = u.idUsuario || u.IdUsuario;
                    // Construir nombre completo con todos los formatos posibles
                    const nombre = u.personaNombre || u.PersonaNombre || u.nombre || u.Nombre || u.nombreUsuario || u.NombreUsuario || '';
                    const apPat = u.apPat || u.ApPat || '';
                    const apMat = u.apMat || u.ApMat || '';
                    const completo = `${nombre} ${apPat} ${apMat}`.trim();
                    usuariosMap[uid] = completo || `Usuario ${uid}`;
                });
            }
        } catch (e) {
            console.warn("No se pudieron cargar usuarios:", e);
        }

        // 3. Renderizar historial
        document.getElementById("historial-id-venta").textContent = String(idVenta).padStart(4, '0');
        
        let html = '';
        if (historial.length === 0) {
            html = '<p style="text-align:center; color:var(--text-muted); padding:40px;">Sin historial</p>';
        } else {
            historial.sort((a, b) => new Date(b.FechaModificacion) - new Date(a.FechaModificacion));
            
            historial.forEach(h => {
                // Obtener nombre del usuario
                const uid = h.IdUsuarioModifico || h.idUsuarioModifico;
                let nombreUsuario = h.NombreUsuarioModifico || '';
                if (!nombreUsuario && uid && usuariosMap[uid]) {
                    nombreUsuario = usuariosMap[uid];
                }
                if (!nombreUsuario && uid) {
                    nombreUsuario = `Usuario #${uid}`;
                }
                if (!nombreUsuario) {
                    nombreUsuario = 'Desconocido';
                }

                const fecha = h.FechaModificacion ? new Date(h.FechaModificacion).toLocaleString('es-BO') : '-';
                const tipo = h.TipoCambio || 'MODIFICACIÓN';
                const color = tipo === 'CREACIÓN' ? '#22c55e' : (tipo === 'ELIMINACIÓN' ? '#ef4444' : '#f59e0b');

                const totalAntes = (h.DetallesAntes || []).reduce((s, d) => s + (parseFloat(d.Subtotal) || 0), 0);
                const totalDespues = (h.DetallesDespues || []).reduce((s, d) => s + (parseFloat(d.Subtotal) || 0), 0);

                const renderTabla = (detalles, esDespues) => {
                    if (!detalles || detalles.length === 0) return '<p style="font-size:12px; color:var(--text-muted); text-align:center; padding:10px;">Sin detalles</p>';
                    const colorTexto = esDespues ? 'var(--text-main)' : 'var(--text-muted)';
                    return `<table style="width:100%; font-size:12px; color:${colorTexto};">
                        <thead><tr style="border-bottom:1px solid rgba(255,255,255,0.1);">
                            <th style="text-align:left; padding:4px;">Producto</th>
                            <th style="text-align:center; padding:4px;">Cant</th>
                            <th style="text-align:right; padding:4px;">Subtotal</th>
                        </tr></thead>
                        <tbody>${detalles.map(d => `
                            <tr>
                                <td style="padding:4px;">${d.NombreProducto || 'Producto'}</td>
                                <td style="text-align:center; padding:4px;">${d.Cantidad}</td>
                                <td style="text-align:right; padding:4px;">Bs. ${parseFloat(d.Subtotal).toFixed(2)}</td>
                            </tr>`).join('')}
                        </tbody>
                    </table>`;
                };

                html += `
                    <div style="background:rgba(15,23,42,0.6); border:1px solid rgba(255,255,255,0.1); border-left:4px solid ${color}; border-radius:12px; padding:20px; margin-bottom:20px;">
                        <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:15px; flex-wrap:wrap; gap:10px;">
                            <div style="display:flex; align-items:center; gap:10px;">
                                <div style="width:36px; height:36px; background:rgba(56,189,248,0.2); border-radius:50%; display:flex; align-items:center; justify-content:center; font-weight:700; color:var(--accent-color); font-size:14px;">
                                    ${nombreUsuario.charAt(0).toUpperCase()}
                                </div>
                                <div>
                                    <div style="font-size:14px; font-weight:600; color:var(--text-main);">${nombreUsuario}</div>
                                    <div style="font-size:12px; color:var(--text-muted);">${fecha}</div>
                                </div>
                            </div>
                            <span style="background:${color}; color:white; padding:4px 12px; border-radius:20px; font-size:11px; font-weight:700; text-transform:uppercase;">${tipo}</span>
                        </div>
                        <div style="display:grid; grid-template-columns:1fr 1fr; gap:15px;">
                            <div style="background:rgba(239,68,68,0.05); border:1px solid rgba(239,68,68,0.2); border-radius:8px; padding:15px;">
                                <div style="font-size:12px; font-weight:700; color:#ef4444; margin-bottom:10px; display:flex; justify-content:space-between;">
                                    <span>← ANTES</span><span>Bs. ${totalAntes.toFixed(2)}</span>
                                </div>
                                ${renderTabla(h.DetallesAntes, false)}
                            </div>
                            <div style="background:rgba(34,197,94,0.05); border:1px solid rgba(34,197,94,0.2); border-radius:8px; padding:15px;">
                                <div style="font-size:12px; font-weight:700; color:#22c55e; margin-bottom:10px; display:flex; justify-content:space-between;">
                                    <span>→ DESPUÉS</span><span>Bs. ${totalDespues.toFixed(2)}</span>
                                </div>
                                ${renderTabla(h.DetallesDespues, true)}
                            </div>
                        </div>
                        ${h.Observaciones ? `<div style="margin-top:15px; padding:10px; background:rgba(255,255,255,0.03); border-radius:6px; font-size:13px; color:var(--text-muted); border-left:3px solid var(--accent-color);"><strong>Observaciones:</strong> ${h.Observaciones}</div>` : ''}
                    </div>
                `;
            });
        }
        
        document.getElementById("historial-contenido").innerHTML = html;
        document.getElementById("modal-historial").classList.remove("hidden");
    } catch (error) {
        console.error("Error:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el historial.' });
    }
}

// === ELIMINAR VENTA ===
async function eliminarVenta(idVenta) {
    const result = await Swal.fire({
        title: '¿Eliminar venta?',
        text: `La venta #${String(idVenta).padStart(4, '0')} será marcada como eliminada.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#64748b',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    });
    if (!result.isConfirmed) return;
    try {
        const response = await fetch(`${API_BASE_URL_VENTAS}/Ventas/${idVenta}`, { method: "DELETE", headers: getAuthHeadersVentas() });
        if (response.ok) {
            Swal.fire({ icon: 'success', title: 'Venta Eliminada', text: 'La venta ha sido eliminada.', timer: 2000, showConfirmButton: false });
            cargarVentas();
        } else {
            const err = await response.json();
            Swal.fire({ icon: 'error', title: 'Error', text: err.mensaje || "No se pudo eliminar la venta." });
        }
    } catch (error) {
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar con el servidor.' });
    }
}

function cerrarSesion() {
    localStorage.clear();
    window.location.href = 'index.html';
}