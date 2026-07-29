<script setup>
import { ref, reactive } from 'vue';
import { onMounted} from 'vue';

const cryptoSeleccionada = ref(''); 
const cantidadCrypto = ref(); 
const fechaHoraSeleccionada = ref('');
const nombreCliente = ref('');
const cargando = ref(false);
const mensajeExito = ref('');
const mensajeError = ref('');

const clientes = ref([]);

const objetoErrores = reactive({
    'seleccionarCripto': [],
    'cantidadEnviar': [],
    'ingresarFechaHora':[],
    'seleccionarCliente':[]
});

onMounted(async () => {
    const data = await traerClientes();
    clientes.value = data || []; 
});

async function validarCampos() {
    
    objetoErrores['seleccionarCripto'] = [];
    objetoErrores['cantidadEnviar'] = [];
    objetoErrores['ingresarFechaHora'] = [];
    objetoErrores['seleccionarCliente'] = [];

    
    if (cryptoSeleccionada.value === "") {
        objetoErrores['seleccionarCripto'].push("Debe seleccionar alguna criptomoneda");
    }

    if (!cantidadCrypto.value || cantidadCrypto.value <= 0) {
        objetoErrores['cantidadEnviar'].push("El monto a vender debe ser mayor a 0");
    }

    if (fechaHoraSeleccionada.value === "") {
        objetoErrores['ingresarFechaHora'].push("Debe ingresar la fecha y hora de venta");
    }

    if (nombreCliente.value.trim() === "") {
        objetoErrores['seleccionarCliente'].push("Debe seleccionar un cliente");
    }

    
    let clienteEncontrado = clientes.value.find(c => c.nombreCliente === nombreCliente.value);

    if (!clienteEncontrado) {
        objetoErrores['seleccionarCliente'].push("El Cliente seleccionado no existe");
        return; // Detenemos aquí para evitar errores al buscar la billetera
    }

    
    let billeteras = await traerBilleteras();

    if (!billeteras) {
        objetoErrores['seleccionarCliente'].push("Error de conexión al buscar billeteras. Intentá de nuevo.");
        return;
    }

    let billeteraEncontrada = billeteras.find(b => b.idCliente === clienteEncontrado.id);

    if (!billeteraEncontrada) {
        objetoErrores['seleccionarCliente'].push("El cliente no tiene una billetera registrada.");
    } else if (cryptoSeleccionada.value) {
        let mapaParaBilletera = {
            "BTC": "bitcoin",
            "ETH": "ethereum",
            "USDT": "usdt"
        };
        let propiedadCripto = mapaParaBilletera[cryptoSeleccionada.value];
        let dineroCriptoBilletera = billeteraEncontrada[propiedadCripto] || 0;

        if (dineroCriptoBilletera < cantidadCrypto.value) {
            objetoErrores['cantidadEnviar'].push(`Saldo insuficiente. Tiene ${dineroCriptoBilletera} ${cryptoSeleccionada.value}`);
        }
    }

    
    let hayErrores = Object.values(objetoErrores).some(x => x.length > 0);

    if (hayErrores) {
        console.log("La validación ha fallado. No se envía el formulario.");
        return;
    }

    enviarAlServidor();
}

async function traerClientes(){

    try{
        var response = await fetch('http://localhost:5056/api/Cliente/');
        if(!response.ok){
            throw new Error('No se pudo obtener todos los clientes');
        }
        var clientes = await response.json();
        return clientes; 
    }
    catch(error){
        console.error(error);
        
    }

}

async function traerBilleteras(){
    try{
        var response = await fetch('http://localhost:5056/api/Billetera/');
        if(!response.ok){
            throw new Error("No se pudo obtener todas las billeteras");
        }
        let billeteras = await response.json();
        return billeteras;

    }
    catch(error){
        console.error(error);
    }
}





async function enviarAlServidor(){
    cargando.value = true;
    mensajeExito.value = '';
    mensajeError.value = '';


    const fechaHoraPartes = fechaHoraSeleccionada.value.split('T');
    const fecha = fechaHoraPartes[0]; 
    const hora = fechaHoraPartes[1]; 

    
    const mapaCryptoCode = {
        'BTC': 'bitcoin',
        'ETH': 'ethereum',
        'USDT': 'usdt'
    };

    const payload = {// lo que se envia el servidor
        cryptoCode: mapaCryptoCode[cryptoSeleccionada.value] || cryptoSeleccionada.value.toLowerCase(),
        action: "sale",
        nombreCliente: nombreCliente.value,
        cryptoAmount: cantidadCrypto.value.toString(),
        dateTime: `${fecha} ${hora}`
    };

    try {
        console.log("Payload que se envía:", payload);
        console.log("CryptoCode:", payload.cryptoCode);
        const response = await fetch('http://localhost:5056/api/Transacciones/', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(payload)
        });
        
        const data = await response.json();
        

        if(!response.ok){
            throw new Error(data.error || 'Error al procesar la transacción');
        }

        mensajeExito.value = "¡Compra realizada exitosamente!" ;
        
        
        cryptoSeleccionada.value = "";
        cantidadCrypto.value = undefined; 
        fechaHoraSeleccionada.value = "";

        
        setTimeout(() => {
            mensajeExito.value = '';
        }, 5000);

    } catch (error) {
        mensajeError.value = error.message || 'Error al enviar los datos al servidor';
        console.error('Error:', error);
    } finally {
        cargando.value = false;
    }
}
</script>

