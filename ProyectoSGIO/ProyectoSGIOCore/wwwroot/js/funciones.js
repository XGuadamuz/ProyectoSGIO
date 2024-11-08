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


////Funcion para Agrear Tareas a un Proyecto
//let tareaIndex = 0;

//function agregarTarea() {
//    // Crear un contenedor de tarea con los inputs organizados en una fila
//    const tareaHTML = `
//                <div class="tarea d-flex align-items-center mb-2" style="gap: 15px;">
//                    <input type="text" name="tareas[${tareaIndex}].Nombre" placeholder="Nombre de la Tarea" class="form-control" required />
//                    <input type="date" name="tareas[${tareaIndex}].FechaInicio" class="form-control" required />
//                    <input type="date" name="tareas[${tareaIndex}].FechaFin" class="form-control" required />
//                    <button type="button" class="btn-close" aria-label="Close" onclick="eliminarTarea(this)"></button>
//                </div>
//            `;

//    // Añadir la tarea al contenedor
//    document.getElementById("tareas-container").insertAdjacentHTML('beforeend', tareaHTML);
//    tareaIndex++;
//}

//// Eliminar el elemento de tarea
//function eliminarTarea(button) {
//    button.parentElement.remove();
//}