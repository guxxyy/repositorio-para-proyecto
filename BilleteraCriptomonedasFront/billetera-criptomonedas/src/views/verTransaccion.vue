<script setup>

import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router'; 

const route = useRoute(); 
const router = useRouter();
let transaccion = ref();


if (!route.query.Transaccion) {
    console.warn("Redirigiendo...");
    router.push('/'); 
} else {
    
    transaccion.value = JSON.parse(route.query.Transaccion);
}


</script>

<template>
    
    <div v-if="transaccion">
        <h1> Informacion de Transaccion</h1>
        <p>Nombre: {{ transaccion.nombreCliente }}</p>
        <p>Criptomoneda: {{ transaccion.cryptoCode }}</p>
        <p>Accion: {{ transaccion.action }}</p>
        <p>Cantidad: {{ transaccion.cryptoAmount }}</p>
        <p>Monto(Ars): {{ transaccion.moneySpent }}</p>
        <p>Fecha y Hora: {{ transaccion.transactionDateTime }}</p>
    </div>

    <div v-else>
        <p>Cargando la informacion de la transaccion</p>
    </div>

</template>

<style></style>