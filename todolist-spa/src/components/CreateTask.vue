<template>
  <div v-if="show" class="modal fade show" tabindex="-1" style="display: block;" aria-modal="true" role="dialog">
    <div class="modal-dialog modal-lg">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="exampleModalLabel">Crear nueva tarea</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" @click="closeModal"></button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="createTask">
            <div class="form-group p-2">
              <label for="taskName">Nombre de la tarea</label>
              <input type="text" v-model="newTask.taskName" class="form-control" id="taskName" placeholder="Ingrese el nombre de la tarea" required>
              <small v-if="errors.taskName" class="text-danger">{{ errors.taskName }}</small>
            </div>
            <div class="form-group p-2">
              <label for="expDate">Fecha de vencimiento (opcional)</label>
              <input type="date" v-model="newTask.expirationDate" class="form-control" id="expDate">
              <small v-if="errors.expirationDate" class="text-danger">{{ errors.expirationDate }}</small>
            </div>
            <div class="modal-footer">
              <button type="submit" class="btn btn-primary mt-2">Crear tarea</button>
              <button type="button" class="btn btn-secondary" @click="closeModal">Cancelar</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits, reactive } from 'vue';
import { useTaskStore } from '../storage/task.js';

const props = defineProps({
  show: Boolean
});

const emit = defineEmits(['update:show', 'close', 'reloadTasks']);

const newTask = reactive({
  taskName: '',
  expirationDate: null,
});

const errors = reactive({
  taskName: '',
  expirationDate: ''
});

const taskStore = useTaskStore();

const createTask = async () => {
  errors.taskName = '';
  errors.expirationDate = '';

  try {
    const expiration = newTask.expirationDate ? newTask.expirationDate : null;
    await taskStore.createTask(newTask.taskName, expiration);
    newTask.taskName = '';
    newTask.expirationDate = null;
    closeModal();
  } catch (error) {
    console.log(error);

    if (error instanceof Error) {
      const errorMessage = error.message;
      
      if (errorMessage.includes('El nombre de la tarea')) {
        errors.taskName = errorMessage
      } else if (errorMessage.includes('La fecha de expiración')) {
        errors.expirationDate = errorMessage
      } else {
        console.error('Error inesperado:', errorMessage);
      }
    } else {
      console.error('Error inesperado:', error);
    }
  }
};

const closeModal = () => {
  emit('update:show', false);
  emit('close');
  emit('reloadTasks');
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
