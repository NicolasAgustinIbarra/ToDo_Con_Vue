import { defineStore } from "pinia";

export const useTaskStore = defineStore('task', {
  state: () => ({
    tasks: [],
  }),

  actions: {
    async getTasks() {
      try {
        const uri = 'https://localhost:7017/Task';
        const rawResponse = await fetch(uri, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
          },
        });

        if (!rawResponse.ok) {
          throw new Error('Error fetching tasks');
        }

        const response = await rawResponse.json();
        this.tasks = response;
      } catch (error) {
        console.error('Error fetching tasks:', error);
      }
    },

    async createTask(taskName, expirationDate) {
      try {
        const uri = 'https://localhost:7017/Task';
        const rawResponse = await fetch(uri, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
          },
          body: JSON.stringify({
            taskName: taskName,
            status: "",
            expirationDate: expirationDate,
            createdDay: ""
          })
        });
    
        if (!rawResponse.ok) {
          const errorResponse = await rawResponse.json();
          throw new Error(JSON.stringify(errorResponse)); 
        }
    
        const response = await rawResponse.json();
        this.tasks.push(response);
      } catch (error) {
        console.error('Error creating task:', error);
        throw error; 
      }
    },
    
    async updateTask(taskId, updateData) {
      try {
        const uri = `https://localhost:7017/Task/${taskId}`;
        const rawResponse = await fetch(uri, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
          },
          body: JSON.stringify(updateData)
        });
    
        if (!rawResponse.ok) {
          const errorResponse = await rawResponse.json();
          throw new Error(errorResponse.message || 'Error updating task');
        }
    
        
        this.tasks = this.tasks.map(task => task.taskId === taskId ? { ...task, ...updateData } : task);
    
        return rawResponse; 
      } catch (error) {
        console.error('Error updating task:', error);
        throw error; 
      }
    },
    
    
    async getTaskById(taskId) {
      try {
        if (!taskId) {
          throw new Error('No task ID provided');
        }

        const uri = `https://localhost:7017/Task/${taskId}`;
        const response = await fetch(uri);

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || 'Error fetching task details');
        }

        const taskData = await response.json();
        this.editingTask = taskData;
      } catch (error) {
        console.error('Error fetching task details:', error);
      }
    },

    async deleteTask(taskId) {
      try {
        if (!taskId) {
          throw new Error('No task ID provided');
        }

        const uri = `https://localhost:7017/Task/${taskId}`;
        const response = await fetch(uri, {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
          },
        });

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || 'Error deleting task');
        }

      
        this.tasks = this.tasks.filter(task => task.taskId !== taskId);
      } catch (error) {
        console.error('Error deleting task:', error);
      }
    },

    async updateTaskStatus(taskId, newStatus) {
      try {
        const uri = `https://localhost:7017/Task/${taskId}/status?newStatus=${newStatus}`;
        const response = await fetch(uri, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if (!response.ok) {
          const errorResponse = await response.json();
          throw new Error(errorResponse.message || 'Error updating task status');
        }
      } catch (error) {
        console.error('Error updating task status:', error);
        throw error; 
      }
    },
  }
});

export default useTaskStore;
