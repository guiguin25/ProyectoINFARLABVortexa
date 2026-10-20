function getAuthHeaders() {
    const token = localStorage.getItem("authToken");
    return { 
        "Content-Type": "application/json", 
        "Authorization": token ? `Bearer ${token}` : "" 
    };
}

// === NAVEGACIÓN DE TABS ===
function cambiarTab(tabName, evt) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
    
    const tabElement = document.getElementById(`tab-${tabName}`);
    if (tabElement) tabElement.classList.add('active');
    
    if (evt && evt.target) {
        evt.target.closest('.tab-button').classList.add('active');
    } else {
        document.querySelector(`button[onclick*="cambiarTab('${tabName}'"]`)?.classList.add('active');
    }
    
    if (tabName === 'clientes') cargarClientesAPI();
    else if (tabName === 'proveedores') cargarProveedoresAPI();
    else if (tabName === 'ventas') cargarVentasAPI();
    else if (tabName === 'compras') cargarComprasAPI();
    else if (tabName === 'devoluciones') cargarDevolucionesAPI();
}

// === PERMISOS DEVOLUCIONES ===
function verificarPermisosDevoluciones() {
    const rolId = parseInt(localStorage.getItem('usuarioRolId') || '0');
    const tabBtn = document.getElementById('tab-btn-devoluciones');
    const btnNuevaDevolucion = document.getElementById('btn-nueva-devolucion');
    
    console.log("=== VERIFICANDO PERMISOS DEVOLUCIONES ===");
    console.log("Rol ID detectado:", rolId);
    console.log("Botón pestaña encontrado:", tabBtn !== null);
    console.log("Botón nueva devolución encontrado:", btnNuevaDevolucion !== null);
    
    if (tabBtn) {
        if (rolId === 1 || rolId === 6) {
            console.log("✅ Rol permitido (Admin o Ventas). Mostrando pestaña.");
            tabBtn.style.display = 'block';
            
            if (btnNuevaDevolucion) {
                if (rolId === 6) {
                    console.log("✅ Rol Ventas (6). Mostrando botón NUEVA DEVOLUCIÓN.");
                    btnNuevaDevolucion.style.display = 'block';
                } else {
                    console.log("❌ Rol Admin (1). OCULTANDO botón NUEVA DEVOLUCIÓN.");
                    btnNuevaDevolucion.style.display = 'none';
                }
            } else {
                console.error("❌ ERROR: No se encontró el botón #btn-nueva-devolucion");
            }
        } else {
            console.log(" Rol no permitido. Ocultando pestaña.");
            tabBtn.style.display = 'none';
        }
    } else {
        console.error("❌ ERROR: No se encontró el botón #tab-btn-devoluciones");
    }
}

// === CLIENTES ===
async function cargarClientesAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/Clientes`, { headers: getAuthHeaders() });
        if (response.ok) {
            const clientes = await response.json();
            const tbody = document.getElementById("tabla-clientes");
            if (!tbody) return;
            tbody.innerHTML = "";
            clientes.forEach(cliente => {
                const tr = document.createElement("tr");
                tr.innerHTML = `
                    <td>${cliente.idCliente || cliente.IdCliente}</td>
                    <td>${cliente.nombre || cliente.Nombre || 'Sin Nombre'}</td>
                    <td>${cliente.telefono || cliente.Telefono || '-'}</td>
                    <td style="color: #f8fafc;">${cliente.email || cliente.Email || '-'}</td>
                    <td>${cliente.direccion || cliente.Direccion || '-'}</td>
                    <td>
                        <button class="btn-action btn-edit" style="padding: 6px 10px; border: none; border-radius: 4px; cursor: pointer; margin-right: 5px;"><i class="fa-solid fa-pen"></i></button>
                        <button class="btn-action btn-delete" style="padding: 6px 10px; border: none; border-radius: 4px; cursor: pointer;"><i class="fa-solid fa-trash"></i></button>
                    </td>`;
                tbody.appendChild(tr);
            });
        }
    } catch (error) { console.error("Error al cargar clientes:", error); }
}
function abrirModalCliente() { Swal.fire({ title: 'Nuevo Cliente', text: 'Funcionalidad en desarrollo', icon: 'info' }); }

// === PROVEEDORES ===
async function cargarProveedoresAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/Proveedores`, { headers: getAuthHeaders() });
        if (response.ok) {
            const proveedores = await response.json();
            const tbody = document.getElementById("tabla-proveedores");
            if (!tbody) return;
            tbody.innerHTML = "";
            proveedores.forEach(prov => {
                const tr = document.createElement("tr");
                tr.innerHTML = `
                    <td>${prov.idProveedor || prov.IdProveedor}</td>
                    <td>${prov.nombre || prov.Nombre || 'Sin Nombre'}</td>
                    <td>${prov.telefono || prov.Telefono || '-'}</td>
                    <td style="color: #f8fafc;">${prov.email || prov.Email || '-'}</td>
                    <td>${prov.direccion || prov.Direccion || '-'}</td>
                    <td>
                        <button class="btn-action btn-edit" style="padding: 6px 10px; border: none; border-radius: 4px; cursor: pointer; margin-right: 5px;"><i class="fa-solid fa-pen"></i></button>
                        <button class="btn-action btn-delete" style="padding: 6px 10px; border: none; border-radius: 4px; cursor: pointer;"><i class="fa-solid fa-trash"></i></button>
                    </td>`;
                tbody.appendChild(tr);
            });
        }
    } catch (error) { console.error("Error al cargar proveedores:", error); }
}
function abrirModalProveedor() { Swal.fire({ title: 'Nuevo Proveedor', text: 'Funcionalidad en desarrollo', icon: 'info' }); }

