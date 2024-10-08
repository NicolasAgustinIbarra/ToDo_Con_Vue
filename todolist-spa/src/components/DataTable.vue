<template>
    <div class="container mt-5">
        <div class="row">
            <div class="col">
                <div class="input-group flex-nowrap">
                    <button class="btn btn-outline-info btn-lg mb-3" @click="showCreateTaskModal = true">
                        <font-awesome-icon :icon="['fas', 'plus']" /> Nueva tarea
                    </button>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" v-model="showActiveTasksOnly" value=""
                            id="flexCheckChecked" checked>
                        <label class="form-check-label" for="flexCheckChecked">
                            Ver solo tareas activas
                        </label>
                    </div>
                    <hr class="border border-info">
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <div class="form-group mb-3">
                        <label for="statusFilter">Filtrar por estado</label>
                            <select v-model="selectedStatus" class="form-control border border-info" id="statusFilter">
                            <option value="">Todos</option>
                            <option value="completada">Completada</option>
                            <option value="activa">Activa</option>
                            <option value="diferida">Diferida</option>
                        </select>
                    </div>
                    <table class="table table-bordered table-striped text-center ">
                        <thead class="thead-dark">
                            <tr>
                                <th scope="col">Nombre</th>
                                <th scope="col">Completar</th>
                                <th scope="col">Fecha de Expiración</th>
                                <th scope="col">Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="task in filteredTasks" :key="task.taskId">
                                <td>{{ task.taskName }}</td>
                                <td>
                                    <button class="btn btn-sm btn-outline-info"
                                        @click="updateTaskStatus(task.taskId)"
                                        :disabled="task.status === 'completada'">
                                        <font-awesome-icon :icon="['fas', 'check']" />
                                    </button>
                                </td>
                                <td>{{ task.expirationDate ? task.expirationDate : '-sin expiración-' }}</td>
                                <td>
                                    <button class="btn btn-sm btn-outline-info"
                                        @reloadTasks="handleReloadTasks"
                                        @click="editTask(task)"><font-awesome-icon :icon="['fas', 'pen']" /></button>
                                    <button class="btn btn-sm  btn-outline-danger ms-1"
                                        @click="deleteTask(task)"><font-awesome-icon
                                            :icon="['fas', 'trash']" /></button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <CreateTask v-model:show="showCreateTaskModal" @close="showCreateTaskModal = false"
            @reloadTasks="handleReloadTasks"></CreateTask>
        <UpdateTask v-model:show="showEditTaskModal" :editing-task="editingTask" @close="showEditTaskModal = false"
            @reloadTasks="handleReloadTasks" />
    </div>
</template>

<script setup>
import { ref, onMounted, computed, defineEmits } from 'vue';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import CreateTask from './CreateTask.vue';
import UpdateTask from './UpdateTask.vue';
import { useTaskStore } from '../storage/task.js';

const showEditTaskModal = ref(false);
const editingTask = ref(null);

const selectedStatus = ref('');
const taskStore = useTaskStore();
const tasks = ref([]);
const showCreateTaskModal = ref(false);
const showActiveTasksOnly = ref(true);

const emit = defineEmits(['mounted']);

onMounted(() => {
  emit('mounted');
});

const fetchTasks = async () => {
    try {
        await taskStore.getTasks();
        tasks.value = taskStore.tasks;
        console.log(tasks.value);
    } catch (error) {
        console.error('Error fetching tasks:', error);
    }
};

onMounted(fetchTasks);

const editTask = (task) => {
    editingTask.value = task;
    showEditTaskModal.value = true;
};

const deleteTask = async (task) => {
  console.log('taskId: ' + task.taskId);
  try {
    await taskStore.deleteTask(task.taskId);
    tasks.value = tasks.value.filter(t => t.taskId !== task.taskId);
  } catch (error) {
    console.error('Error:', error);
  }
};

const updateTaskStatus = async (taskId) => {
  const newStatus = 'completada'; 
  try {
    await taskStore.updateTaskStatus(taskId, newStatus);
    const updatedTaskIndex = tasks.value.findIndex(task => task.taskId === taskId);
    if (updatedTaskIndex !== -1) {
      tasks.value[updatedTaskIndex].status = newStatus;
    }
  } catch (error) {
    console.error('Error:', error);
  }
};

const filteredTasks = computed(() => {
  if (!selectedStatus.value && showActiveTasksOnly.value) {
    return tasks.value.filter(task => task.status === 'activa' || task.status === 'diferida');
  } else if (!selectedStatus.value && !showActiveTasksOnly.value) {
    return tasks.value;
  } else if (selectedStatus.value && !showActiveTasksOnly.value) {
    return tasks.value.filter(task => task.status === selectedStatus.value);
  } else {
    return tasks.value.filter(task => task.status === selectedStatus.value && (task.status === 'activa' || task.status === 'diferida'));
  }
});

const handleReloadTasks = () => {
  fetchTasks(); 
};



</script>

<style scoped></style>