<template>
    <h1 class="titulo-principal">Vender Criptomoneda</h1>

    

    <div v-if="mensajeExito" class="mensaje-exito">
        {{ mensajeExito }}
    </div>

    <div v-if="mensajeError" class="mensaje-error-global">
        {{ mensajeError }}
    </div>

    <form @submit.prevent="validarCampos" class="formulario-cripto" novalidate>

        <label for="select-cliente">Ingresa un Cliente</label>
        <select id="select-cliente" v-model="nombreCliente" class="input-campo">
            <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.nombreCliente">
                {{ cliente.nombreCliente }}
            </option>
        </select>
        <p v-for="err in objetoErrores.seleccionarCliente" :key="err" class="mensaje-error">{{ err }}</p>

       

        <label for="crypto-select">Selecciona una Criptomoneda</label>
        <select id="crypto-select" v-model="cryptoSeleccionada" class="input-campo" :disabled="cargando">
            <option value="" disabled>-- Elige una opción --</option>
            <option value="BTC">Bitcoin (BTC)</option>
            <option value="ETH">Ethereum (ETH)</option>
            <option value="USDT">Tether (USDT)</option>
        </select>
        <p v-for="err in objetoErrores.seleccionarCripto" :key="err" class="mensaje-error">{{ err }}</p>

        <label for="monto-crypto">Cantidad a vender</label>
        <input id="monto-crypto" type="number" step="any" v-model.number="cantidadCrypto" placeholder="0.00000000" class="input-campo" :disabled="cargando"/>
        <p v-for="err in objetoErrores.cantidadEnviar" :key="err" class="mensaje-error">{{ err }}</p>

        <label for="datetime-select">Fecha y Hora de Venta</label>
        <input id="datetime-select" type="datetime-local" v-model="fechaHoraSeleccionada" class="input-campo" :disabled="cargando">
        <p v-for="err in objetoErrores.ingresarFechaHora" :key="err" class="mensaje-error">{{ err }}</p>

        <button id="button-comprar-cripto" type="submit" class="btn-comprar" :disabled="cargando">
            {{ cargando ? 'Procesando...' : 'Vender Cripto' }}
        </button>

    </form>
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

.mensaje-exito {
    max-width: 400px;
    margin: 0 auto 15px;
    padding: 12px;
    background-color: #d4edda;
    color: #155724;
    border: 1px solid #c3e6cb;
    border-radius: 5px;
    text-align: center;
    font-weight: bold;
    font-family: Arial, sans-serif;
}

.mensaje-error-global {
    max-width: 400px;
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

.formulario-cripto { 
    max-width: 400px; 
    margin: 0 auto; 
    padding: 25px; 
    background-color: #f9f9f9; 
    border-radius: 10px; 
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1); 
    font-family: Arial, sans-serif; 
}

label { 
    display: block; 
    font-weight: bold; 
    color: #444; 
    margin-bottom: 5px; 
}

.input-campo { 
    width: 100%; 
    padding: 10px; 
    margin-bottom: 15px; 
    border: 1px solid #ccc; 
    border-radius: 5px; 
    box-sizing: border-box; 
    font-size: 15px; 
}

.input-campo:focus { 
    border-color: #007BFF; 
    outline: none; 
}

.input-campo:disabled {
    background-color: #e9ecef;
    cursor: not-allowed;
}

.mensaje-error { 
    color: #dc3545;
    font-size: 13px; 
    margin-top: -10px; 
    margin-bottom: 15px; 
    font-weight: bold; 
}

.btn-comprar { 
    width: 100%; 
    padding: 12px; 
    background-color: #28a745; 
    color: white; 
    border: none; 
    border-radius: 5px; 
    font-size: 16px; 
    font-weight: bold; 
    cursor: pointer; 
    transition: background-color 0.3s ease; 
}

.btn-comprar:hover:not(:disabled) { 
    background-color: #218838; 
}

.btn-comprar:disabled {
    background-color: #6c757d;
    cursor: not-allowed;
}
</style>