// === VENTAS ===
async function cargarVentasAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/Ventas`, { headers: getAuthHeaders() });
        if (response.ok) {
            const ventas = await response.json();
            const tbody = document.getElementById("tabla-ventas");
            if (!tbody) return;
            tbody.innerHTML = "";
            
            if (ventas.length === 0) {
                tbody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: #94a3b8; padding: 40px;"><i class="fa-solid fa-circle-info" style="font-size: 48px; margin-bottom: 10px; display: block; opacity: 0.5;"></i>No hay ventas registradas</td></tr>`;
                return;
            }
            
            ventas.forEach(venta => {
                const tr = document.createElement("tr");
                const estadoTexto = venta.estado || (venta.Estado ? 'Completada' : 'Eliminada');
                const estaEliminada = estadoTexto === 'Eliminada';
                
                if (estaEliminada) tr.classList.add('fila-eliminada');
                
                tr.innerHTML = `
                    <td><strong>#${(venta.idVenta || venta.IdVenta).toString().padStart(4, '0')}</strong></td>
                    <td>${venta.nombreCliente || 'Sin Cliente'}</td>
                    <td><span class="${estaEliminada ? 'status-cancelled' : 'status-approved'}">${estadoTexto}</span></td>
                    <td>
                        <div class="acciones-cell">
                            <button class="btn-action btn-info" onclick="verDetalleVenta(${venta.idVenta || venta.IdVenta})" style="background: #38bdf8; color: white;" title="Ver detalle"><i class="fa-solid fa-eye"></i></button>
                            ${!estaEliminada ? `
                                <button class="btn-action btn-edit" onclick="editarVenta(${venta.idVenta || venta.IdVenta})" style="background: #3b82f6; color: white;" title="Editar"><i class="fa-solid fa-pen"></i></button>
                                <button class="btn-action btn-delete" onclick="eliminarVenta(${venta.idVenta || venta.IdVenta})" style="background: #ef4444; color: white;" title="Eliminar"><i class="fa-solid fa-trash"></i></button>
                            ` : ''}
                        </div>
                    </td>`;
                tbody.appendChild(tr);
            });
        }
    } catch (error) { console.error("Error al cargar ventas:", error); }
}

