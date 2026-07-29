import ComprarCripto from '@/views/comprarCripto.vue';
import VenderCripto from '@/views/venderCriptomoneda.vue';
import NuevoCliente from '@/views/nuevoCliente.vue';
import HistorialTransacciones from '@/views/HistorialTransacciones.vue';
import HistorialCliente from '@/views/historialCliente.vue';
import VerTransaccion from '@/views/verTransaccion.vue';
import EditarTransaccion from '@/views/editarTransaccion.vue';
import { createRouter, createWebHistory } from 'vue-router';



const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path:'/nuevoCliente',
      name: 'nuevoCliente',
      component: NuevoCliente
    },
    {
      path: '/comprarCriptomonedas',
      name: 'comprarCriptomonedas',
      component: ComprarCripto
    },
    {
      path:'/historialTransacciones',
      name:'historialTransacciones',
      component: HistorialTransacciones
    },
    {
      path:'/venderCriptomonedas',
      name:'venderCriptomonedas',
      component: VenderCripto
    },
    {
      path:'/historialCliente',
      name:'historialCliente',
      component: HistorialCliente
    },
    {
      path:'/verTransaccion',
      name:'verTransaccion',
      component: VerTransaccion
    },
    {
      path:'/editarTransaccion',
      name:'editarTransaccion',
      component: EditarTransaccion
    }
  ],
})

export default router
