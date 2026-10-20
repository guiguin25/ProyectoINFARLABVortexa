const API_BASE_URL = "https://localhost:7067/api";

document.addEventListener("DOMContentLoaded", () => {
    // === ELEMENTOS DEL DOM ===
    const loginCard = document.getElementById("login-card");
    const forgotCard = document.getElementById("forgot-card");
    const forgotCardStep2 = document.getElementById("forgot-card-step2");
    const forgotCardStep3 = document.getElementById("forgot-card-step3");
    
    const linkForgot = document.getElementById("link-forgot");
    const btnBackLogin = document.getElementById("btn-back-login");
    const formLogin = document.getElementById("form-login");
    
    const formStep1 = document.getElementById("form-forgot-step1");
    const formStep2 = document.getElementById("form-forgot-step2");
    const formStep3 = document.getElementById("form-forgot-step3");
    const btnBackStep1 = document.getElementById("btn-back-step1");
    const btnBackStep2 = document.getElementById("btn-back-step2");
    
    const rememberMeCheckbox = document.getElementById("remember-me");
    const usernameInput = document.getElementById("username");
    
    const registerCard = document.getElementById("register-card");
    const linkRegister = document.getElementById("link-register");
    const btnBackLoginFromRegister = document.getElementById("btn-back-login-from-register");
    const formRegister = document.getElementById("form-register");

    let recoveryEmail = "";
    let recoveryToken = "";

    //CARGAR USUARIO RECORDADO
    if (rememberMeCheckbox && usernameInput) {
        const rememberedUser = localStorage.getItem("infarlab_remember_user");
        if (rememberedUser) {
            usernameInput.value = rememberedUser;
            rememberMeCheckbox.checked = true;
        }
    }

    //NAVEGACIÓN ENTRE VISTAS
    if (linkForgot && forgotCard && loginCard) {
        linkForgot.addEventListener("click", (e) => {
            e.preventDefault();
            loginCard.classList.add("hidden");
            forgotCard.classList.remove("hidden");
            if (forgotCardStep2) forgotCardStep2.classList.add("hidden");
            if (forgotCardStep3) forgotCardStep3.classList.add("hidden");
        });
    }

    if (btnBackLogin && loginCard) {
        btnBackLogin.addEventListener("click", () => {
            if (forgotCard) forgotCard.classList.add("hidden");
            if (forgotCardStep2) forgotCardStep2.classList.add("hidden");
            if (forgotCardStep3) forgotCardStep3.classList.add("hidden");
            loginCard.classList.remove("hidden");
        });
    }

    if (linkRegister && registerCard && loginCard) {
        linkRegister.addEventListener("click", (e) => {
            e.preventDefault();
            loginCard.classList.add("hidden");
            registerCard.classList.remove("hidden");
        });
    }

    if (btnBackLoginFromRegister && loginCard) {
        btnBackLoginFromRegister.addEventListener("click", () => {
            registerCard.classList.add("hidden");
            loginCard.classList.remove("hidden");
            if (formRegister) formRegister.reset();
        });
    }

    if (btnBackStep1 && forgotCard && forgotCardStep2) {
        btnBackStep1.addEventListener("click", () => {
            forgotCardStep2.classList.add("hidden");
            forgotCard.classList.remove("hidden");
        });
    }

    if (btnBackStep2 && forgotCardStep2 && forgotCardStep3) {
        btnBackStep2.addEventListener("click", () => {
            forgotCardStep3.classList.add("hidden");
            forgotCardStep2.classList.remove("hidden");
        });
    }

    const recoveryEmailInput = document.getElementById("recovery-email");
    const emailValidationMsg = document.getElementById("email-validation-msg");

    if (recoveryEmailInput) {
        recoveryEmailInput.addEventListener("input", (e) => {
            const email = e.target.value.trim();
            const infarlabPattern = /^[a-zA-Z0-9._%+-]+@infarlab\.com$/i;
            
            if (email === "") {
                recoveryEmailInput.classList.remove("valid", "invalid");
                if (emailValidationMsg) emailValidationMsg.classList.remove("show", "error", "success");
            } else if (infarlabPattern.test(email)) {
                recoveryEmailInput.classList.remove("invalid");
                recoveryEmailInput.classList.add("valid");
                if (emailValidationMsg) {
                    emailValidationMsg.classList.remove("error");
                    emailValidationMsg.classList.add("show", "success");
                    emailValidationMsg.textContent = "Formato correcto @infarlab.com";
                }
            } else {
                recoveryEmailInput.classList.remove("valid");
                recoveryEmailInput.classList.add("invalid");
                if (emailValidationMsg) {
                    emailValidationMsg.classList.remove("success");
                    emailValidationMsg.classList.add("show", "error");
                    emailValidationMsg.textContent = "SOLO SE ACEPTA FORMATO @infarlab.com";
                }
            }
        });
        
        recoveryEmailInput.addEventListener("blur", () => {
            const email = recoveryEmailInput.value.trim();
            const infarlabPattern = /^[a-zA-Z0-9._%+-]+@infarlab\.com$/i;
            if (email && !infarlabPattern.test(email)) {
                recoveryEmailInput.classList.add("invalid");
                if (emailValidationMsg) {
                    emailValidationMsg.classList.add("show", "error");
                    emailValidationMsg.textContent = "SOLO SE ACEPTA FORMATO @infarlab.com";
                }
            }
        });
    }

    if (formStep1) {
        formStep1.addEventListener("submit", (e) => {
            e.preventDefault();
            recoveryEmail = document.getElementById("recovery-email").value.trim();
            const infarlabPattern = /^[a-zA-Z0-9._%+-]+@infarlab\.com$/i;
            if (!recoveryEmail || !recoveryEmail.includes('@')) {
                Swal.fire({ icon: 'warning', title: 'Email Requerido', text: 'Ingresa un correo electrónico.' });
                return;
            }
            if (!infarlabPattern.test(recoveryEmail)) {
                Swal.fire({ icon: 'error', title: 'Email Inválido', text: 'Solo se aceptan correos con dominio @infarlab.com' });
                return;
            }
            forgotCard.classList.add("hidden");
            forgotCardStep2.classList.remove("hidden");
        });
    }

    if (formStep2) {
        formStep2.addEventListener("submit", async (e) => {
            e.preventDefault();
            const totpCode = document.getElementById("recovery-totp").value.trim();
            if (!totpCode || totpCode.length !== 6) {
                Swal.fire({ icon: 'warning', title: 'Código Inválido', text: 'Ingresa un código de 6 dígitos.' });
                return;
            }
            Swal.fire({ title: 'Validando...', allowOutsideClick: false, didOpen: () => Swal.showLoading() });
            try {
                const response = await fetch(`${API_BASE_URL}/Auth/validar-totp-recuperacion`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ email: recoveryEmail, codigoTotp: totpCode })
                });
                const text = await response.text();
                const data = text ? JSON.parse(text) : {};
                if (response.ok && data.token) {
                    recoveryToken = data.token;
                    Swal.close();
                    forgotCardStep2.classList.add("hidden");
                    forgotCardStep3.classList.remove("hidden");
                } else {
                    Swal.fire({ icon: 'error', title: 'Error', text: data.mensaje || "Código inválido o usuario sin 2FA." });
                }
            } catch (error) {
                Swal.fire({ icon: 'error', title: 'Error de Conexión', text: 'No se pudo conectar con el servidor.' });
            }
        });
    }

    if (formStep3) {
        formStep3.addEventListener("submit", async (e) => {
            e.preventDefault();
            const newPass = document.getElementById("new-password").value;
            const confirmPass = document.getElementById("confirm-password").value;
            if (newPass !== confirmPass) {
                Swal.fire({ icon: 'error', title: 'Error', text: 'Las contraseñas no coinciden.' });
                return;
            }
            Swal.fire({ title: 'Restableciendo...', allowOutsideClick: false, didOpen: () => Swal.showLoading() });
            try {
                const response = await fetch(`${API_BASE_URL}/Auth/restablecer-contrasena`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ email: recoveryEmail, token: recoveryToken, nuevaContrasena: newPass, confirmarContrasena: confirmPass })
                });
                const text = await response.text();
                const data = text ? JSON.parse(text) : {};
                if (response.ok) {
                    Swal.fire({ icon: 'success', title: '¡Contraseña Actualizada!', text: data.mensaje || "Tu contraseña ha sido restablecida.", timer: 3000, showConfirmButton: false }).then(() => {
                        if (formStep1) formStep1.reset();
                        if (formStep2) formStep2.reset();
                        if (formStep3) formStep3.reset();
                        recoveryEmail = "";
                        recoveryToken = "";
                        if (forgotCard) forgotCard.classList.add("hidden");
                        if (forgotCardStep2) forgotCardStep2.classList.add("hidden");
                        if (forgotCardStep3) forgotCardStep3.classList.add("hidden");
                        if (loginCard) loginCard.classList.remove("hidden");
                    });
                } else {
                    Swal.fire({ icon: 'error', title: 'Error', text: data.mensaje || "No se pudo restablecer la contraseña." });
                }
            } catch (error) {
                Swal.fire({ icon: 'error', title: 'Error de Conexión', text: 'No se pudo conectar con el servidor.' });
            }
        });
    }

    //LOGIN
    if (formLogin) {
        formLogin.addEventListener("submit", async (e) => {
            e.preventDefault();
            if (!usernameInput) return;
            
            try {
                const response = await fetch(`${API_BASE_URL}/Auth/login`, { 
                    method: "POST", 
                    headers: { "Content-Type": "application/json" }, 
                    body: JSON.stringify({ emailOrUsername: usernameInput.value, contrasena: document.getElementById("password").value }) 
                });

                if (response.ok) {
                    const data = await response.json();
                    if (data.token) localStorage.setItem("authToken", data.token);
                    localStorage.setItem("usuarioNombre", data.nombreUsuario || usernameInput.value);
                    localStorage.setItem("usuarioEmail", data.email || "");
                    
                    const rolId = data.idRol || data.rol || data.IdRol || data.rolNombre || data.nombreRol || "";
                    const rolStr = String(rolId).toLowerCase().trim();
                    let rolFinal = 1;
                    let rolNombre = "Administrador";

                    if (rolStr === "6" || rolStr === "ventas" || rolStr === "ven") {
                        rolFinal = 6; rolNombre = "Ventas";
                    } else if (rolStr === "1" || rolStr === "administrador" || rolStr === "adm") {
                        rolFinal = 1; rolNombre = "Administrador";
                    } else if (rolStr === "2" || rolStr.includes("log")) {
                        rolFinal = 2; rolNombre = "Logística";
                    } else if (rolStr === "3" || rolStr.includes("cal")) {
                        rolFinal = 3; rolNombre = "Calidad";
                    } else if (rolStr === "4" || rolStr.includes("materia") || rolStr === "amp") {
                        rolFinal = 4; rolNombre = "Almacén MP";
                    } else if (rolStr === "5" || rolStr.includes("producto") || rolStr === "apt") {
                        rolFinal = 5; rolNombre = "Almacén PT";
                    }

                    localStorage.setItem("usuarioRolId", rolFinal);
                    localStorage.setItem("usuarioRolNombre", rolNombre);
                    localStorage.setItem("usuarioRol", rolFinal);

                    if (rememberMeCheckbox && rememberMeCheckbox.checked) {
                        localStorage.setItem("infarlab_remember_user", usernameInput.value);
                    } else {
                        localStorage.removeItem("infarlab_remember_user");
                    }

                    Swal.fire({ 
                        icon: 'success', 
                        title: '¡Bienvenido!', 
                        text: `Iniciando sesión como ${data.nombreUsuario || usernameInput.value}`,
                        timer: 1500, 
                        showConfirmButton: false 
                    }).then(() => { 
                        window.location.href = "dashboard.html";
                    });
                } else {
                    const err = await response.json();
                    Swal.fire({ icon: 'error', title: 'Acceso Denegado', text: err.mensaje || 'Credenciales incorrectas.' });
                }
            } catch (error) {
                Swal.fire({ icon: 'error', title: 'Error de Conexión', text: 'Verifica que la API esté corriendo.' });
            }
        });
    }

    // === REGISTRO CON CONTRASEÑA AUTOMÁTICA ===
    if (formRegister) {
        const regNombre = document.getElementById("reg-nombre");
        const regApPat = document.getElementById("reg-apPat");
        const regApMat = document.getElementById("reg-apMat");
        const regCI = document.getElementById("reg-ci");
        const regEmail = document.getElementById("reg-email");
        const regRol = document.getElementById("reg-rol");
        const regUsername = document.getElementById("reg-username");
        const regPassword = document.getElementById("reg-password");

        const rolesIniciales = { "1": "ADM", "2": "LOG", "3": "CAL", "4": "AMP", "5": "APT", "6": "VEN" };
        let emailManuallyEdited = false;
        let usernameGenerated = false;
        let passwordGenerated = false;

        function soloLetras(e) { e.target.value = e.target.value.replace(/[0-9]/g, ""); }
        if (regNombre) regNombre.addEventListener("input", (e) => { soloLetras(e); actualizarAutocompletados(); });
        if (regApPat) regApPat.addEventListener("input", (e) => { soloLetras(e); actualizarAutocompletados(); });
        if (regApMat) regApMat.addEventListener("input", (e) => { soloLetras(e); actualizarAutocompletados(); });
        if (regCI) regCI.addEventListener("input", (e) => { e.target.value = e.target.value.replace(/[^0-9]/g, "").slice(0, 10); });

        function actualizarAutocompletados() {
            if (!regNombre || !regApPat || !regRol || !regEmail || !regUsername) return;
            const nombre = regNombre.value.trim();
            const apPat = regApPat.value.trim();
            const apMat = regApMat ? regApMat.value.trim() : "";
            const rolId = regRol.value;

            if (nombre && apPat) {
                const nombreNorm = nombre.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/[^a-z0-9]/g, "");
                const apPatNorm = apPat.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/[^a-z0-9]/g, "");
                const emailGenerado = `${nombreNorm}.${apPatNorm}@infarlab.com`;
                if (!emailManuallyEdited || regEmail.value.endsWith("@infarlab.com")) regEmail.value = emailGenerado;
            }
            if (nombre && apPat && rolId && !usernameGenerated) {
                const nombreNorm = nombre.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/[^a-z]/g, "");
                const apPatNorm = apPat.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/[^a-z]/g, "");
                const apMatNorm = apMat ? apMat.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/[^a-z]/g, "") : "";
                const numerosAleatorios = Math.floor(1000 + Math.random() * 9000);
                regUsername.value = apMatNorm ? `${rolesIniciales[rolId]}_${nombreNorm}_${apPatNorm}_${apMatNorm}_${numerosAleatorios}` : `${rolesIniciales[rolId]}_${nombreNorm}_${apPatNorm}_${numerosAleatorios}`;
                usernameGenerated = true;
                
                // Generar contraseña automáticamente cuando se genera el username
                if (!passwordGenerated && regPassword) {
                    const passwordTemporal = generarPasswordTemporal(12);
                    regPassword.value = passwordTemporal;
                    passwordGenerated = true;
                }
            }
        }

        if (regRol) regRol.addEventListener("change", () => { 
            usernameGenerated = false; 
            passwordGenerated = false;
            if (regPassword) regPassword.value = "";
            actualizarAutocompletados(); 
        });
        if (regEmail) regEmail.addEventListener("input", () => { if (!regEmail.value.endsWith("@infarlab.com")) emailManuallyEdited = true; });

        formRegister.addEventListener("submit", async (e) => {
            e.preventDefault();
            
            const passwordTemporal = regPassword ? regPassword.value : generarPasswordTemporal(12);
            
            if (!passwordTemporal || passwordTemporal.length < 6) {
                Swal.fire({ icon: 'warning', title: 'Contraseña no generada', text: 'Complete todos los campos para generar la contraseña.' });
                return;
            }
            
            Swal.fire({ title: 'Registrando...', allowOutsideClick: false, didOpen: () => Swal.showLoading() });
            
            try {
                const response = await fetch(`${API_BASE_URL}/Auth/register`, { 
                    method: "POST", 
                    headers: { "Content-Type": "application/json" }, 
                    body: JSON.stringify({ 
                        nombre: regNombre.value.trim(), 
                        apPat: regApPat.value.trim(), 
                        apMat: regApMat.value.trim(), 
                        ci: regCI.value.trim(), 
                        email: regEmail.value.trim(), 
                        idRol: parseInt(regRol.value), 
                        username: regUsername.value.trim(), 
                        contrasena: passwordTemporal
                    }) 
                });
                const data = await response.json();
                if (response.ok) {
                    Swal.close();
                    
                    const userId = data.idUsuario || data.IdUsuario || data.id;
                    const userName = regNombre.value.trim();
                    
                    mostrarModalCambioPassword(userId, passwordTemporal, userName);
                    
                    usernameGenerated = false;
                    passwordGenerated = false;
                } else {
                    Swal.fire({ icon: 'error', title: 'Error', text: data.mensaje || "No se pudo crear la cuenta." });
                }
            } catch (error) {
                Swal.fire({ icon: 'error', title: 'Error de Conexión', text: 'No se pudo conectar con el servidor.' });
            }
        });
    }

    const currentPath = window.location.pathname;
    if (currentPath.includes("inventario.html") && typeof cargarProductosAPI === 'function') cargarProductosAPI();
    else if (currentPath.includes("comercial.html") && typeof cargarComercialAPI === 'function') cargarComercialAPI();
    else if (currentPath.includes("usuarios.html") && typeof cargarUsuariosAPI === 'function') cargarUsuariosAPI();
    else if (currentPath.includes("ventas.html") && typeof cargarVentasAPI === 'function') cargarVentasAPI();
});

