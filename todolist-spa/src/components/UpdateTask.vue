<template>
  <div v-if="show" class="modal fade show" tabindex="-1" style="display: block;" aria-modal="true" role="dialog">
    <div class="modal-dialog modal-lg">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="exampleModalLabel">Editar tarea</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"
            @click="closeModal"></button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="updateTask">
            <div class="form-group p-2">
              <label for="taskName">Nombre de la tarea</label>
              <input type="text" v-model="taskData.taskName" class="form-control" id="taskName"
                placeholder="Ingrese el nombre de la tarea" required>
              <small v-if="errors.taskName" class="text-danger">{{ errors.taskName }}</small>
            </div>
            <div class="form-group p-2">
              <label for="expirationDate">Fecha de vencimiento</label>
              <input type="date" v-model="taskData.expirationDate" class="form-control" id="expirationDate">
              <small v-if="errors.expirationDate" class="text-danger">{{ errors.expirationDate }}</small>
            </div>
            <div class="form-group p-2">
              <label for="status">Estado</label>
              <select v-model="taskData.status" class="form-control" id="status">
                <option value="completada">Completada</option>
                <option value="activa">Activa</option>
                <option value="diferida">Diferida</option>
              </select>
              <small v-if="errors.status" class="text-danger">{{ errors.status }}</small>
            </div>
            <div class="modal-footer">
              <button type="submit" class="btn btn-primary mt-2" :disabled="isEditingTaskLoading">Guardar
                cambios</button>
              <button type="button" class="btn btn-secondary" @click="closeModal">Cancelar</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits, reactive, ref, watch } from 'vue';
import { useTaskStore } from '../storage/task.js';

const props = defineProps({
  show: Boolean,
  editingTask: {
    type: Object,
    required: true,
  },
});

const emit = defineEmits(['update:show', 'close', 'reloadTasks']);

const isEditingTaskLoading = ref(false);
const errors = reactive({
  taskName: '',
  expirationDate: '',
  status: '',
});

const taskData = reactive({
  taskName: '',
  expirationDate: '',
  status: '',
});


const convertToISOFormat = (dateStr) => {
  if (!dateStr) return null;
  const [day, month, year] = dateStr.split('/');
  return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
};


const convertToDisplayFormat = (dateStr) => {
  if (!dateStr) return null;
  const [year, month, day] = dateStr.split('-');
  return `${day}/${month}/${year}`;
};


watch(
  () => props.editingTask,
  (newTask) => {
    if (newTask) {
      taskData.taskName = newTask.taskName || '';
      taskData.expirationDate = convertToISOFormat(newTask.expirationDate) || '';
      taskData.status = newTask.status || '';
    }
  },
  { immediate: true }
);

const taskStore = useTaskStore();

const updateTask = async () => {
  errors.taskName = '';
  errors.expirationDate = '';
  errors.status = '';

  if (!isValidForm()) {
    return;
  }

  isEditingTaskLoading.value = true;

  console.log('que hay en fecha' + taskData.expirationDate);

  try {
    const updateData = {
      taskName: taskData.taskName,
      expirationDate: taskData.expirationDate,
      status: taskData.status,
    };

    if(!(updateData.expirationDate)){
      console.log('que hay aca?' + updateData.expirationDate);
      updateData.expirationDate = null
    }

    const response = await taskStore.updateTask(props.editingTask.taskId, updateData);

    if (!response.ok) {
      const errorData = await response.json();
      handleBackendErrors(errorData.errors || {});
      throw new Error(errorData.message || 'Error updating task');
    }

    taskStore.tasks = taskStore.tasks.map(task => task.taskId === props.editingTask.taskId ? { ...task, ...taskData } : task);
    closeModal();
    emit('reloadTasks');
  } catch (error) {
    console.error('Error updating task:', error);
  } finally {
    isEditingTaskLoading.value = false;
  }
};

const isValidForm = () => {
  let isValid = true;

  
  if (taskData.taskName.length < 3 || taskData.taskName.length > 100) {
    errors.taskName = 'El nombre de la tarea debe tener entre 3 y 100 caracteres';
    isValid = false;
  } else {
    const invalidCharsRegex = new RegExp('[^a-zA-Z0-9#$%&()\\[\\]; ]');
    if (invalidCharsRegex.test(taskData.taskName)) {
      errors.taskName = 'El nombre de la tarea solo puede contener letras, números y símbolos: $ # %& ( ) [ ] ;';
      isValid = false;
    } else if (taskData.taskName.toUpperCase() === taskData.taskName) {
      errors.taskName = 'El nombre de la tarea no debe estar escrito todo en mayúsculas';
      isValid = false;
    }
  }


  if (taskData.expirationDate) {
    const expirationDate = new Date(taskData.expirationDate);
    console.log('Fecha de vencimiento:', expirationDate);

    const dayOfWeek = expirationDate.getDay();
    console.log('Day of week:', dayOfWeek);

    if (dayOfWeek === 5 || dayOfWeek === 6) {
      errors.expirationDate = 'La fecha de expiración no puede ser un fin de semana';
      isValid = false;
    } else {
      const today = new Date();
      if (expirationDate > new Date(today.setDate(today.getDate() + 30))) {
        errors.expirationDate = 'La fecha de expiración no puede ser más de 30 días posterior a la fecha actual';
        isValid = false;
      }
    }
  }

  return isValid;
};



const closeModal = () => {
  emit('update:show', false);
  emit('close');
};
</script>

<style scoped>
.modal-dialog {
  max-width: 600px;
}

.modal.fade.show {
  display: block;
}
</style>
