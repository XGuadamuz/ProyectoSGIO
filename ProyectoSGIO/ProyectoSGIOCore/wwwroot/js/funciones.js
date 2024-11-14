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
    const phaseIndex = document.querySelectorAll('.fase-card').length;
    const phaseHtml = `
        <div class="fase-card" data-phase-index="${phaseIndex}">
            <div class="fase-header">
                <label>Ingrese un nombre para esta Fase</label>
                <input type="text" name="fases[${phaseIndex}].Nombre" placeholder="Nombre de la Fase" />
                
                <!-- Botón de eliminación de fase -->
                <button type="button" class="btn-delete-phase" onclick="deletePhase(${phaseIndex})">
                    <i class="fa fa-trash"></i>
                </button>
            </div>

            <hr />
            <h4>Tareas</h4>
            <div class="tareas-container" data-phase-index="${phaseIndex}"></div>

            <button type="button" class="btn btn-success mt-3" onclick="addTask(${phaseIndex})">Agregar Tarea</button>
        </div>
    `;
    document.getElementById('fases-container').insertAdjacentHTML('beforeend', phaseHtml);
}


//Funcion para agrear Tareas a un Proyecto
function addTask(phaseIndex) {
    const tareasContainer = document.querySelector(`.tareas-container[data-phase-index="${phaseIndex}"]`);
    const taskIndex = tareasContainer.querySelectorAll('.tarea-item').length;
    const taskHtml = `
        <div class="tarea-item">
            <input type="text" name="fases[${phaseIndex}].Tareas[${taskIndex}].Nombre" placeholder="Nombre de la Tarea" />

            <label>Fecha de Inicio</label>
            <input type="date" name="fases[${phaseIndex}].Tareas[${taskIndex}].FechaInicio" />

            <label>Fecha de Fin</label>
            <input type="date" name="fases[${phaseIndex}].Tareas[${taskIndex}].FechaFin" />

            <!-- Botón de eliminación -->
            <button type="button" class="btn-delete" onclick="eliminarTarea(this)">
                <i class="fa fa-trash"></i>
            </button>
        </div>
    `;
    tareasContainer.insertAdjacentHTML('beforeend', taskHtml);
}

// Eliminar Fase
function deletePhase(phaseIndex) {
    const phaseCard = document.querySelector(`.fase-card[data-phase-index="${phaseIndex}"]`);
    if (phaseCard) {
        phaseCard.remove();
    }
}

// Eliminar el elemento de tarea
function eliminarTarea(button) {
    const tareaItem = button.closest('.tarea-item');
    tareaItem.remove();
}