// === FUNCIONES GLOBALES ===

function generarPasswordTemporal(longitud = 12) {
    const mayusculas = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    const minusculas = 'abcdefghijklmnopqrstuvwxyz';
    const numeros = '0123456789';
    const especiales = '!@#$%^&*';
    const todos = mayusculas + minusculas + numeros + especiales;
    
    let password = '';
    password += mayusculas[Math.floor(Math.random() * mayusculas.length)];
    password += minusculas[Math.floor(Math.random() * minusculas.length)];
    password += numeros[Math.floor(Math.random() * numeros.length)];
    password += especiales[Math.floor(Math.random() * especiales.length)];
    
    for (let i = password.length; i < longitud; i++) {
        password += todos[Math.floor(Math.random() * todos.length)];
    }
    
    return password.split('').sort(() => Math.random() - 0.5).join('');
}

function togglePassword(inputId, button) {
    const input = document.getElementById(inputId);
    const icon = button.querySelector('i');
    
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}

function mostrarModalCambioPassword(userId, passwordTemporal, userName) {
    const modal = document.getElementById("modal-cambiar-password");
    const passwordSpan = document.getElementById("password-temporal");
    const userIdInput = document.getElementById("cambiar-password-userid");
    
    if (modal && passwordSpan && userIdInput) {
        passwordSpan.textContent = passwordTemporal;
        userIdInput.value = userId;
        modal.classList.remove("hidden");
        
        Swal.fire({
            icon: 'success',
            title: '¡Cuenta Creada!',
            html: `<p style="color: var(--text-main); margin-bottom: 10px;">
                    Bienvenido <strong>${userName}</strong>
                   </p>
                   <p style="color: var(--text-muted); font-size: 14px;">
                    Tu cuenta ha sido creada. Por seguridad, debes establecer una contraseña personalizada.
                   </p>`,
            confirmButtonText: 'Continuar',
            confirmButtonColor: '#3b82f6',
            allowOutsideClick: false
        }).then(() => {
            document.getElementById("nueva-password").focus();
        });
    }
}

