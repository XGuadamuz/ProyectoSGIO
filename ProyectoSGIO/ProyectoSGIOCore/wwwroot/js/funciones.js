// Función para generar una contraseña automáticamente
document.getElementById('generar-contrasena').addEventListener('click', function () {
    const contraseña = Math.random().toString(36).slice(-8);
    document.getElementById('contraseña').value = contraseña;
});


// Función para mostrar/ocultar la contraseña
function togglePasswordVisibility(inputId, iconId) {
    const passwordInput = document.getElementById(inputId);
    const icon = document.getElementById(iconId);

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        icon.classList.remove("bi-eye-slash");
        icon.classList.add("bi-eye");
    } else {
        passwordInput.type = "password";
        icon.classList.remove("bi-eye");
        icon.classList.add("bi-eye-slash");
    }
}


// Función para habilitar la edición del correo
document.getElementById('edit-correo').addEventListener('click', function () {
    const correoInput = document.getElementById('correo');
    if (correoInput.disabled) {
        correoInput.disabled = false;
        correoInput.focus();
    } else {
        correoInput.disabled = true;
    }
});