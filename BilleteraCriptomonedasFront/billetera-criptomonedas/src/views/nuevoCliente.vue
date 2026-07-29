
<script setup>
import { reactive, ref } from 'vue';

let nombreCliente = ref('');
let nombre = ref('');
let email = ref('');


let objetoErrores = reactive({
  nombreCliente:[],
  nombre: [],
  email: []
})


async function validarCampos() {

  objetoErrores['nombreCliente'] = [];
  objetoErrores['nombre'] = [];
  objetoErrores['email'] = [];

  // verificar si el nombre de usuario existe
  
  let clientes = await extraerTodosLosClientes();
  let clienteYaExiste = clientes.some(u=> u.nombreCliente === nombreCliente.value);
  if(clienteYaExiste){
    objetoErrores['nombreCliente'].push("Ese nombre de usuario ya existe");
  }


  let n = nombre.value.trim();
  const regexSimbolos = /[^A-Za-z0-9Á-ÿ\s]/;

  if ((!n.includes(" "))) {
    objetoErrores['nombre'].push("Debe ingresar nombre y apellido");
  }
  if (/\d/.test(n)) {
    objetoErrores['nombre'].push("No se pueden ingresar números");
  }
  if (regexSimbolos.test(n)) {
    objetoErrores['nombre'].push("No se pueden ingresar símbolos");
  }


  let e = email.value.trim();
  let dominio = e.split("@")[1];

  let dominios = ["hotmail.com", "gmail.com", "outlook.com", "yahoo.com"];

  if (!dominios.includes(dominio)) {
    objetoErrores["email"].push("El dominio del correo es incorrecto");
  }

  let errores = Object.values(objetoErrores).some(x => x.length>0);
  if(errores){
    console.log("La validacion a fallado. No se envia el formulario");
    return;
  }

  enviarAlServidor();
}


async function extraerTodosLosClientes(){
  let response  = await fetch('http://localhost:5056/api/Cliente/');
  let clientes = await response.json();
  console.log(response );
  console.log(clientes);
  return clientes; 
}

async function enviarAlServidor(){
  const payload = {
    nombre: nombre.value.trim(),
    email: email.value.trim(),
    nombreCliente: nombreCliente.value.trim()
  };
  console.log(payload); // hasta aca bien
  try {
    const response = await fetch('http://localhost:5056/api/Cliente/', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload)
    });

    if(!response.ok){
      throw new Error('Error al crear el cliente');
    }

    const data = await response.json();
    console.log("Cliente creado:", data);

    nombre.value = "";
    email.value = "";
    nombreCliente.value = "";
  } catch (error) {
    console.error('Error:', error);
  }
}
</script>

<template>

  <div class="wallet-container">
    <div class="wallet-card">
      <h1>Alta de Cliente</h1>
      <p class="subtitle">Registra los datos para iniciar en la plataforma</p>

      <form @submit.prevent="validarCampos">


        <div class="form-group">
          <label for="input-nombre">Nombre y Apellido</label>
          <input v-model="nombre" id="input-nombre" type="text" placeholder="Ej. Juan Pérez" required>
          <p v-for="error in objetoErrores['nombre']" :key="error" class="mensaje-error">{{ error }}</p>
        </div>

        <div class="form-group">
          <label for="input-nombre-cliente">Nombre del Cliente</label>
          <input v-model="nombreCliente" id="input-nombre-cliente" type="text" placeholder="Ej. JuanPerez123" required>
          <p v-for="error in objetoErrores['nombreCliente']" :key="error" class="mensaje-error">{{ error }}</p>
        </div>




        <div class="form-group">
          <label for="input-email">Correo Electrónico</label>
          <input v-model="email" id="input-email" type="email" placeholder="ejemplo@gmail.com" required>
          <p v-for="error in objetoErrores['email']" :key="error" class="mensaje-error">{{ error }}</p>
        </div>


        <button id="button-agregar" type="submit">Crear Cuenta</button>

      </form>
    </div>
  </div>
</template>

<style scoped>
.wallet-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 85vh;
  background-color: #f8fafc;
  font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}


.wallet-card {
  background: #ffffff;
  padding: 2.5rem;
  border-radius: 16px;
  box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.05);
  width: 100%;
  max-width: 400px;
}

h1 {
  margin: 0 0 0.5rem 0;
  font-size: 24px;
  color: #0f172a;
  text-align: center;
}

.subtitle {
  margin: 0 0 2rem 0;
  font-size: 14px;
  color: #64748b;
  text-align: center;
}


.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-size: 14px;
  font-weight: 500;
  color: #334155;
}


input {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  font-size: 14px;
  color: #0f172a;
  box-sizing: border-box;
  transition: all 0.2s ease;
}

input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 4px rgba(59, 130, 246, 0.1);
}


#button-agregar {
  width: 100%;
  padding: 14px;
  background-color: #2563eb;
  color: #ffffff;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s ease;
  margin-top: 0.5rem;
}

#button-agregar:hover {
  background-color: #1d4ed8;
}


.mensaje-error {
    color: #dc3545;          
    font-size: 13px;
    margin-top: -10px;        
    margin-bottom: 15px;      
    font-weight: bold;
}


</style>