function cerrarModalPassword() {
    const modal = document.getElementById("modal-cambiar-password");
    if (modal) {
        modal.classList.add("hidden");
        document.getElementById("form-cambiar-password").reset();
    }
}

document.getElementById("form-cambiar-password")?.addEventListener("submit", async (e) => {
    e.preventDefault();
    
    const nuevaPassword = document.getElementById("nueva-password").value;
    const confirmarPassword = document.getElementById("confirmar-password").value;
    const userId = document.getElementById("cambiar-password-userid").value;
    const passwordTemporal = document.getElementById("password-temporal").textContent;
    
    if (nuevaPassword !== confirmarPassword) {
        Swal.fire({ icon: 'error', title: 'Error', text: 'Las contraseñas no coinciden.' });
        return;
    }
    
    if (nuevaPassword.length < 6) {
        Swal.fire({ icon: 'warning', title: 'Contraseña muy corta', text: 'La contraseña debe tener al menos 6 caracteres.' });
        return;
    }
    
    Swal.fire({ title: 'Guardando...', allowOutsideClick: false, didOpen: () => Swal.showLoading() });
    
    try {
        const response = await fetch(`${API_BASE_URL}/Auth/cambiar-password`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
        UserId: parseInt(userId),
        ContrasenaActual: passwordTemporal,
        ContrasenaNueva: nuevaPassword
    })
});
        
        if (response.ok) {
            Swal.fire({
                icon: 'success',
                title: '¡Contraseña Actualizada!',
                text: 'Tu contraseña ha sido cambiada exitosamente. Ahora puedes iniciar sesión.',
                timer: 3000,
                showConfirmButton: false
            }).then(() => {
                cerrarModalPassword();
                window.location.href = "index.html";
            });
        } else {
            const error = await response.json();
            Swal.fire({ icon: 'error', title: 'Error', text: error.mensaje || "No se pudo cambiar la contraseña." });
        }
    } catch (error) {
        Swal.fire({ icon: 'error', title: 'Error de Conexión', text: 'No se pudo conectar con el servidor.' });
    }
});