async function verDetalleVenta(id) {
    try {
        const [ventaRes, historialRes, usuariosRes] = await Promise.all([
            fetch(`${API_BASE_URL}/Ventas/${id}`, { headers: getAuthHeaders() }),
            fetch(`${API_BASE_URL}/Ventas/${id}/historial`, { headers: getAuthHeaders() }),
            fetch(`${API_BASE_URL}/Usuarios`, { headers: getAuthHeaders() })
        ]);

        if (!ventaRes.ok) throw new Error('No se pudo cargar la venta');
        
        const venta = await ventaRes.json();
        const historial = historialRes.ok ? await historialRes.json() : [];
        const usuarios = usuariosRes.ok ? await usuariosRes.json() : [];

        const mapaUsuarios = {};
        usuarios.forEach(u => {
            const uid = u.idUsuario || u.IdUsuario;
            const personaNombre = u.personaNombre || u.PersonaNombre || '';
            const nombre = u.nombre || u.Nombre || '';
            const nombreUsuario = u.nombreUsuario || u.NombreUsuario || '';
            const apPat = u.apPat || u.ApPat || '';
            const apMat = u.apMat || u.ApMat || '';
            
            let nombreCompleto = '';
            if (personaNombre && personaNombre.trim() !== '') {
                if (personaNombre.trim().split(' ').length >= 3) {
                    nombreCompleto = personaNombre.trim();
                } else {
                    nombreCompleto = `${personaNombre} ${apPat} ${apMat}`.trim();
                }
            } else if (nombre && nombre.trim() !== '') {
                nombreCompleto = `${nombre} ${apPat} ${apMat}`.trim();
            } else {
                nombreCompleto = nombreUsuario || `Usuario ${uid}`;
            }
            nombreCompleto = nombreCompleto.replace(/\s+/g, ' ').trim();
            mapaUsuarios[uid] = nombreCompleto || `Usuario ${uid}`;
        });

        const detallesHTML = venta.detalles && venta.detalles.length > 0 ? 
            venta.detalles.map(d => `<tr>
                <td style="padding: 10px;">${d.nombreProducto || 'Producto'}</td>
                <td style="padding: 10px; text-align: center;">${d.cantidad}</td>
                <td style="padding: 10px; text-align: right;">Bs. ${d.precioUnitario.toFixed(2)}</td>
                <td style="padding: 10px; text-align: right;"><strong>Bs. ${d.subtotal.toFixed(2)}</strong></td>
            </tr>`).join('') : '<tr><td colspan="4" style="text-align: center; padding: 20px; color: #94a3b8;">Sin productos</td></tr>';

        let historialHTML = '';
        if (historial && historial.length > 0) {
            historialHTML = `<div style="margin-top: 40px; border-top: 3px solid rgba(56, 189, 248, 0.3); padding-top: 25px;">
                <h3 style="color: #38bdf8; margin-bottom: 20px; font-size: 18px;"><i class="fa-solid fa-clock-rotate-left"></i> Historial de Cambios (${historial.length})</h3>
                ${historial.map(h => {
                    let datosAntesObj = {}, datosDespuesObj = {};
                    try {
                        datosAntesObj = typeof h.datosAntes === 'string' ? JSON.parse(h.datosAntes) : h.datosAntes;
                        datosDespuesObj = typeof h.datosDespues === 'string' ? JSON.parse(h.datosDespues) : h.datosDespues;
                    } catch (e) {}
                    
                    const fechaFormateada = new Date(h.fechaModificacion).toLocaleString('es-BO');
                    const tipoBadge = h.tipoCambio === 'CREACION' ? 
                        '<span style="background: #22c55e; color: white; padding: 2px 8px; border-radius: 4px; font-size: 11px;">CREACIÓN</span>' :
                        '<span style="background: #f59e0b; color: white; padding: 2px 8px; border-radius: 4px; font-size: 11px;">ACTUALIZACIÓN</span>';
                    
                    const uid = h.idUsuarioModifico || h.IdUsuarioModifico;
                    const nombreUsuario = h.nombreUsuarioModifico || mapaUsuarios[uid] || `Usuario #${uid}`;

                    return `<div style="background: rgba(56, 189, 248, 0.05); padding: 20px; border-radius: 10px; margin-bottom: 20px; border-left: 4px solid #38bdf8;">
                        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; padding-bottom: 10px; border-bottom: 1px solid rgba(255,255,255,0.1);">
                            <div style="display: flex; align-items: center; gap: 10px;">
                                <div style="width: 36px; height: 36px; background: rgba(56, 189, 248, 0.2); border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 700; color: #38bdf8; font-size: 16px;">
                                    ${nombreUsuario.charAt(0).toUpperCase()}
                                </div>
                                <div>
                                    <div style="color: #f8fafc; font-weight: 600; font-size: 14px;">${nombreUsuario}</div>
                                    <span style="color: #94a3b8; font-size: 12px;">${fechaFormateada}</span>
                                </div>
                            </div>
                            ${tipoBadge}
                        </div>
                        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px;">
                            <div style="background: rgba(239, 68, 68, 0.1); padding: 15px; border-radius: 8px;">
                                <strong style="color: #ef4444; display: block; margin-bottom: 10px; font-size: 14px;"><i class="fa-solid fa-arrow-left"></i> ANTES</strong>
                                <div style="font-size: 12px; color: #f8fafc;">
                                    <div style="margin-bottom: 8px;"><strong>Total:</strong> Bs. ${(datosAntesObj.total || 0).toFixed(2)}</div>
                                    <div><strong>Productos:</strong> ${(datosAntesObj.detalles || []).length}</div>
                                    <details style="margin-top: 10px; cursor: pointer;"><summary style="color: #94a3b8; font-size: 11px;">Ver detalles</summary><pre style="color: #cbd5e1; font-size: 10px; margin-top: 5px; white-space: pre-wrap; word-break: break-all;">${JSON.stringify(datosAntesObj.detalles || [], null, 2)}</pre></details>
                                </div>
                            </div>
                            <div style="background: rgba(34, 197, 94, 0.1); padding: 15px; border-radius: 8px;">
                                <strong style="color: #22c55e; display: block; margin-bottom: 10px; font-size: 14px;"><i class="fa-solid fa-arrow-right"></i> DESPUÉS</strong>
                                <div style="font-size: 12px; color: #f8fafc;">
                                    <div style="margin-bottom: 8px;"><strong>Total:</strong> Bs. ${(datosDespuesObj.total || 0).toFixed(2)}</div>
                                    <div><strong>Productos:</strong> ${(datosDespuesObj.detalles || []).length}</div>
                                    <details style="margin-top: 10px; cursor: pointer;"><summary style="color: #94a3b8; font-size: 11px;">Ver detalles</summary><pre style="color: #cbd5e1; font-size: 10px; margin-top: 5px; white-space: pre-wrap; word-break: break-all;">${JSON.stringify(datosDespuesObj.detalles || [], null, 2)}</pre></details>
                                </div>
                            </div>
                        </div>
                        ${h.observaciones ? `<div style="margin-top: 15px; padding: 10px; background: rgba(255,255,255,0.05); border-radius: 6px; font-size: 12px; color: #94a3b8;"><strong>Observaciones:</strong> ${h.observaciones}</div>` : ''}
                    </div>`;
                }).join('')}</div>`;
        } else {
            historialHTML = `<div style="margin-top: 30px; padding: 30px; text-align: center; background: rgba(255,255,255,0.02); border-radius: 10px; border: 2px dashed rgba(255,255,255,0.1);">
                <i class="fa-solid fa-circle-info" style="font-size: 48px; color: #94a3b8; opacity: 0.5; margin-bottom: 15px;"></i>
                <p style="color: #94a3b8; font-size: 14px;">No hay historial de cambios registrado</p></div>`;
        }

        Swal.fire({
            title: `<strong>Detalle de Venta #${(venta.idVenta || venta.IdVenta).toString().padStart(4, '0')}</strong>`,
            html: `<div style="text-align: left; max-width: 900px; color: #f8fafc;">
                <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 20px; margin-bottom: 25px; background: rgba(56, 189, 248, 0.05); padding: 20px; border-radius: 10px;">
                    <div><label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Cliente</label><div style="color: #f8fafc; font-weight: 600; font-size: 15px;">${venta.nombreCliente || 'Sin Cliente'}</div></div>
                    <div><label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Fecha</label><div style="color: #f8fafc; font-size: 15px;">${new Date(venta.fechaVenta).toLocaleString('es-BO')}</div></div>
                    <div><label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Tipo de Pago</label><div style="color: #f8fafc; font-size: 15px;">${venta.tipoPago || 'N/A'}</div></div>
                    <div><label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Estado</label><span class="${venta.estado === 'Completada' ? 'status-approved' : 'status-cancelled'}" style="padding: 6px 12px; border-radius: 6px; font-weight: 600;">${venta.estado}</span></div>
                </div>
                <h4 style="color: #38bdf8; margin: 25px 0 15px 0; font-size: 16px;"><i class="fa-solid fa-cart-shopping"></i> Productos</h4>
                <div style="background: rgba(30, 41, 59, 0.5); border-radius: 10px; overflow: hidden;">
                    <table style="width: 100%; border-collapse: collapse;">
                        <thead style="background: rgba(56, 189, 248, 0.2);"><tr>
                            <th style="padding: 12px; text-align: left; color: #f8fafc; border-bottom: 2px solid rgba(56, 189, 248, 0.3);">Producto</th>
                            <th style="padding: 12px; text-align: center; color: #f8fafc; border-bottom: 2px solid rgba(56, 189, 248, 0.3);">Cant.</th>
                            <th style="padding: 12px; text-align: right; color: #f8fafc; border-bottom: 2px solid rgba(56, 189, 248, 0.3);">Precio</th>
                            <th style="padding: 12px; text-align: right; color: #f8fafc; border-bottom: 2px solid rgba(56, 189, 248, 0.3);">Subtotal</th>
                        </tr></thead>
                        <tbody>${detallesHTML}</tbody>
                        <tfoot style="background: rgba(34, 197, 94, 0.1);"><tr>
                            <td colspan="3" style="padding: 15px; text-align: right; color: #22c55e; font-weight: bold; font-size: 16px;">TOTAL:</td>
                            <td style="padding: 15px; text-align: right; color: #22c55e; font-weight: bold; font-size: 20px;">Bs. ${parseFloat(venta.total).toFixed(2)}</td>
                        </tr></tfoot>
                    </table>
                </div>${historialHTML}</div>`,
            width: '1000px', confirmButtonText: 'Cerrar', confirmButtonColor: '#64748b', showCloseButton: true
        });
    } catch (error) {
        console.error("[ERROR] Error al cargar detalle:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el detalle de la venta.', width: '500px' });
    }
}

