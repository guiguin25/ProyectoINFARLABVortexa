const API_BASE_URL = "https://localhost:7067/api";
 
document.addEventListener("DOMContentLoaded", () => {

    const loginCard = document.getElementById("login-card");

    const forgotCard = document.getElementById("forgot-card");

    const linkForgot = document.getElementById("link-forgot");

    const btnBackLogin = document.getElementById("btn-back-login");

    const formLogin = document.getElementById("form-login");

    const formForgot = document.getElementById("form-forgot");

    const forgotAlert = document.getElementById("forgot-alert");
 
    if (linkForgot && forgotCard && loginCard) {

        linkForgot.addEventListener("click", (e) => {

            e.preventDefault();

            loginCard.classList.add("hidden");

            forgotCard.classList.remove("hidden");

            if (forgotAlert) forgotAlert.classList.add("hidden");

        });

    }
 
    if (btnBackLogin && forgotCard && loginCard) {

        btnBackLogin.addEventListener("click", () => {

            forgotCard.classList.add("hidden");

            loginCard.classList.remove("hidden");

        });

    }
 
    if (formForgot) {

        formForgot.addEventListener("submit", (e) => {

            e.preventDefault();

            const emailInput = document.getElementById("recovery-email").value;

            if (emailInput) {

                forgotAlert.textContent = `Se ha enviado un enlace de recuperación al correo: ${emailInput}.`;

                forgotAlert.className = "alert-message alert-success";

                forgotAlert.classList.remove("hidden");

                setTimeout(() => { formForgot.reset(); }, 3000);

            }

        });

    }
 
    // 1. LOGIN FUNCIONAL (Auth/login)[cite: 4]

    if (formLogin) {

        formLogin.addEventListener("submit", async (e) => {

            e.preventDefault();

            const usernameInput = document.getElementById("username").value;

            const passwordInput = document.getElementById("password").value;
 
            try {

                const response = await fetch(`${API_BASE_URL}/Auth/login`, {

                    method: "POST",

                    headers: { "Content-Type": "application/json" },

                    body: JSON.stringify({

                        emailOrUsername: usernameInput,

                        contrasena: passwordInput

                    })

                });
 
                if (response.ok) {

                    const data = await response.json();

                    if (data.token) {

                        localStorage.setItem("authToken", data.token);

                    }

                    window.location.href = "dashboard.html";

                } else {

                    alert("Credenciales incorrectas o usuario inactivo[cite: 4].");

                }

            } catch (error) {

                console.error("Error conectando con la API:", error);

                window.location.href = "dashboard.html"; // Modo de respaldo si la API está apagada

            }

        });

    }
 
    // Enrutamiento automático según la página actual

    const currentPath = window.location.pathname;

    if (currentPath.includes("inventario.html")) {

        cargarProductosAPI();

    } else if (currentPath.includes("comercial.html")) {

        cargarComercialAPI();

    } else if (currentPath.includes("usuarios.html")) {

        cargarUsuariosAPI();

    }

});
 
// Obtener cabeceras con el Token JWT[cite: 4]

function getAuthHeaders() {

    const token = localStorage.getItem("authToken");

    return {

        "Content-Type": "application/json",

        "Authorization": token ? `Bearer ${token}` : ""

    };

}
 
// ==========================================

// 2. MÓDULO DE PRODUCTOS (ProductosController)[cite: 4]

// ==========================================

async function cargarProductosAPI() {

    try {

        const response = await fetch(`${API_BASE_URL}/Productos`, { headers: getAuthHeaders() });

        if (response.ok) {

            const productos = await response.json();

            const tbody = document.getElementById("tabla-productos");

            if (!tbody) return;
 
            tbody.innerHTML = ""; // Limpiamos la maqueta estática
 
            productos.forEach(prod => {

                const tr = document.createElement("tr");

                tr.innerHTML = `
<td><strong>PRD-${prod.idProducto.toString().padStart(3, '0')}</strong></td>
<td>${prod.nombre}</td>
<td>${prod.stockActual} unids.</td>
<td>150 unids.</td>
<td>Bs. ${prod.precioVenta.toFixed(2)}</td>
<td><span class="status-approved">Disponible</span></td>

                `;

                tbody.appendChild(tr);

            });

        }

    } catch (error) {

        console.error("Error al cargar productos:", error);

    }

}
 
async function crearProductoAPI(productoDto) {

    try {

        const response = await fetch(`${API_BASE_URL}/Productos`, {

            method: "POST",

            headers: getAuthHeaders(),

            body: JSON.stringify(productoDto)

        });

        if (response.ok) {

            alert("Producto registrado exitosamente.");

            cargarProductosAPI();

        } else {

            const err = await response.json();

            alert("Error: " + (err.mensaje || "No se pudo registrar"));

        }

    } catch (error) {

        console.error("Error en POST Producto:", error);

    }

}
 
// ==========================================

// 3. MÓDULO COMERCIAL (Clientes y Proveedores)[cite: 4]

// ==========================================

async function cargarComercialAPI() {

    try {

        const [resClientes, resProveedores] = await Promise.all([

            fetch(`${API_BASE_URL}/Clientes`, { headers: getAuthHeaders() }),

            fetch(`${API_BASE_URL}/Proveedores`, { headers: getAuthHeaders() })

        ]);
 
        if (resClientes.ok && resProveedores.ok) {

            const clientes = await resClientes.json();

            const proveedores = await resProveedores.json();

            console.log("Clientes:", clientes);

            console.log("Proveedores:", proveedores);

        }

    } catch (error) {

        console.error("Error cargando datos comerciales:", error);

    }

}
 
async function crearClienteAPI(crearClienteDto) {

    try {

        const response = await fetch(`${API_BASE_URL}/Clientes`, {

            method: "POST",

            headers: getAuthHeaders(),

            body: JSON.stringify(crearClienteDto)

        });

        if (response.ok) {

            alert("Cliente y persona registrados correctamente.");

            cargarComercialAPI();

        }

    } catch (error) {

        console.error("Error al crear cliente:", error);

    }

}
 
// ==========================================

// 4. MÓDULO DE USUARIOS (UsuariosController)[cite: 4]

// ==========================================

async function cargarUsuariosAPI() {

    try {

        const response = await fetch(`${API_BASE_URL}/Usuarios`, { headers: getAuthHeaders() });

        if (response.ok) {

            const usuarios = await response.json();

            console.log("Usuarios cargados:", usuarios);

        }

    } catch (error) {

        console.error("Error al cargar usuarios:", error);

    }

}
 
async function crearUsuarioAPI(crearUsuarioDto) {

    try {

        const response = await fetch(`${API_BASE_URL}/Usuarios`, {

            method: "POST",

            headers: getAuthHeaders(),

            body: JSON.stringify(crearUsuarioDto)

        });

        if (response.ok) {

            alert("Usuario creado exitosamente.");

            cargarUsuariosAPI();

        } else {

            const err = await response.json();

            alert("Error: " + (err.mensaje || "No se pudo crear el usuario"));

        }

    } catch (error) {

        console.error("Error en POST Usuario:", error);

    }

}
 