function getAuthHeaders() {
    const token = localStorage.getItem("authToken");
    return { "Content-Type": "application/json", "Authorization": token ? `Bearer ${token}` : "" };
}

// --- MÓDULO DE PRODUCTOS ---
async function cargarProductosAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/Productos`, { headers: getAuthHeaders() });
        if (response.ok) {
            const productos = await response.json();
            const tbody = document.getElementById("tabla-productos");
            if (!tbody) return;
            tbody.innerHTML = "";
            productos.forEach(prod => {
                const tr = document.createElement("tr");
                tr.innerHTML = `<td><strong>PRD-${prod.idProducto.toString().padStart(3, '0')}</strong></td><td>${prod.nombre}</td><td>${prod.stockActual} unids.</td><td>150 unids.</td><td>Bs. ${prod.precioVenta.toFixed(2)}</td><td><span class="status-approved">Disponible</span></td>`;
                tbody.appendChild(tr);
            });
        }
    } catch (error) { console.error("Error al cargar productos:", error); }
}

async function crearProductoAPI(productoDto) {
    try {
        const response = await fetch(`${API_BASE_URL}/Productos`, { method: "POST", headers: getAuthHeaders(), body: JSON.stringify(productoDto) });
        if (response.ok) { 
            Swal.fire({ icon: 'success', title: 'Producto Registrado', text: 'El producto se ha guardado correctamente.', timer: 2000, showConfirmButton: false });
            cargarProductosAPI(); 
        } else { 
            const err = await response.json(); 
            Swal.fire({ icon: 'error', title: 'Error', text: err.mensaje || "No se pudo registrar el producto." }); 
        }
    } catch (error) { console.error("Error en POST Producto:", error); }
}

// --- MÓDULO COMERCIAL ---
async function cargarComercialAPI() {
    try {
        const [resClientes, resProveedores] = await Promise.all([ 
            fetch(`${API_BASE_URL}/Clientes`, { headers: getAuthHeaders() }), 
            fetch(`${API_BASE_URL}/Proveedores`, { headers: getAuthHeaders() }) 
        ]);
        if (resClientes.ok && resProveedores.ok) {
            console.log("Clientes:", await resClientes.json());
            console.log("Proveedores:", await resProveedores.json());
        }
    } catch (error) { console.error("Error cargando datos comerciales:", error); }
}

async function crearClienteAPI(crearClienteDto) {
    try {
        const response = await fetch(`${API_BASE_URL}/Clientes`, { method: "POST", headers: getAuthHeaders(), body: JSON.stringify(crearClienteDto) });
        if (response.ok) { 
            Swal.fire({ icon: 'success', title: 'Cliente Registrado', text: 'Los datos del cliente se han guardado correctamente.', timer: 2000, showConfirmButton: false });
            cargarComercialAPI(); 
        } else {
            const err = await response.json();
            Swal.fire({ icon: 'error', title: 'Error', text: err.mensaje || "No se pudo registrar el cliente." });
        }
    } catch (error) { console.error("Error al crear cliente:", error); }
}

// --- MÓDULO DE USUARIOS ---
async function cargarUsuariosAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/Usuarios`, { headers: getAuthHeaders() });
        if (response.ok) {
            const usuarios = await response.json();
            const tbody = document.getElementById("tabla-usuarios");
            if (!tbody) return;
            tbody.innerHTML = "";
            if (usuarios.length === 0) {
                tbody.innerHTML = "<tr><td colspan='8' style='text-align:center; padding: 20px;'>No hay usuarios registrados</td></tr>";
                return;
            }
            usuarios.forEach(u => {
                const tr = document.createElement("tr");
                const usuarioStr = encodeURIComponent(JSON.stringify(u));
                const usuarioActivo = u.estado === true || u.Estado === true;
                const idUsuario = u.idUsuario || u.IdUsuario;
                tr.innerHTML = `
                    <td>${idUsuario}</td>
                    <td>${u.personaNombre || u.PersonaNombre || 'Sin Persona'}</td>
                    <td>${u.nombreUsuario || u.NombreUsuario}</td>
                    <td><span style="color: #f8fafc; font-size: 13px;">${u.email || u.Email || '-'}</span></td>
                    <td><span class="badge" style="background: #3b82f6; padding: 4px 8px; border-radius: 4px; font-size: 12px; color: white;">${u.rolNombre || u.RolNombre || 'Sin Rol'}</span></td>
                    <td><span style="color: ${usuarioActivo ? '#22c55e' : '#ef4444'}; font-weight: bold;">${usuarioActivo ? 'Activo' : 'Inactivo'}</span></td>
                    <td class="col-acciones">
                        <div class="acciones-container">
                            <button class="btn-action btn-edit" onclick="abrirModalEditar('${usuarioStr}')"><i class="fa-solid fa-pen"></i></button>
                            <button class="btn-action btn-delete" onclick="eliminarUsuario(${idUsuario})"><i class="fa-solid fa-trash"></i></button>
                        </div>
                    </td>
                    <td class="col-2fa">
                        ${!usuarioActivo 
                            ? `<button class="btn-activar-2fa" onclick="configurar2FA(${idUsuario})"><i class="fa-solid fa-qrcode"></i> Activar</button>`
                            : '<span class="sin-2fa">-</span>'
                        }
                    </td>
                `;
                tbody.appendChild(tr);
            });
        }
    } catch (error) { console.error("Error al cargar usuarios:", error); }
}

