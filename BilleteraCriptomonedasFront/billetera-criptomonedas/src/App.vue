
<script setup>
import { RouterView } from 'vue-router';
</script>

<template>
    <RouterView/>
    <nav class="navegacion">
        <router-link to="/venderCriptomonedas" class="link-nav">Vender Cripto</router-link>
        <router-link to="/comprarCriptomonedas" class="link-nav">Comprar Cripto</router-link>
        <router-link to="/historialTransacciones" class="link-nav">Ver Historial</router-link>
        <router-link to="/nuevoCliente" class="link-nav">Alta de Cliente</router-link>
        <router-link to="/historialCliente" class="link-nav">Historial de Cliente</router-link>
    </nav>
 
    
</template>

<style scoped>
 
.navegacion {
    display: flex;
    justify-content: center;
    gap: 10px;
    margin: 20px 0;
    font-family: Arial, sans-serif;
}
 
.link-nav {
    padding: 10px 20px;
    border: 1px solid #ccc;
    border-radius: 5px;
    background-color: #f9f9f9;
    cursor: pointer;
    font-size: 14px;
    font-weight: bold;
    color: #444;
    text-decoration: none;
    transition: background-color 0.2s ease;
}
 
.link-nav:hover {
    background-color: #e9ecef;
}
 
.link-nav.router-link-active {
    background-color: #28a745;
    color: white;
    border-color: #28a745;
}
 
</style>


