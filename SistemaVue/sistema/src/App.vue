<template>
  <v-app id="inspire">
    <v-navigation-drawer
      v-model="drawer"
      :clipped="$vuetify.breakpoint.lgAndUp"
      fixed
      app
      v-if="logueado"
    >
      <v-list dense>
        <template v-if="esAdministrador || esAlmacenero || esVendedor">
          <v-list-tile :to="{name: 'home'}">
            <v-list-tile-action>
              <v-icon>home</v-icon>
            </v-list-tile-action>
            <v-list-tile-title>Inicio</v-list-tile-title>
          </v-list-tile>
        </template>
        <template v-if="esAdministrador || esAlmacenero">
          <v-list-group>
            <v-list-tile slot="activator">
              <v-list-tile-content>
                <v-list-tile-title>
                  Almacén
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: 'categoria'}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Categorias
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: 'articulo'}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Articulos
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>            
          </v-list-group>
        </template>
        <template v-if="esAdministrador || esAlmacenero">
          <v-list-group>
            <v-list-tile slot="activator">
              <v-list-tile-content>
                <v-list-tile-title>
                  Compras
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: 'Ingreso'}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Ingresos
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: 'Provedor'}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Proveedores
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>            
          </v-list-group>
        </template>
        <template v-if="esAdministrador || esVendedor">
          <v-list-group>
            <v-list-tile slot="activator">
              <v-list-tile-content>
                <v-list-tile-title>
                  Ventas
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: 'venta'}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Ventas
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: 'Cliente'}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Clientes
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>            
          </v-list-group>
        </template>
        <template v-if="esAdministrador">
          <v-list-group>
            <v-list-tile slot="activator">
              <v-list-tile-content>
                <v-list-tile-title>
                  Accesos
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: 'roles'}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Roles
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: 'Usuario'}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Usuarios
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>            
          </v-list-group>
        </template>
        <template v-if="esAdministrador">
          <v-list-group>
            <v-list-tile slot="activator">
              <v-list-tile-content>
                <v-list-tile-title>
                  Consultas
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: ''}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Consultas compras
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{name: 'ConsultasVentas'}">
              <v-list-tile-action>
                <v-icon>table_chart</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  Consultas Ventas
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>            
          </v-list-group>
        </template>                                
      </v-list>
    </v-navigation-drawer>
    <v-toolbar
      :clipped-left="$vuetify.breakpoint.lgAndUp"
      color="blue darken-3"
      dark
      app
      fixed
    >
      <v-toolbar-title style="width: 300px" class="ml-0 pl-3">
        <v-toolbar-side-icon @click.stop="drawer = !drawer"></v-toolbar-side-icon>
        <span class="hidden-sm-and-down">Sistema</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn  @click="salir" v-if="logueado" icon>
        <v-icon>logout</v-icon> Salir
      </v-btn>
      <v-btn :to="{name: 'Login'}" v-else>
        <v-icon>apps</v-icon> Login
      </v-btn>
    </v-toolbar>
    <v-content>
      <v-container fluid fill-height>
        <v-slide-y-transition mode="out-in">
          <router-view/>
        </v-slide-y-transition>
      </v-container>
    </v-content>
    <v-footer dark height="auto">
      <v-layout justify-center>
        <v-flex text-xs-center>
          <v-card flat tile color="primary" class="white--text">
            <v-card-text class="white--text pt-0">
              Sistema de prueba &copy; 2019
            </v-card-text>
          </v-card>
        </v-flex>
      </v-layout>
    </v-footer>
  </v-app>
</template>

<script>
import { log } from 'util';

export default {
  name: 'App',
  data () {
    return {
      drawer: null
    }
  },
  computed: {
    logueado(){
      return this.$store.state.usuario;
      console.log(this.$store.state.usuario);
      
    },
    esAdministrador(){
      return this.$store.state.usuario && this.$store.state.usuario.rol == 'Administrador';
      console.log('Administrador');
      
    },
    esAlmacenero(){
      return this.$store.state.usuario && this.$store.state.usuario.rol == 'Almacenero';
      console.log('Almacenero');
    },
    esVendedor(){
      return this.$store.state.usuario && this.$store.state.usuario.rol == 'Vendedor';
      console.log('Vendedor');
    }
  },
  created() {

    this.$store.dispatch("autoLogin");

  },
  methods: {
    salir(){
      this.$store.dispatch("salir");
    }
  }
}
</script>