function configurar2FA(idUsuario) {
    window.location.href = `configurar-2fa.html?id=${idUsuario}`;
}

const modal = document.getElementById("modal-usuario");
const formCrud = document.getElementById("form-usuario-crud");
const rowPassword = document.getElementById("row-password");

document.getElementById("btn-nuevo-usuario")?.addEventListener("click", () => {
    if (formCrud) formCrud.reset();
    const setIdUsuario = document.getElementById("crud-id-usuario");
    if (setIdUsuario) setIdUsuario.value = "";
    const setIdPersona = document.getElementById("crud-id-persona");
    if (setIdPersona) setIdPersona.value = "";
    const setTitle = document.getElementById("modal-titulo");
    if (setTitle) setTitle.textContent = "Nuevo Usuario";
    if (rowPassword) rowPassword.style.display = "flex";
    const passwordInput = document.getElementById("crud-password");
    if (passwordInput) { passwordInput.value = ""; passwordInput.required = true; }
    if (modal) modal.classList.remove("hidden");
});

document.getElementById("btn-cerrar-modal")?.addEventListener("click", () => { if (modal) modal.classList.add("hidden"); });
document.getElementById("btn-cancelar-modal")?.addEventListener("click", () => { if (modal) modal.classList.add("hidden"); });

function abrirModalEditar(usuarioEncoded) {
    try {
        const u = JSON.parse(decodeURIComponent(usuarioEncoded));
        if (!modal) return;
        const setTitle = document.getElementById("modal-titulo");
        if (setTitle) setTitle.textContent = "Editar Usuario";
        const setIdUsuario = document.getElementById("crud-id-usuario");
        if (setIdUsuario) setIdUsuario.value = u.idUsuario || "";
        const setIdPersona = document.getElementById("crud-id-persona");
        if (setIdPersona) setIdPersona.value = u.idPersona || "";
        const campos = [
            { id: "crud-nombre", valor: u.nombre || "" }, { id: "crud-apPat", valor: u.apPat || "" },
            { id: "crud-apMat", valor: u.apMat || "" }, { id: "crud-ci", valor: u.ci || "" },
            { id: "crud-email", valor: u.email || "" }, { id: "crud-username", valor: u.nombreUsuario || "" }
        ];
        campos.forEach(campo => { const el = document.getElementById(campo.id); if (el) el.value = campo.valor; });
        const rolSelect = document.getElementById("crud-rol");
        if (rolSelect) rolSelect.value = u.idRol || "1";
        const estadoSelect = document.getElementById("crud-estado");
        if (estadoSelect) estadoSelect.value = (u.estado !== false).toString();
        if (rowPassword) rowPassword.style.display = "none";
        const passwordInput = document.getElementById("crud-password");
        if (passwordInput) { passwordInput.value = ""; passwordInput.required = false; }
        modal.classList.remove("hidden");
    } catch (error) {
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudieron cargar los datos.' });
    }
}

