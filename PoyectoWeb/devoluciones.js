alert("✅ DEVOLUCIONES.JS SE ESTÁ CARGANDO CORRECTAMENTE");
console.log("=== DEVOLUCIONES.JS INICIADO ===");

// ... el resto de tu código sigue igual ...
const API_BASE_URL = "https://localhost:7067/api";

// === VERIFICAR AUTENTICACIÓN ===
if (!localStorage.getItem('authToken')) {
    window.location.href = 'index.html';
}

// === CARGAR INFORMACIÓN DEL USUARIO EN EL HEADER ===
document.addEventListener('DOMContentLoaded', () => {
    const rolId = parseInt(localStorage.getItem('usuarioRolId') || '0');
    const nombreUsuario = localStorage.getItem('usuarioNombre') || 'Usuario';
    const rolNombre = localStorage.getItem('usuarioRolNombre') || 'Usuario';

    document.getElementById('user-name').textContent = nombreUsuario;
    document.getElementById('user-role').textContent = rolNombre;
    document.getElementById('user-avatar').textContent = nombreUsuario.trim().charAt(0).toUpperCase();

    // Verificar permisos: Solo Admin (1) y Ventas (6) pueden acceder
    if (rolId !== 1 && rolId !== 6) {
        Swal.fire({
            icon: 'error',
            title: 'Acceso Denegado',
            text: 'No tienes permisos para acceder a esta sección.',
            confirmButtonColor: '#3b82f6'
        }).then(() => {
            window.location.href = 'dashboard.html';
        });
        return;
    }

    // Controlar visibilidad del botón "Nueva Devolución"
    const btnNueva = document.querySelector('.btn-primary-sm');
    if (btnNueva) {
        if (rolId === 1) {
            // Admin NO puede crear devoluciones
            btnNueva.style.display = 'none';
        } else if (rolId === 6) {
            // Ventas SÍ puede crear
            btnNueva.style.display = 'block';
        }
    }

    // Cargar devoluciones al iniciar
    cargarDevolucionesAPI();
});

// === HEADERS DE AUTENTICACIÓN ===
function getAuthHeaders() {
    const token = localStorage.getItem("authToken");
    return {
        "Content-Type": "application/json",
        "Authorization": token ? `Bearer ${token}` : ""
    };
}

// === CERRAR SESIÓN ===
function cerrarSesion() {
    localStorage.clear();
    window.location.href = 'index.html';
}

