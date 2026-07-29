<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router'; 


const router = useRouter();


const transacciones = ref([]);
const cargando = ref(true);
const mensajeError = ref('');

async function obtenerHistorial(){
    cargando.value = true;
    mensajeError.value = '';

    try {
        const response = await fetch('http://localhost:5056/api/Transacciones/');

        if(!response.ok){
            throw new Error('No se pudo obtener el historial');
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


async function verTransaccion(transaccion){

    router.push({
        path: '/verTransaccion', 
        query: {Transaccion: JSON.stringify(transaccion) } 
        

    });
}

async function modificarTransaccion(transaccion){
    router.push({
        path: '/editarTransaccion', 
        query: {Transaccion: JSON.stringify(transaccion) } 
    

    });
}

async function eliminarTransaccion(id){
    const quiereContinuar = window.confirm("¿Realmente quiere eliminar la transacción?");
    
    if(!quiereContinuar){
        console.log("El usuario canceló la eliminación");
        return;
    }
    
    try {
        const response = await fetch(`http://localhost:5056/api/Transacciones/${id}`, { method: 'DELETE' })
        
        if (!response.ok) {
            const errorData = await response.json(); 
            throw new Error(errorData.error || 'Error al procesar la eliminación');
        }   

        console.log("¡Eliminación realizada exitosamente!");
        
        
        await obtenerHistorial(); 
        
    } catch (error) {
        
        mensajeError.value = error.message;
        console.error('Error:', error);
    }
}


onMounted(() => {
    obtenerHistorial();
});
</script>

<template>
    <h1 class="titulo-principal">Historial de Compras</h1>

    

    <div v-if="mensajeError" class="mensaje-error-global">
        {{ mensajeError }}
    </div>

    <div v-if="cargando" class="mensaje-cargando">
        Cargando historial...
    </div>

    <div v-else-if="transacciones.length === 0" class="mensaje-vacio">
        No hay transacciones registradas
    </div>

    <table v-else class="tabla-historial">
        <thead>
            <tr>
                <th>Cliente</th>
                <th>Criptomoneda</th>
                <th>Acción</th>
                <th>Cantidad</th>
                <th>Monto (ARS)</th>
                <th>Fecha de Compra</th>
                <th class="columna-acciones"></th>
            
            </tr>
        </thead>
        <tbody>
            <tr v-for="transaccion in transacciones" :key="transaccion.id" class="fila-transaccion">
                <td>{{ transaccion.nombreCliente}}</td>
                <td>{{ transaccion.cryptoCode }}</td>
                <td>{{ transaccion.action }}</td>
                <td>{{ transaccion.cryptoAmount }}</td>
                <td>{{ transaccion.moneySpent }}</td>
                <td>{{ transaccion.transactionDateTime }}</td>
                <td class="celda-acciones">
                    <div class="contenedor-acciones">
                        <button @click="verTransaccion(transaccion)" class="btn-accion btn-ver" title="Ver">👁️</button>
                        <button @click="modificarTransaccion(transaccion)" class="btn-accion btn-modificar" title="Modificar">✏️</button>
                        <button @click="eliminarTransaccion(transaccion.id)" class="btn-accion btn-borrar" title="Borrar">🗑️</button>
                    </div>

                </td>
            </tr> 
        </tbody>
    </table>
</template>

<style scoped>

.titulo-principal { 
    text-align: center; 
    font-family: Arial, sans-serif; 
    color: #333; 
    margin-top: 20px; 
}


.link-historial {
    font-family: Arial, sans-serif;
    color: #007BFF;
    text-decoration: none;
    font-size: 14px;
    font-weight: bold;
}

.link-historial:hover {
    text-decoration: underline;
    color: #0056b3;
}

.mensaje-error-global {
    max-width: 600px;
    margin: 0 auto 15px;
    padding: 12px;
    background-color: #f8d7da;
    color: #721c24;
    border: 1px solid #f5c6cb;
    border-radius: 5px;
    text-align: center;
    font-weight: bold;
    font-family: Arial, sans-serif;
}

.mensaje-cargando,
.mensaje-vacio {
    text-align: center;
    font-family: Arial, sans-serif;
    color: #666;
    margin-top: 30px;
}

.tabla-historial {
    max-width: 700px;
    margin: 0 auto;
    width: 100%;
    border-collapse: collapse;
    font-family: Arial, sans-serif;
    background-color: #f9f9f9;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
    border-radius: 10px;
    overflow: hidden;
}

.tabla-historial th,
.tabla-historial td {
    padding: 12px 15px;
    text-align: center;
    border-bottom: 1px solid #ddd;
}

.tabla-historial th {
    background-color: #28a745;
    color: white;
    font-weight: bold;
}

.tabla-historial tbody tr:hover {
    background-color: #f1f1f1;
}



.columna-acciones {
    width: 120px;
}

.celda-acciones {
    text-align: right;
    vertical-align: middle;
}


.contenedor-acciones {
    display: flex;
    justify-content: flex-end;
    gap: 8px;
    opacity: 0;
    visibility: hidden;
    transition: opacity 0.2s ease, visibility 0.2s ease;
}


.fila-transaccion:hover .contenedor-acciones {
    opacity: 1;
    visibility: visible;
}


.btn-accion {
    border: none;
    background: none;
    cursor: pointer;
    padding: 6px;
    border-radius: 4px;
    transition: background-color 0.2s;
}

.btn-accion:hover {
    background-color: rgba(0, 0, 0, 0.05);
}

</style>