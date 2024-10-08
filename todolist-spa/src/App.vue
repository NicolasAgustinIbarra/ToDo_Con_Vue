<template>
  <header>
    <Navbar @reloadTasks="handleReloadTasks"></Navbar>
  </header>
  <section>
    <DataTable ref="dataTable" @mounted="onDataTableMounted"></DataTable>
  </section>
</template>

<script setup>
import { ref } from 'vue';
import Navbar from "./components/NavBar.vue";
import DataTable from "./components/DataTable.vue";

const dataTableRef = ref(null);
const isDataTableMounted = ref(false);

const handleReloadTasks = () => {
  if (isDataTableMounted.value && dataTableRef.value) {
    dataTableRef.value.handleReloadTasks();
  } else {
    console.warn('DataTable is not mounted yet or reference is null');
  }
};

const onDataTableMounted = () => {
  isDataTableMounted.value = true;
};

const actualizarBaseDatos = async () => {
  try {
    const resultado = await dataBaseStore.actualizarConexion(nuevaConexion.value);
    alert('Configuración de la base de datos exitosa!');
    console.log('Resultado:', resultado);
    handleReloadTasks(); // Asegurarse de que DataTable esté montado antes de recargar
    emit('reloadTasks');
  } catch (error) {
    alert('Error al configurar la base de datos');
    console.error('Error:', error);
  }
};
</script>