if (formCrud) {
    formCrud.addEventListener("submit", async (e) => {
        e.preventDefault();
        const idUsuarioEl = document.getElementById("crud-id-usuario");
        const idPersonaEl = document.getElementById("crud-id-persona");
        const dto = {
            idUsuario: idUsuarioEl.value ? parseInt(idUsuarioEl.value) : null,
            idPersona: idPersonaEl && idPersonaEl.value ? parseInt(idPersonaEl.value) : null,
            nombre: document.getElementById("crud-nombre").value, 
            apPat: document.getElementById("crud-apPat").value,
            apMat: document.getElementById("crud-apMat").value, 
            ci: document.getElementById("crud-ci").value, 
            email: document.getElementById("crud-email").value,
            nombreUsuario: document.getElementById("crud-username").value, 
            idRol: parseInt(document.getElementById("crud-rol").value),
            contrasena: document.getElementById("crud-password").value || null,
            estado: document.getElementById("crud-estado").value === "true"
        };
        const url = dto.idUsuario ? `${API_BASE_URL}/Usuarios/${dto.idUsuario}` : `${API_BASE_URL}/Usuarios`;
        const method = dto.idUsuario ? "PUT" : "POST";
        try {
            const response = await fetch(url, { method: method, headers: { ...getAuthHeaders(), "Content-Type": "application/json" }, body: JSON.stringify(dto) });
            const data = await response.json();
            if (response.ok) {
                Swal.fire({ icon: 'success', title: '¡Guardado!', text: data.mensaje || 'Operación completada.', timer: 2000, showConfirmButton: false });
                if (modal) modal.classList.add("hidden");
                if (typeof cargarUsuariosAPI === 'function') cargarUsuariosAPI();
            } else {
                Swal.fire({ icon: 'error', title: 'Error', text: data.mensaje || "No se pudo completar." });
            }
        } catch (error) {
            Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar.' });
        }
    });
}

