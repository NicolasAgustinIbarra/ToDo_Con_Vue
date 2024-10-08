import { defineStore } from "pinia";

export const useDataBaseStore = defineStore('database', {
  state: () => ({
 
  }),

  actions: {
    async actualizarConexion(nuevaConexion) {
      try {
        const response = await fetch('https://localhost:7017/api/connection/actualizar-conexion', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ newConnectionString: nuevaConexion }),
        });

        if (!response.ok) {
          throw new Error(`Error: ${response.status} - ${response.statusText}`);
        }

        const data = await response.json().catch(() => {
          throw new Error('La respuesta no es un JSON válido');
        });

        
        localStorage.setItem('nuevaConexion', nuevaConexion);

        return data;
      } catch (error) {
        console.error('Error al actualizar la conexión:', error);
        throw error;
      }
    },

    obtenerConexion() {
      return localStorage.getItem('nuevaConexion');
    },
  },
});
