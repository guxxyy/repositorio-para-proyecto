<script setup>
import { onMounted, ref } from 'vue';

const transacciones = ref([]);
const cargando = ref(false); 
const mensajeError = ref('');


const clienteSeleccionado = ref('');


const clientes = ref([]); 

onMounted(async () => {
    await traerClientes();
});

async function traerClientes(){
    try {
        var response = await fetch('http://localhost:5056/api/Cliente/');
        if(!response.ok){
            throw new Error('No se pudo obtener todos los clientes');
        }
        
        
        var listaRecibida = await response.json(); 
        
        
        for (let cliente of listaRecibida){
            clientes.value.push(cliente.nombreCliente);
        }
    }
    catch(error){
        console.error(error);
    }
}




async function obtenerHistorial(){
    if (!clienteSeleccionado.value) {
        mensajeError.value = 'Por favor, selecciona un cliente primero.';
        return;
    }

    cargando.value = true;
    mensajeError.value = '';
    transacciones.value = [];

    try {
        
        const response = await fetch(`http://localhost:5056/api/Transacciones/${clienteSeleccionado.value}`);

        if(!response.ok){
            throw new Error('No se pudo obtener el historial de este cliente');
        }

        const data = await response.json(); 
        transacciones.value = data;

    } catch (error) {
        mensajeError.value = error.message || 'Error al obtener el historial';
        console.error('Error:', error);
    } finally {
        cargando.value = false;
    }
}
</script>

<template>
    <h1 class="titulo-principal">Historial de Movimientos</h1>

    <div class="contenedor-filtro">
        <label for="select-cliente">Seleccionar Cliente: </label>
        <select id="select-cliente" v-model="clienteSeleccionado" class="input-select">
            <option value="" disabled>-- Seleccione un cliente --</option>
            <option v-for="cliente in clientes" :key="cliente" :value="cliente">
                {{ cliente }}
            </option>
        </select>
        
        <button @click="obtenerHistorial" class="btn-buscar" :disabled="cargando">
            Buscar Transacciones
        </button>
    </div>

    <div v-if="mensajeError" class="mensaje-error-global">
        {{ mensajeError }}
    </div>

    <div v-if="cargando" class="mensaje-cargando">
        Cargando historial...
    </div>

    <table v-else-if="transacciones.length > 0" class="tabla-historial">
        <thead>
            <tr>
                <th>Cliente</th>
                <th>Criptomoneda</th>
                <th>Acción</th>
                <th>Cantidad</th>
                <th>Monto (ARS)</th>
                <th>Fecha</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="transaccion in transacciones" :key="transaccion.id">
                <td>{{ transaccion.nombreCliente }}</td>
                <td>{{ transaccion.cryptoCode }}</td>
                <td>{{ transaccion.action }}</td>
                <td>{{ transaccion.cryptoAmount }}</td>
                <td>{{ transaccion.moneySpent }}</td>
                <td>{{ transaccion.transactionDateTime }}</td>
            </tr>
        </tbody>
    </table>

    <div v-else-if="clienteSeleccionado && !cargando" class="mensaje-vacio">
        No hay transacciones registradas para este cliente
    </div>
</template>

<style scoped>

.contenedor-filtro {
    max-width: 700px;
    margin: 20px auto;
    display: flex;
    gap: 15px;
    align-items: center;
    justify-content: center;
    font-family: Arial, sans-serif;
}

.input-select {
    padding: 8px 12px;
    border-radius: 5px;
    border: 1px solid #ccc;
    font-size: 14px;
}

.btn-buscar {
    padding: 8px 16px;
    background-color: #007BFF;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-weight: bold;
}

.btn-buscar:hover {
    background-color: #0056b3;
}

.btn-buscar:disabled {
    background-color: #cccccc;
    cursor: not-allowed;
}

</style>