async function abrirModalVenta() {
    try {
        const [clientesRes, productosRes] = await Promise.all([
            fetch(`${API_BASE_URL}/Clientes`, { headers: getAuthHeaders() }),
            fetch(`${API_BASE_URL}/Productos`, { headers: getAuthHeaders() })
        ]);
        const clientes = clientesRes.ok ? await clientesRes.json() : [];
        const productos = productosRes.ok ? await productosRes.json() : [];
        
        const token = localStorage.getItem('authToken');
        let idUsuarioActual = 1;
        if (token) {
            try {
                const payload = JSON.parse(atob(token.split('.')[1]));
                idUsuarioActual = parseInt(payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]) || 1;
            } catch (e) { console.warn("No se pudo extraer el ID del usuario del token"); }
        }
        
        Swal.fire({
            title: 'Nueva Venta',
            html: `<div id="venta-form" style="text-align: left; max-width: 600px;">
                <div style="margin-bottom: 15px;"><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Cliente *</label>
                    <select id="venta-cliente" class="swal2-input" style="width: 100%; margin: 0;"><option value="">Seleccione un cliente...</option>
                    ${clientes.map(c => `<option value="${c.idCliente || c.IdCliente || c.id}">${c.nombre || c.Nombre || 'Sin Nombre'}</option>`).join('')}</select></div>
                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 15px;">
                    <div><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Tipo de Pago *</label>
                        <select id="venta-tipo-pago" class="swal2-input" style="width: 100%; margin: 0;"><option value="Efectivo">Efectivo</option><option value="Tarjeta">Tarjeta</option><option value="Transferencia">Transferencia</option><option value="Credito">Crédito</option></select></div>
                    <div><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Fecha *</label>
                        <input type="datetime-local" id="venta-fecha" class="swal2-input" style="width: 100%; margin: 0;"></div>
                </div>
                <div style="margin-bottom: 15px;"><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Productos *</label>
                    <div id="venta-productos-container"></div>
                    <button type="button" id="btn-agregar-producto" style="margin-top: 10px; padding: 8px 16px; background: #38bdf8; color: white; border: none; border-radius: 6px; cursor: pointer;"><i class="fa-solid fa-plus"></i> Agregar Producto</button></div>
                <div style="margin-bottom: 15px;"><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Observaciones</label>
                    <textarea id="venta-observaciones" class="swal2-textarea" placeholder="Notas adicionales..." style="width: 100%; margin: 0;"></textarea></div>
                <div style="background: rgba(56, 189, 248, 0.1); padding: 15px; border-radius: 8px; margin-top: 15px;">
                    <strong style="color: #38bdf8; font-size: 18px;">Total: Bs. <span id="venta-total">0.00</span></strong></div></div>`,
            width: '700px', showCancelButton: true, confirmButtonText: 'Registrar Venta', cancelButtonText: 'Cancelar',
            confirmButtonColor: '#22c55e', cancelButtonColor: '#64748b',
            didOpen: () => {
                window._productosVenta = productos;
                agregarProductoVenta(productos);
                document.getElementById('btn-agregar-producto').addEventListener('click', () => agregarProductoVenta(productos));
                const fechaInput = document.getElementById('venta-fecha');
                if (fechaInput) {
                    const actualizarFecha = () => {
                        const ahora = new Date();
                        fechaInput.value = new Date(ahora.getTime() - ahora.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
                    };
                    actualizarFecha();
                    window._intervaloFechaVenta = setInterval(actualizarFecha, 1000);
                }
            },
            willClose: () => { if (window._intervaloFechaVenta) clearInterval(window._intervaloFechaVenta); },
            preConfirm: () => {
                const clienteId = document.getElementById('venta-cliente').value;
                const tipoPago = document.getElementById('venta-tipo-pago').value;
                const fecha = document.getElementById('venta-fecha').value;
                const observaciones = document.getElementById('venta-observaciones').value;
                if (!clienteId) { Swal.showValidationMessage('Debe seleccionar un cliente'); return false; }
                if (!fecha) { Swal.showValidationMessage('Debe seleccionar una fecha'); return false; }
                
                const productosSeleccionados = [];
                const productosSelects = document.querySelectorAll('.venta-producto-select');
                const cantidadInputs = document.querySelectorAll('.venta-producto-cantidad');
                for (let i = 0; i < productosSelects.length; i++) {
                    if (productosSelects[i].value) {
                        const option = productosSelects[i].options[productosSelects[i].selectedIndex];
                        productosSeleccionados.push({
                            idProducto: parseInt(productosSelects[i].value),
                            cantidad: parseInt(cantidadInputs[i].value) || 1,
                            precioUnitario: parseFloat(option.dataset.precio) || 0
                        });
                    }
                }
                if (productosSeleccionados.length === 0) { Swal.showValidationMessage('Debe agregar al menos un producto'); return false; }
                return { idCliente: parseInt(clienteId), idUsuario: idUsuarioActual, tipoPago, fechaVenta: new Date(fecha).toISOString(), observaciones, detalles: productosSeleccionados };
            }
        }).then((result) => { if (result.isConfirmed) registrarVenta(result.value); });
    } catch (error) {
        console.error("Error al abrir modal de venta:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos necesarios.' });
    }
}

function agregarProductoVenta(productos) {
    const container = document.getElementById('venta-productos-container');
    if (!container) return;
    const div = document.createElement('div');
    div.className = 'venta-producto-row';
    div.style.cssText = 'display: flex; gap: 10px; margin-bottom: 10px; align-items: center;';
    div.innerHTML = `
        <select class="venta-producto-select swal2-input" style="flex: 2; margin: 0;"><option value="">Seleccione producto...</option>
        ${productos.map(p => `<option value="${p.idProducto || p.IdProducto || p.id}" data-precio="${p.precioVenta || p.PrecioVenta || 0}">${p.nombre || p.Nombre} - Bs. ${parseFloat(p.precioVenta || p.PrecioVenta || 0).toFixed(2)}</option>`).join('')}</select>
        <input type="number" class="venta-producto-cantidad swal2-input" placeholder="Cant." min="1" value="1" style="flex: 1; min-width: 80px; margin: 0;">
        <button type="button" class="btn-eliminar-producto" style="background: #ef4444; color: white; border: none; border-radius: 6px; width: 42px; height: 42px; cursor: pointer;"><i class="fa-solid fa-trash"></i></button>`;
    container.appendChild(div);
    div.querySelector('.btn-eliminar-producto').addEventListener('click', function() { this.parentElement.remove(); calcularTotalVenta(); });
    div.querySelector('.venta-producto-select').addEventListener('change', calcularTotalVenta);
    div.querySelector('.venta-producto-cantidad').addEventListener('input', calcularTotalVenta);
}

function calcularTotalVenta() {
    let total = 0;
    const productosSelects = document.querySelectorAll('.venta-producto-select');
    const cantidadInputs = document.querySelectorAll('.venta-producto-cantidad');
    for (let i = 0; i < productosSelects.length; i++) {
        const select = productosSelects[i];
        const cantidad = parseInt(cantidadInputs[i].value) || 0;
        const option = select.options[select.selectedIndex];
        const precio = parseFloat(option.dataset.precio) || 0;
        if (select.value) { total += cantidad * precio; }
    }
    const totalSpan = document.getElementById('venta-total');
    if (totalSpan) { totalSpan.textContent = total.toFixed(2); }
}

async function registrarVenta(data) {
    try {
        const response = await fetch(`${API_BASE_URL}/Ventas`, {
            method: 'POST', headers: { ...getAuthHeaders(), 'Content-Type': 'application/json' },
            body: JSON.stringify({ idCliente: data.idCliente, idUsuario: data.idUsuario, tipoPago: data.tipoPago, fechaVenta: data.fechaVenta, detalles: data.detalles })
        });
        if (response.ok) {
            const resultado = await response.json();
            Swal.fire({ icon: 'success', title: '¡Venta Registrada!', text: resultado.mensaje || 'La venta se ha registrado correctamente.', timer: 2000, showConfirmButton: false });
            cargarVentasAPI();
        } else {
            const errorText = await response.text();
            let mensajeError = 'No se pudo registrar la venta.';
            try { const errorJson = JSON.parse(errorText); mensajeError = errorJson.mensaje || errorJson.message || errorText; } catch (e) { mensajeError = errorText; }
            Swal.fire({ icon: 'error', title: 'Error al Registrar', text: mensajeError, width: '500px' });
        }
    } catch (error) {
        console.error("Error al registrar venta:", error);
        Swal.fire({ icon: 'error', title: 'Error de Conexión', text: 'No se pudo conectar con el servidor.' });
    }
}

async function editarVenta(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/Ventas/${id}`, { headers: getAuthHeaders() });
        if (!response.ok) throw new Error('No se pudo cargar la venta');
        const venta = await response.json();
        
        const [clientesRes, productosRes] = await Promise.all([
            fetch(`${API_BASE_URL}/Clientes`, { headers: getAuthHeaders() }),
            fetch(`${API_BASE_URL}/Productos`, { headers: getAuthHeaders() })
        ]);
        const clientes = clientesRes.ok ? await clientesRes.json() : [];
        const productos = productosRes.ok ? await productosRes.json() : [];

        Swal.fire({
            title: 'Editar Venta',
            html: `<div id="venta-edit-form" style="text-align: left; max-width: 600px;">
                <div style="margin-bottom: 15px;"><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Cliente *</label>
                    <select id="edit-venta-cliente" class="swal2-input" style="width: 100%; margin: 0;">
                    ${clientes.map(c => `<option value="${c.idCliente || c.IdCliente}" ${(c.idCliente || c.IdCliente) === venta.idCliente ? 'selected' : ''}>${c.nombre || c.Nombre}</option>`).join('')}</select></div>
                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 15px;">
                    <div><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Tipo de Pago *</label>
                        <select id="edit-venta-tipo-pago" class="swal2-input" style="width: 100%; margin: 0;">
                        <option value="Efectivo" ${venta.tipoPago === 'Efectivo' ? 'selected' : ''}>Efectivo</option>
                        <option value="Tarjeta" ${venta.tipoPago === 'Tarjeta' ? 'selected' : ''}>Tarjeta</option>
                        <option value="Transferencia" ${venta.tipoPago === 'Transferencia' ? 'selected' : ''}>Transferencia</option>
                        <option value="Credito" ${venta.tipoPago === 'Credito' ? 'selected' : ''}>Crédito</option></select></div>
                    <div><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Fecha *</label>
                        <input type="datetime-local" id="edit-venta-fecha" class="swal2-input" style="width: 100%; margin: 0;"></div>
                </div>
                <div style="margin-bottom: 15px;"><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Productos *</label>
                    <div id="edit-venta-productos-container"></div>
                    <button type="button" id="edit-btn-agregar-producto" style="margin-top: 10px; padding: 8px 16px; background: #38bdf8; color: white; border: none; border-radius: 6px; cursor: pointer;"><i class="fa-solid fa-plus"></i> Agregar Producto</button></div>
                <div style="margin-bottom: 15px;"><label style="display: block; margin-bottom: 5px; color: #94a3b8; font-size: 13px;">Observaciones</label>
                    <textarea id="edit-venta-observaciones" class="swal2-textarea" placeholder="Notas adicionales..." style="width: 100%; margin: 0;">${venta.observaciones || ''}</textarea></div>
                <div style="background: rgba(56, 189, 248, 0.1); padding: 15px; border-radius: 8px; margin-top: 15px;">
                    <strong style="color: #38bdf8; font-size: 18px;">Total: Bs. <span id="edit-venta-total">0.00</span></strong></div></div>`,
            width: '700px', showCancelButton: true, confirmButtonText: 'Actualizar Venta', cancelButtonText: 'Cancelar',
            confirmButtonColor: '#3b82f6', cancelButtonColor: '#64748b',
            didOpen: () => {
                window._productosVentaEdit = productos;
                if (venta.detalles && venta.detalles.length > 0) {
                    venta.detalles.forEach(detalle => agregarProductoVentaEdit(productos, detalle));
                } else { agregarProductoVentaEdit(productos); }
                document.getElementById('edit-btn-agregar-producto').addEventListener('click', () => agregarProductoVentaEdit(productos));
                calcularTotalVentaEdit();
                const fechaInput = document.getElementById('edit-venta-fecha');
                if (fechaInput) {
                    const actualizarFecha = () => {
                        const ahora = new Date();
                        fechaInput.value = new Date(ahora.getTime() - ahora.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
                    };
                    actualizarFecha();
                    window._intervaloFechaEdit = setInterval(actualizarFecha, 1000);
                }
            },
            willClose: () => { if (window._intervaloFechaEdit) clearInterval(window._intervaloFechaEdit); },
            preConfirm: () => {
                const clienteId = document.getElementById('edit-venta-cliente').value;
                const tipoPago = document.getElementById('edit-venta-tipo-pago').value;
                const fecha = document.getElementById('edit-venta-fecha').value;
                const observaciones = document.getElementById('edit-venta-observaciones').value;
                if (!clienteId) { Swal.showValidationMessage('Debe seleccionar un cliente'); return false; }
                if (!fecha) { Swal.showValidationMessage('Debe seleccionar una fecha'); return false; }
                const productosSeleccionados = [];
                const productosSelects = document.querySelectorAll('.edit-venta-producto-select');
                const cantidadInputs = document.querySelectorAll('.edit-venta-producto-cantidad');
                for (let i = 0; i < productosSelects.length; i++) {
                    if (productosSelects[i].value) {
                        const option = productosSelects[i].options[productosSelects[i].selectedIndex];
                        productosSeleccionados.push({
                            idProducto: parseInt(productosSelects[i].value),
                            cantidad: parseInt(cantidadInputs[i].value) || 1,
                            precioUnitario: parseFloat(option.dataset.precio) || 0
                        });
                    }
                }
                if (productosSeleccionados.length === 0) { Swal.showValidationMessage('Debe agregar al menos un producto'); return false; }
                return { idCliente: parseInt(clienteId), idUsuario: venta.idUsuario || 1, tipoPago, fechaVenta: new Date(fecha).toISOString(), observaciones, detalles: productosSeleccionados };
            }
        }).then((result) => { if (result.isConfirmed) actualizarVenta(id, result.value); });
    } catch (error) {
        console.error("Error al cargar venta:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos de la venta.' });
    }
}

function agregarProductoVentaEdit(productos, detalleExistente = null) {
    const container = document.getElementById('edit-venta-productos-container');
    if (!container) return;
    const div = document.createElement('div');
    div.className = 'edit-venta-producto-row';
    div.style.cssText = 'display: flex; gap: 10px; margin-bottom: 10px; align-items: center;';
    const cantidadValor = detalleExistente ? detalleExistente.cantidad : 1;
    div.innerHTML = `
        <select class="edit-venta-producto-select swal2-input" style="flex: 2; margin: 0;"><option value="">Seleccione producto...</option>
        ${productos.map(p => `<option value="${p.idProducto || p.IdProducto}" data-precio="${p.precioVenta || p.PrecioVenta || 0}" ${(detalleExistente && detalleExistente.idProducto === (p.idProducto || p.IdProducto)) ? 'selected' : ''}>${p.nombre || p.Nombre} - Bs. ${parseFloat(p.precioVenta || p.PrecioVenta || 0).toFixed(2)}</option>`).join('')}</select>
        <input type="number" class="edit-venta-producto-cantidad swal2-input" placeholder="Cant." min="1" value="${cantidadValor}" style="flex: 1; min-width: 80px; margin: 0;">
        <button type="button" class="btn-eliminar-producto-edit" style="background: #ef4444; color: white; border: none; border-radius: 6px; width: 42px; height: 42px; cursor: pointer;"><i class="fa-solid fa-trash"></i></button>`;
    container.appendChild(div);
    div.querySelector('.btn-eliminar-producto-edit').addEventListener('click', function() { this.parentElement.remove(); calcularTotalVentaEdit(); });
    div.querySelector('.edit-venta-producto-select').addEventListener('change', calcularTotalVentaEdit);
    div.querySelector('.edit-venta-producto-cantidad').addEventListener('input', calcularTotalVentaEdit);
}

function calcularTotalVentaEdit() {
    let total = 0;
    const productosSelects = document.querySelectorAll('.edit-venta-producto-select');
    const cantidadInputs = document.querySelectorAll('.edit-venta-producto-cantidad');
    for (let i = 0; i < productosSelects.length; i++) {
        const select = productosSelects[i];
        const cantidad = parseInt(cantidadInputs[i].value) || 0;
        const option = select.options[select.selectedIndex];
        const precio = parseFloat(option.dataset.precio) || 0;
        if (select.value) { total += cantidad * precio; }
    }
    const totalSpan = document.getElementById('edit-venta-total');
    if (totalSpan) { totalSpan.textContent = total.toFixed(2); }
}

async function actualizarVenta(id, data) {
    try {
        const response = await fetch(`${API_BASE_URL}/Ventas/${id}`, {
            method: 'PUT', headers: { ...getAuthHeaders(), 'Content-Type': 'application/json' },
            body: JSON.stringify({ idCliente: data.idCliente, idUsuario: data.idUsuario, tipoPago: data.tipoPago, fechaVenta: data.fechaVenta, detalles: data.detalles })
        });
        if (response.ok || response.status === 204) {
            Swal.fire({ icon: 'success', title: 'Venta Actualizada', text: 'La venta se ha actualizado correctamente.', timer: 2000, showConfirmButton: false });
            cargarVentasAPI();
        } else {
            const error = await response.json();
            Swal.fire({ icon: 'error', title: 'Error', text: error.mensaje || 'No se pudo actualizar la venta.' });
        }
    } catch (error) {
        console.error("Error al actualizar venta:", error);
        Swal.fire({ icon: 'error', title: 'Error de Conexión', text: 'No se pudo conectar con el servidor.' });
    }
}

async function eliminarVenta(id) {
    const result = await Swal.fire({
        title: '¿Eliminar esta venta?', text: "La venta se marcará como eliminada pero permanecerá en el historial",
        icon: 'warning', showCancelButton: true, confirmButtonColor: '#ef4444', cancelButtonColor: '#64748b',
        confirmButtonText: 'Sí, eliminar', cancelButtonText: 'No, mantener'
    });
    if (result.isConfirmed) {
        try {
            const response = await fetch(`${API_BASE_URL}/Ventas/${id}`, { method: 'DELETE', headers: getAuthHeaders() });
            if (response.ok) {
                Swal.fire({ icon: 'success', title: 'Venta Eliminada', text: 'La venta ha sido marcada como eliminada.', timer: 2000, showConfirmButton: false });
                cargarVentasAPI();
            } else {
                const error = await response.json();
                Swal.fire({ icon: 'error', title: 'Error', text: error.mensaje || 'No se pudo eliminar la venta.' });
            }
        } catch (error) {
            console.error("Error al eliminar venta:", error);
            Swal.fire({ icon: 'error', title: 'Error de Conexión', text: 'No se pudo conectar con el servidor.' });
        }
    }
}

// === COMPRAS ===
async function cargarComprasAPI() {
    try {
        const tbody = document.getElementById("tabla-compras");
        if (!tbody) return;
        tbody.innerHTML = `<tr><td colspan="6" style="text-align: center; color: #94a3b8; padding: 40px;"><i class="fa-solid fa-circle-info" style="font-size: 48px; margin-bottom: 10px; display: block; opacity: 0.5;"></i>No hay compras registradas</td></tr>`;
    } catch (error) { console.error("Error al cargar compras:", error); }
}
function abrirModalCompra() { Swal.fire({ title: 'Nueva Compra', text: 'Funcionalidad en desarrollo', icon: 'info' }); }

// ========================================================================
// === DEVOLUCIONES (LÓGICA CORREGIDA: ADMIN SOLO VE, VENTAS CREA) ===
// ========================================================================

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
                
                // Solo el Admin (rol 1) puede ver los botones de aprobar/rechazar en pendientes
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
        }
    } catch (error) { console.error("Error al cargar devoluciones:", error); }
}

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
                                        data-cliente="${v.nombreCliente || ''}"
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
            width: '700px', showCancelButton: true, confirmButtonText: 'Registrar Devolución', cancelButtonText: 'Cancelar',
            confirmButtonColor: '#3b82f6', cancelButtonColor: '#64748b',
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
        }).then((result) => { if (result.isConfirmed) registrarDevolucion(result.value); });
    } catch (error) {
        console.error("Error al abrir modal:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos.' });
    }
}

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
            Swal.fire({ icon: 'success', title: 'Devolución Registrada', text: 'La devolución se ha registrado correctamente.', timer: 2000, showConfirmButton: false });
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
                        <div><label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Venta</label><div style="color: #f8fafc; font-weight: 600;">VNT-${(d.idVenta || d.IdVenta || 0).toString().padStart(4, '0')}</div></div>
                        <div><label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Fecha</label><div style="color: #f8fafc;">${d.fechaDevolucion ? new Date(d.fechaDevolucion).toLocaleString('es-BO') : '-'}</div></div>
                        <div><label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Cliente</label><div style="color: #f8fafc; font-weight: 600;">${d.nombreCliente || d.clienteNombre || '-'}</div></div>
                        <div><label style="display: block; color: #94a3b8; font-size: 12px; margin-bottom: 5px;">Estado</label><span style="background: ${estadoColor}20; color: ${estadoColor}; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: 600;">${estado}</span></div>
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
                    ${d.observaciones ? `<div style="background: rgba(255,255,255,0.02); border-radius: 10px; padding: 15px; margin-top: 15px;"><h4 style="color: #94a3b8; margin: 0 0 10px 0; font-size: 14px;"><i class="fa-solid fa-comment"></i> Observaciones</h4><div style="font-size: 13px; color: #f8fafc;">${d.observaciones}</div></div>` : ''}
                </div>
            `,
            width: '700px', confirmButtonText: 'Cerrar', confirmButtonColor: '#64748b', showCloseButton: true
        });
    } catch (error) {
        console.error("Error al cargar detalle:", error);
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el detalle.' });
    }
}

