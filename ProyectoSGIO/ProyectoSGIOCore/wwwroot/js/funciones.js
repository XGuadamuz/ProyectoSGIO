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


//Funcion para Agrear Tareas a un Proyecto
let tareaIndex = 1;

function agregarTarea() {
    const container = document.getElementById('tareas-container');
    const newTarea = document.createElement('div');
    newTarea.className = 'tarea';
    newTarea.innerHTML = `
            <input type="text" name="tareas[${tareaIndex}].Nombre" placeholder="Nombre de la Tarea" required />
            <input type="date" name="tareas[${tareaIndex}].FechaInicio" required />
            <input type="date" name="tareas[${tareaIndex}].FechaFin" required />
            <input type="checkbox" name="tareas[${tareaIndex}].Completada" /> Completada
        `;
    container.appendChild(newTarea);
    tareaIndex++;
}