async function eliminarUsuario(id) {
    const result = await Swal.fire({ title: '¿Estás seguro?', text: "Esta acción desactivará al usuario", icon: 'warning', showCancelButton: true, confirmButtonColor: '#ef4444', cancelButtonColor: '#64748b', confirmButtonText: 'Sí, desactivar', cancelButtonText: 'Cancelar' });
    if (!result.isConfirmed) return;
    try {
        const response = await fetch(`${API_BASE_URL}/Usuarios/${id}`, { method: "DELETE", headers: getAuthHeaders() });
        if (response.ok) {
            Swal.fire({ icon: 'success', title: 'Desactivado', text: 'El usuario ha sido desactivado.', timer: 2000, showConfirmButton: false });
            if (typeof cargarUsuariosAPI === 'function') cargarUsuariosAPI();
        } else {
            const data = await response.json();
            Swal.fire({ icon: 'error', title: 'Error', text: data.mensaje || "No se pudo desactivar." });
        }
    } catch (error) {
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar.' });
    }
}

// === MÓDULO DE VENTAS ===
async function cargarVentasAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/Ventas`, { headers: getAuthHeaders() });
        if (response.ok) {
            const ventas = await response.json();
            const tbody = document.getElementById("tabla-ventas");
            if (tbody) {
                tbody.innerHTML = "";
                if (ventas.length === 0) {
                    tbody.innerHTML = "<tr><td colspan='6' style='text-align:center; padding: 20px;'>No hay ventas registradas</td></tr>";
                    return;
                }
                ventas.forEach(v => {
                    const tr = document.createElement("tr");
                    tr.innerHTML = `
                        <td><strong>VNT-${(v.idVenta || v.IdVenta).toString().padStart(4, '0')}</strong></td>
                        <td>${v.clienteNombre || v.ClienteNombre || 'Cliente'}</td>
                        <td>${v.productoNombre || v.ProductoNombre || 'Producto'}</td>
                        <td>${v.cantidad || v.Cantidad} unids.</td>
                        <td>Bs. ${(v.totalVenta || v.TotalVenta || 0).toFixed(2)}</td>
                        <td><span class="status-completed">Completada</span></td>
                    `;
                    tbody.appendChild(tr);
                });
            }
        }
    } catch (error) { console.error("Error al cargar ventas:", error); }
}

async function cargarClientesVentasAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/Clientes`, { headers: getAuthHeaders() });
        if (response.ok) {
            const clientes = await response.json();
            const select = document.getElementById("venta-cliente");
            if (select) {
                select.innerHTML = '<option value="">Seleccione un cliente</option>';
                clientes.forEach(c => {
                    const option = document.createElement("option");
                    option.value = c.idCliente || c.IdCliente;
                    option.textContent = `${c.nombre || c.Nombre} ${c.apellido || c.Apellido || ''}`.trim() || `Cliente ${c.idCliente || c.IdCliente}`;
                    select.appendChild(option);
                });
            }
        }
    } catch (error) { console.error("Error al cargar clientes:", error); }
}