async function aprobarDevolucion(id) {
    const result = await Swal.fire({
        title: '¿Aprobar devolución?', text: "La devolución será marcada como aprobada y se procesará el reembolso.",
        icon: 'question', showCancelButton: true, confirmButtonColor: '#22c55e', cancelButtonColor: '#64748b',
        confirmButtonText: 'Sí, aprobar', cancelButtonText: 'Cancelar'
    });
    if (!result.isConfirmed) return;
    try {
        const response = await fetch(`${API_BASE_URL}/Devoluciones/${id}/aprobar`, { method: 'PUT', headers: getAuthHeaders() });
        if (response.ok) {
            Swal.fire({ icon: 'success', title: 'Devolución Aprobada', text: 'La devolución ha sido aprobada exitosamente.', timer: 2000, showConfirmButton: false });
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

async function rechazarDevolucion(id) {
    const result = await Swal.fire({
        title: '¿Rechazar devolución?', text: "La devolución será marcada como rechazada.",
        icon: 'warning', showCancelButton: true, confirmButtonColor: '#ef4444', cancelButtonColor: '#64748b',
        confirmButtonText: 'Sí, rechazar', cancelButtonText: 'Cancelar'
    });
    if (!result.isConfirmed) return;
    try {
        const response = await fetch(`${API_BASE_URL}/Devoluciones/${id}/rechazar`, { method: 'PUT', headers: getAuthHeaders() });
        if (response.ok) {
            Swal.fire({ icon: 'success', title: 'Devolución Rechazada', text: 'La devolución ha sido rechazada.', timer: 2000, showConfirmButton: false });
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

// === INICIALIZACIÓN ===
document.addEventListener('DOMContentLoaded', () => {
    verificarPermisosDevoluciones();
});