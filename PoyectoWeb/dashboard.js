if (!localStorage.getItem('authToken')) {
    window.location.href = 'index.html';
}

const permisosPorRol = {
    1: { nombre: "Administrador", paginasPermitidas: ["dashboard.html", "inventario.html", "comercial.html", "cuarentena.html", "usuarios.html"] },
    2: { nombre: "Logística", paginasPermitidas: ["dashboard.html", "inventario.html"] },
    3: { nombre: "Calidad", paginasPermitidas: ["dashboard.html", "cuarentena.html"] },
    4: { nombre: "Almacén MP", paginasPermitidas: ["dashboard.html", "inventario.html"] },
    5: { nombre: "Almacén PT", paginasPermitidas: ["dashboard.html", "inventario.html"] },
    6: { nombre: "Ventas", paginasPermitidas: ["dashboard.html", "ventas.html"] }
};

function obtenerRolId() {
    const rolId = localStorage.getItem('usuarioRolId');
    const rolNombre = String(localStorage.getItem('usuarioRolNombre') || "").toLowerCase();
    const rol = String(localStorage.getItem('usuarioRol') || "").toLowerCase();

    if (rolId && rolId !== "") return parseInt(rolId);
    if (rol === "6" || rol === "ventas" || rol === "ven") return 6;
    if (rolNombre === "6" || rolNombre === "ventas" || rolNombre === "ven") return 6;
    if (rol === "1" || rol === "administrador" || rol === "adm") return 1;
    
    return 1;
}

// PROTECCIÓN DE ACCESO: Si el rol no tiene permiso para esta página, redirigir
(function protegerAcceso() {
    const rolId = obtenerRolId();
    const paginaActual = window.location.pathname.split('/').pop() || 'dashboard.html';
    const permisos = permisosPorRol[rolId];
    
    if (permisos && !permisos.paginasPermitidas.includes(paginaActual)) {
        console.warn(`Acceso denegado: Rol ${permisos.nombre} no tiene permiso para ${paginaActual}. Redirigiendo a dashboard.html`);
        window.location.href = 'dashboard.html';
    }
})();

document.addEventListener('DOMContentLoaded', () => {
    const rolId = obtenerRolId();
    const nombreUsuario = localStorage.getItem('usuarioNombre') || 'Usuario';

    const userNameElement = document.getElementById('user-name');
    if (userNameElement) userNameElement.textContent = nombreUsuario;

    const userRoleElement = document.getElementById('user-role');
    if (userRoleElement) userRoleElement.textContent = permisosPorRol[rolId]?.nombre || "Rol Desconocido";

    const userAvatarElement = document.getElementById('user-avatar');
    if (userAvatarElement) userAvatarElement.textContent = nombreUsuario.trim().charAt(0).toUpperCase();

    // FILTRAR MENÚ LATERAL
    const menuItems = document.querySelectorAll('.sidebar-menu li[data-roles]');
    menuItems.forEach(item => {
        const rolesPermitidos = item.getAttribute('data-roles').split(',').map(r => parseInt(r.trim()));
        if (rolesPermitidos.includes(rolId)) {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });

    // Marcar menú activo
    const currentPath = window.location.pathname.split('/').pop() || 'dashboard.html';
    menuItems.forEach(item => {
        const link = item.querySelector('a');
        if (link && link.getAttribute('href') === currentPath) {
            item.classList.add('active');
        } else {
            item.classList.remove('active');
        }
    });

    cargarEstadisticasDashboard(rolId);
});

async function cargarEstadisticasDashboard(rolId) {
    try {
        const headers = { "Content-Type": "application/json", "Authorization": `Bearer ${localStorage.getItem('authToken')}` };
        
        if (rolId === 1) {
            const [prodRes, cliRes, venRes] = await Promise.all([
                fetch(`${API_BASE_URL}/Productos`, { headers }),
                fetch(`${API_BASE_URL}/Clientes`, { headers }),
                fetch(`${API_BASE_URL}/Ventas`, { headers })
            ]);
            if (prodRes.ok) {
                const prods = await prodRes.json();
                document.getElementById('stat-lotes-criticos').textContent = prods.filter(p => p.stockActual < 50).length;
            }
            if (cliRes.ok) {
                const clis = await cliRes.json();
                document.getElementById('stat-clientes-activos').textContent = clis.length;
            }
            if (venRes.ok) {
                const vens = await venRes.json();
                document.getElementById('stat-ventas-mes').textContent = vens.length;
            }
        } else if (rolId === 6) {
            const [cliRes, venRes] = await Promise.all([
                fetch(`${API_BASE_URL}/Clientes`, { headers }),
                fetch(`${API_BASE_URL}/Ventas`, { headers })
            ]);
            if (cliRes.ok) {
                const clis = await cliRes.json();
                document.getElementById('stat-clientes-activos').textContent = clis.length;
            }
            if (venRes.ok) {
                const vens = await venRes.json();
                document.getElementById('stat-ventas-mes').textContent = vens.length;
            }
            const statCards = document.querySelectorAll('.stat-card');
            if (statCards[0]) statCards[0].style.display = 'none';
        } else {
            document.getElementById('stat-lotes-criticos').textContent = '-';
            document.getElementById('stat-clientes-activos').textContent = '-';
            document.getElementById('stat-ventas-mes').textContent = '-';
        }
        
        const syncStatus = document.getElementById('sync-status');
        if (syncStatus) syncStatus.textContent = 'Sincronizado';
    } catch (error) {
        console.error("Error cargando estadísticas:", error);
    }
}

function cerrarSesion() {
    localStorage.clear();
    window.location.href = 'index.html';
}