async function cargarProductosVentasAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/Productos`, { headers: getAuthHeaders() });
        if (response.ok) {
            const productos = await response.json();
            const select = document.getElementById("venta-producto");
            if (select) {
                select.innerHTML = '<option value="">Seleccione un producto</option>';
                productos.forEach(p => {
                    const option = document.createElement("option");
                    option.value = p.idProducto || p.IdProducto;
                    option.textContent = `${p.nombre} - Stock: ${p.stockActual} - Bs. ${(p.precioVenta || 0).toFixed(2)}`;
                    option.dataset.precio = p.precioVenta || 0;
                    select.appendChild(option);
                });
            }
        }
    } catch (error) { console.error("Error al cargar productos:", error); }
}

async function crearVentaAPI(ventaDto) {
    try {
        const response = await fetch(`${API_BASE_URL}/Ventas`, { method: "POST", headers: getAuthHeaders(), body: JSON.stringify(ventaDto) });
        if (response.ok) { 
            Swal.fire({ icon: 'success', title: 'Venta Registrada', text: 'La venta se ha guardado correctamente.', timer: 2000, showConfirmButton: false });
            if (typeof cargarVentasAPI === 'function') cargarVentasAPI();
            const modalVenta = document.getElementById("modal-venta");
            if (modalVenta) modalVenta.classList.add("hidden");
        } else { 
            const err = await response.json(); 
            Swal.fire({ icon: 'error', title: 'Error', text: err.mensaje || "No se pudo registrar la venta." }); 
        }
    } catch (error) { 
        Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo conectar con el servidor.' });
    }
}

function calcularTotalVenta() {
    const selectProducto = document.getElementById("venta-producto");
    const cantidadInput = document.getElementById("venta-cantidad");
    const totalInput = document.getElementById("venta-total");
    if (selectProducto && cantidadInput && totalInput) {
        const option = selectProducto.options[selectProducto.selectedIndex];
        const precio = parseFloat(option.dataset.precio) || 0;
        const cantidad = parseInt(cantidadInput.value) || 0;
        totalInput.value = (precio * cantidad).toFixed(2);
    }
}