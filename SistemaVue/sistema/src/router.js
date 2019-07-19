import Vue from "vue";
import Router from "vue-router";
import Home from "./views/Home.vue";
import Categoria from "./components/Almacen/Categorias.vue";
import Articulo from "./components/Almacen/Articulos.vue";
import Rol from "./components/Accesos/Rol.vue";
import Usuario from "./components/Accesos/Usuario.vue";
import Provedor from "./components/Compras/Proveedores.vue";
import Cliente from "./components/Ventas/Clientes.vue";
import Login from "./components/Login/Login.vue";
import Ingreso from './components/Compras/Ingreso.vue';
import Venta from './components/Ventas/Venta.vue'
import ConsultasVenta from './components/Ventas/ConsultaVentas.vue'
import store from "./store";


Vue.use(Router);

var router = new Router({
  mode: "history",
  base: process.env.BASE_URL,
  routes: [{
      path: "/",
      name: "home",
      component: Home,
      meta: {
        administrador: true,
        almacenero: true,
        vendedor: true
      }
    },
    {
      path: "/Categorias",
      name: "categoria",
      component: Categoria,
      meta: {
        administrador: true,
        almacenero: true
      }
    },
    {
      path: "/Articulos",
      name: "articulo",
      component: Articulo,
      meta: {
        administrador: true,
        almacenero: true
      }
    },
    {
      path: "/Ingresos",
      name: "Ingreso",
      component: Ingreso,
      meta: {
        administrador: true,
        almacenero: true
      }
    },
    {
      path: "/Roles",
      name: "roles",
      component: Rol,
      meta: {
        administrador: true
      }
    },
    {
      path: "/Usuarios",
      name: "Usuario",
      component: Usuario,
      meta: {
        administrador: true
      }
    },
    {
      path: "/Proveedores",
      name: "Provedor",
      component: Provedor,
      meta: {
        administrador: true,
        almacenero: true
      }
    },
    {
      path: "/Clientes",
      name: "Cliente",
      component: Cliente,
      meta: {
        administrador: true,
        vendedor: true
      }
    },
    {
      path: "/Ventas",
      name: "venta",
      component: Venta,
      meta: {
        administrador: true,
        vendedor: true
      }
    },
    {
      path: "/ConsultasVentas",
      name: "ConsultasVentas",
      component: ConsultasVenta,
      meta: {
        administrador: true,
        vendedor: true
      }
    },
    {
      path: "/Login",
      name: "Login",
      component: Login,
      meta: {
        libre: true
      }
    }
  ]
});

router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.libre)) {
    next();
  } else if (
    store.state.usuario &&
    store.state.usuario.rol == "Administrador"
  ) {
    if (to.matched.some(record => record.meta.administrador)) {
      next();
    }
  } else if (store.state.usuario && store.state.usuario.rol == "Almacenero") {
    if (to.matched.some(record => record.meta.almacenero)) {
      next();
    }
  } else if (store.state.usuario && store.state.usuario.rol == "Vendedor") {
    if (to.matched.some(record => record.meta.vendedor)) {
      next();
    }
  } else {
    next({
      name: "Login"
    });
  }
});

export default router;