// === CARGAR DEVOLUCIONES ===
async function cargarDevolucionesAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/Devoluciones`, { headers: getAuthHeaders() });
        if (response.ok) {
            const devoluciones = await response.json();
            const tbody = document.getElementById("tabla-devoluciones");
            if (!tbody) return;
            tbody.innerHTML = "";

            const rolId = parseInt(localStorage.getItem('usuarioRolId') || '0');

            if (devoluciones.length === 0) {
                tbody.innerHTML = `<tr><td colspan="9" style="text-align: center; color: #94a3b8; padding: 40px;">
                    <i class="fa-solid fa-circle-info" style="font-size: 48px; margin-bottom: 10px; display: block; opacity: 0.5;"></i>
                    No hay devoluciones registradas
                </td></tr>`;
                return;
            }

            devoluciones.forEach(d => {
                const tr = document.createElement("tr");
                const estado = d.estado || d.Estado || 'Pendiente';
                const estadoColor = estado === 'Aprobada' ? '#22c55e' : (estado === 'Rechazada' ? '#ef4444' : '#f59e0b');
                const idDev = d.idDevolucion || d.IdDevolucion;
                const idVenta = d.idVenta || d.IdVenta || 0;

                // Solo Admin (rol 1) puede aprobar/rechazar devoluciones pendientes
                const puedeGestionar = (rolId === 1 && estado === 'Pendiente');

                tr.innerHTML = `
                    <td><strong>#${idDev.toString().padStart(4, '0')}</strong></td>
                    <td>VNT-${idVenta.toString().padStart(4, '0')}</td>
                    <td>${d.nombreCliente || d.clienteNombre || 'Cliente'}</td>
                    <td>${d.nombreProducto || d.productoNombre || 'Producto'}</td>
                    <td>${d.cantidad || d.Cantidad || 0}</td>
                    <td style="max-width: 200px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;" title="${d.motivo || d.Motivo || ''}">${d.motivo || d.Motivo || '-'}</td>
                    <td>${d.fechaDevolucion ? new Date(d.fechaDevolucion).toLocaleDateString('es-BO') : '-'}</td>
                    <td><span style="background: ${estadoColor}20; color: ${estadoColor}; padding: 4px 10px; border-radius: 12px; font-size: 11px; font-weight: 600;">${estado}</span></td>
                    <td>
                        <div class="acciones-cell">
                            <button class="btn-action btn-info" onclick="verDetalleDevolucion(${idDev})" style="background: #0ea5e9; color: white;" title="Ver detalle">
                                <i class="fa-solid fa-eye"></i>
                            </button>
                            ${puedeGestionar ? `
                                <button class="btn-action btn-edit" onclick="aprobarDevolucion(${idDev})" style="background: #22c55e; color: white;" title="Aprobar">
                                    <i class="fa-solid fa-check"></i>
                                </button>
                                <button class="btn-action btn-delete" onclick="rechazarDevolucion(${idDev})" style="background: #ef4444; color: white;" title="Rechazar">
                                    <i class="fa-solid fa-xmark"></i>
                                </button>
                            ` : ''}
                        </div>
                    </td>
                `;
                tbody.appendChild(tr);
            });
        } else {
            console.error("Error HTTP al cargar devoluciones:", response.status);
            const tbody = document.getElementById("tabla-devoluciones");
            if (tbody) tbody.innerHTML = `<tr><td colspan="9" style="text-align:center; padding: 20px; color: #ef4444;">Error al cargar datos (Status: ${response.status})</td></tr>`;
        }
    } catch (error) {
        console.error("❌ Error de red al cargar devoluciones:", error);
    }
}

// === ABRIR MODAL PARA NUEVA DEVOLUCIÓN ===
async function abrirModalDevolucion() {
    try {
        const ventasRes = await fetch(`${API_BASE_URL}/Ventas`, { headers: getAuthHeaders() });
        const ventas = ventasRes.ok ? await ventasRes.json() : [];
        const ventasCompletadas = ventas.filter(v => v.estado === 'Completada' || v.Estado === true);

        Swal.fire({
            title: 'Nueva Devolución',
            html: `
                <div style="text-align: left; max-width: 600px;">
                    <div style="margin-bottom: 15px;">
                        <label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Venta *</label>
                        <select id="dev-venta" class="swal2-input" style="width: 100%; margin: 0;">
                            <option value="">Seleccione una venta...</option>
                            ${ventasCompletadas.map(v => `
                                <option value="${v.idVenta || v.IdVenta}" 
                                        data-producto="${v.productoNombre || ''}"
                                        data-cantidad="${v.cantidad || 0}">
                                    VNT-${(v.idVenta || v.IdVenta).toString().padStart(4, '0')} - ${v.nombreCliente || 'Cliente'}
                                </option>
                            `).join('')}
                        </select>
                    </div>
                    <div style="margin-bottom: 15px;">
                        <label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Producto *</label>
                        <input type="text" id="dev-producto" class="swal2-input" readonly style="width: 100%; margin: 0; background: rgba(255,255,255,0.05);">
                    </div>
                    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 15px;">
                        <div>
                            <label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Cantidad a devolver *</label>
                            <input type="number" id="dev-cantidad" class="swal2-input" min="1" style="width: 100%; margin: 0;">
                        </div>
                        <div>
                            <label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Fecha *</label>
                            <input type="date" id="dev-fecha" class="swal2-input" style="width: 100%; margin: 0;">
                        </div>
                    </div>
                    <div style="margin-bottom: 15px;">
                        <label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Motivo *</label>
                        <select id="dev-motivo" class="swal2-input" style="width: 100%; margin: 0;">
                            <option value="">Seleccione un motivo...</option>
                            <option value="Producto defectuoso">Producto defectuoso</option>
                            <option value="Producto incorrecto">Producto incorrecto</option>
                            <option value="Producto dañado en transporte">Producto dañado en transporte</option>
                            <option value="Cliente arrepentido">Cliente arrepentido</option>
                            <option value="Otro">Otro</option>
                        </select>
                    </div>
                    <div style="margin-bottom: 15px;">
                        <label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Observaciones</label>
                        <textarea id="dev-observaciones" class="swal2-textarea" placeholder="Detalles adicionales..." style="width: 100%; margin: 0;"></textarea>
                    </div>
                </div>
            `,
            width: '700px',
            showCancelButton: true,
            confirmButtonText: 'Registrar Devolución',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#3b82f6',
            cancelButtonColor: '#64748b',
            didOpen: () => {
                document.getElementById('dev-fecha').value = new Date().toISOString().split('T')[0];
                document.getElementById('dev-venta').addEventListener('change', function() {
                    const option = this.options[this.selectedIndex];
                    document.getElementById('dev-producto').value = option.dataset.producto || '';
                    document.getElementById('dev-cantidad').max = option.dataset.cantidad || 1;
                });
            },
            preConfirm: () => {
                const idVenta = document.getElementById('dev-venta').value;
                const producto = document.getElementById('dev-producto').value;
                const cantidad = parseInt(document.getElementById('dev-cantidad').value);
                const fecha = document.getElementById('dev-fecha').value;
                const motivo = document.getElementById('dev-motivo').value;
                const observaciones = document.getElementById('dev-observaciones').value;

                if (!idVenta) { Swal.showValidationMessage('Debe seleccionar una venta'); return false; }
                if (!producto) { Swal.showValidationMessage('Debe haber un producto'); return false; }
                if (!cantidad || cantidad < 1) { Swal.showValidationMessage('La cantidad debe ser mayor a 0'); return false; }
                if (!fecha) { Swal.showValidationMessage('Debe seleccionar una fecha'); return false; }
                if (!motivo) { Swal.showValidationMessage('Debe seleccionar un motivo'); return false; }

                return { idVenta, producto, cantidad, fecha, motivo, observaciones };
            }
        }).then((result) => {
            if (result.isConfirmed) registrarDevolucion(result.value);
        });
    } catch (error) {
        console.error("Error al abrir modal:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos.' });
    }
}

// === REGISTRAR DEVOLUCIÓN ===
async function registrarDevolucion(data) {
    try {
        const response = await fetch(`${API_BASE_URL}/Devoluciones`, {
            method: 'POST',
            headers: { ...getAuthHeaders(), 'Content-Type': 'application/json' },
            body: JSON.stringify({
                idVenta: parseInt(data.idVenta),
                cantidad: data.cantidad,
                motivo: data.motivo,
                fechaDevolucion: new Date(data.fecha).toISOString(),
                observaciones: data.observaciones,
                estado: 'Pendiente'
            })
        });

        if (response.ok) {
            Swal.fire({
                icon: 'success',
                title: 'Devolución Registrada',
                text: 'La devolución se ha registrado correctamente.',
                timer: 2000,
                showConfirmButton: false
            });
            cargarDevolucionesAPI();
        } else {
            const error = await response.json();
            Swal.fire({ icon: 'error', title: 'Error', text: error.mensaje || 'No se pudo registrar la devolución.' });
        }
    } catch (error) {
        console.error("Error al registrar devolución:", error);
        Swal.fire({ icon: 'error', title: 'Error de Conexión', text: 'No se pudo conectar con el servidor.' });
    }
}

// === VER DETALLE DE DEVOLUCIÓN ===
async function verDetalleDevolucion(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/Devoluciones/${id}`, { headers: getAuthHeaders() });
        if (!response.ok) throw new Error('No se pudo cargar');

        const d = await response.json();
        const estado = d.estado || d.Estado || 'Pendiente';
        const estadoColor = estado === 'Aprobada' ? '#22c55e' : (estado === 'Rechazada' ? '#ef4444' : '#f59e0b');

        Swal.fire({
            title: `<strong>Devolución #${(d.idDevolucion || d.IdDevolucion).toString().padStart(4, '0')}</strong>`,
            html: `
                <div style="text-align: left; max-width: 600px; color: #f8fafc;">
                    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 20px; background: rgba(56, 189, 248, 0.05); padding: 20px; border-radius: 10px;">
                        <div>
                            <label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Venta</label>
                            <div style="color: #f8fafc; font-weight: 600;">VNT-${(d.idVenta || d.IdVenta || 0).toString().padStart(4, '0')}</div>
                        </div>
                        <div>
                            <label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Fecha</label>
                            <div style="color: #f8fafc;">${d.fechaDevolucion ? new Date(d.fechaDevolucion).toLocaleString('es-BO') : '-'}</div>
                        </div>
                        <div>
                            <label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Cliente</label>
                            <div style="color: #f8fafc; font-weight: 600;">${d.nombreCliente || d.clienteNombre || '-'}</div>
                        </div>
                        <div>
                            <label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Estado</label>
                            <span style="background: ${estadoColor}20; color: ${estadoColor}; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: 600;">${estado}</span>
                        </div>
                    </div>
                    <div style="background: rgba(30, 41, 59, 0.5); border-radius: 10px; padding: 15px;">
                        <h4 style="color: #38bdf8; margin: 0 0 10px 0; font-size: 14px;"><i class="fa-solid fa-box"></i> Producto</h4>
                        <div style="font-size: 14px; margin-bottom: 8px;"><strong>${d.nombreProducto || d.productoNombre || 'Producto'}</strong></div>
                        <div style="font-size: 13px; color: #94a3b8;">Cantidad devuelta: <strong style="color: #f8fafc;">${d.cantidad || d.Cantidad || 0}</strong></div>
                    </div>
                    <div style="background: rgba(245, 158, 11, 0.05); border-radius: 10px; padding: 15px; margin-top: 15px;">
                        <h4 style="color: #f59e0b; margin: 0 0 10px 0; font-size: 14px;"><i class="fa-solid fa-triangle-exclamation"></i> Motivo</h4>
                        <div style="font-size: 13px; color: #f8fafc;">${d.motivo || d.Motivo || '-'}</div>
                    </div>
                    ${d.observaciones ? `
                        <div style="background: rgba(255,255,255,0.02); border-radius: 10px; padding: 15px; margin-top: 15px;">
                            <h4 style="color: #94a3b8; margin: 0 0 10px 0; font-size: 14px;"><i class="fa-solid fa-comment"></i> Observaciones</h4>
                            <div style="font-size: 13px; color: #f8fafc;">${d.observaciones}</div>
                        </div>
                    ` : ''}
                </div>
            `,
            width: '700px',
            confirmButtonText: 'Cerrar',
            confirmButtonColor: '#64748b',
            showCloseButton: true
        });
    } catch (error) {
        console.error("Error al cargar detalle:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el detalle.' });
    }
}

