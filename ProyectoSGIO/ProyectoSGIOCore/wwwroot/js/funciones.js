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


//Funcion para agrear Fases a un Proyecto
function addPhase() {
    const phaseIndex = document.querySelectorAll('.fase').length;
    const phaseHtml = `
                <div class="fase">
                    <label>Nombre de la Fase</label>
                    <input name="fases[${phaseIndex}].Nombre" />

                    <h4>Tareas</h4>
                    <div class="tareas-container" data-phase-index="${phaseIndex}">
                    </div>
                    <button type="button" onclick="addTask(${phaseIndex})">Agregar Tarea</button>
                </div>
            `;
    document.getElementById('fases-container').insertAdjacentHTML('beforeend', phaseHtml);
}

//Funcion para agrear Tareas a un Proyecto

function addTask(phaseIndex) {
    const tareasContainer = document.querySelector(`.tareas-container[data-phase-index="${phaseIndex}"]`);
    const taskIndex = tareasContainer.querySelectorAll('.tarea').length;
    const taskHtml = `
                <div class="tarea">
                    <label>Nombre de la Tarea</label>
                    <input name="fases[${phaseIndex}].Tareas[${taskIndex}].Nombre" />

                    <label>Fecha de Inicio</label>
                    <input type="date" name="fases[${phaseIndex}].Tareas[${taskIndex}].FechaInicio" />

                    <label>Fecha de Fin</label>
                    <input type="date" name="fases[${phaseIndex}].Tareas[${taskIndex}].FechaFin" />
                </div>
            `;
    tareasContainer.insertAdjacentHTML('beforeend', taskHtml);
}


//// Eliminar el elemento de tarea
//function eliminarTarea(button) {
//    button.parentElement.remove();
//}