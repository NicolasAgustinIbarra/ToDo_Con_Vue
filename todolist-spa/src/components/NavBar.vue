<template>
  <nav id="navbar" class="navbar navbar-expand-lg navbar-light bg-light">
    <div class="container-fluid">
      <div class="navbar-brand">
        <h3 class="nav-item">
          <font-awesome-icon :icon="['fas', 'list-check']" /> ToDo List
        </h3>
      </div>
      <form @submit.prevent="actualizarBaseDatos" style="width: 45%;">
        <div class="input-group flex-nowrap">
          <input 
            v-model="nuevaConexion" 
            type="text" 
            class="form-control border border-info" 
            placeholder="Inserte su base de datos" 
            aria-label="Username" 
            aria-describedby="addon-wrapping"
          >
          <button type="submit" class="nav-item btn btn-outline-info me-2">
            <font-awesome-icon :icon="['fas', 'database']" /> Base de Datos
          </button>
        </div>
      </form>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted, defineEmits } from 'vue';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { useDataBaseStore } from '../storage/database.js';

const nuevaConexion = ref('');
const dataBaseStore = useDataBaseStore();

const emit = defineEmits(['reloadTasks']);

onMounted(() => {
  const conexionGuardada = dataBaseStore.obtenerConexion();
  if (conexionGuardada) {
    nuevaConexion.value = conexionGuardada;
  }
});

const actualizarBaseDatos = async () => {
  try {
    const resultado = await dataBaseStore.actualizarConexion(nuevaConexion.value);
    alert('Configuración de la base de datos exitosa!');
    console.log('Resultado:', resultado);
    emit('reloadTasks');
    // window.location.reload();
  } catch (error) {
    alert('Error al configurar la base de datos');
    console.error('Error:', error);
  }
};
</script>

<style scoped>
.btn {
  color: #023047;
}
</style>