// === APROBAR DEVOLUCIÓN (Solo Admin) ===
async function aprobarDevolucion(id) {
    const result = await Swal.fire({
        title: '¿Aprobar devolución?',
        text: "La devolución será marcada como aprobada y se procesará el reembolso.",
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#22c55e',
        cancelButtonColor: '#64748b',
        confirmButtonText: 'Sí, aprobar',
        cancelButtonText: 'Cancelar'
    });

    if (!result.isConfirmed) return;

    try {
        const response = await fetch(`${API_BASE_URL}/Devoluciones/${id}/aprobar`, {
            method: 'PUT',
            headers: getAuthHeaders()
        });

        if (response.ok) {
            Swal.fire({
                icon: 'success',
                title: 'Devolución Aprobada',
                text: 'La devolución ha sido aprobada exitosamente.',
                timer: 2000,
                showConfirmButton: false
            });
            cargarDevolucionesAPI();
        } else {
            const error = await response.json();
            Swal.fire({ icon: 'error', title: 'Error', text: error.mensaje || 'No se pudo aprobar la devolución.' });
        }
    } catch (error) {
        console.error("Error al aprobar:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar con el servidor.' });
    }
}

// === RECHAZAR DEVOLUCIÓN (Solo Admin) ===
async function rechazarDevolucion(id) {
    const result = await Swal.fire({
        title: '¿Rechazar devolución?',
        text: "La devolución será marcada como rechazada.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#64748b',
        confirmButtonText: 'Sí, rechazar',
        cancelButtonText: 'Cancelar'
    });

    if (!result.isConfirmed) return;

    try {
        const response = await fetch(`${API_BASE_URL}/Devoluciones/${id}/rechazar`, {
            method: 'PUT',
            headers: getAuthHeaders()
        });

        if (response.ok) {
            Swal.fire({
                icon: 'success',
                title: 'Devolución Rechazada',
                text: 'La devolución ha sido rechazada.',
                timer: 2000,
                showConfirmButton: false
            });
            cargarDevolucionesAPI();
        } else {
            const error = await response.json();
            Swal.fire({ icon: 'error', title: 'Error', text: error.mensaje || 'No se pudo rechazar la devolución.' });
        }
    } catch (error) {
        console.error("Error al rechazar:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar con el servidor.' });
    }
}