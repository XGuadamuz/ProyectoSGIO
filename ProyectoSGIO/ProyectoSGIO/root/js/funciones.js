// Función para generar una contraseña automáticamente
document.getElementById('generar-contrasena').addEventListener('click', function () {
    const contraseña = Math.random().toString(36).slice(-8);
    document.getElementById('contraseña').value = contraseña;
});

// Función para mostrar/ocultar la contraseña
document.getElementById('toggle-password').addEventListener('click', function () {
    const contraseñaInput = document.getElementById('contraseña');
    const icono = document.getElementById('icon-password');

    if (contraseñaInput.type === "password") {
        contraseñaInput.type = "text";
        icono.classList.remove('bi-eye-slash');
        icono.classList.add('bi-eye');
    } else {
        contraseñaInput.type = "password";
        icono.classList.remove('bi-eye');
        icono.classList.add('bi-eye-slash');
    }
});