<template>
  <v-layout align-start>
    <v-flex>
      <v-toolbar flat color="white">
        <v-toolbar-title>Usuarios</v-toolbar-title>
        <v-divider class="mx-2" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-text-field
          class="text-xs-center"
          v-model="search"
          append-icon="search"
          label="Busqueda"
          single-line
          hide-details
        ></v-text-field>
        <v-spacer></v-spacer>
        <v-dialog v-model="dialog" max-width="500px">
          <template v-slot:activator="{ on }">
            <v-btn color="primary" dark class="mb-2" v-on="on">Nuevo Usuario</v-btn>
          </template>
          <v-card>
            <v-card-title>
              <span class="headline">{{ formTitle }}</span>
            </v-card-title>
            <v-card-text>
              <v-container grid-list-md>
                <v-layout wrap>
                  <v-flex xs12 sm6 md6>
                    <v-text-field v-model="nombre" label="Nombre"></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md6>
                    <v-select v-model="idrol" :items="Roles" label="Roles"></v-select>
                  </v-flex>
                  <v-flex xs12 sm6 md6>
                    <v-select v-model="tipo_documento" :items="documentos" label="Tipo Documento"></v-select>
                  </v-flex>
                  <v-flex xs12 sm6 md6>
                    <v-text-field v-model="num_documento" label="Número Documento"></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md6>
                    <v-text-field v-model="direccion" label="Dirección"></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md6>
                    <v-text-field v-model="telefono" label="Telefono"></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md6>
                    <v-text-field v-model="email" label="Email"></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md6>
                    <v-text-field v-model="password" type="password" label="Contraseña"></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm12 md12 v-show="valida==1">
                    <div class="red--text" v-for="v in validaMensaje" :key="v" v-text="v"></div>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" flat @click="guardar">Guardar</v-btn>
              <v-btn color="orange darken-1" flat @click="close">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <v-dialog v-model="adModal" max-width="290">
          <v-card>
            <v-card-title class="headline" v-if="adAccion==1">
              <v-icon color="orange darken-1">warning</v-icon>
              <span>Cuidado!</span>
            </v-card-title>
            <v-card-title class="headline" v-if="adAccion==2">
              <v-icon color="orange darken-1">warning</v-icon>
              <span class="text-xs-center">Cuidado!</span>
            </v-card-title>
            <v-card-text>
              Estás a punto de
              <span v-if="adAccion==1">Activar</span>
              <span v-if="adAccion==2">Desactivar</span>
              el usuario "{{AdNombre}}"
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn v-if="adAccion==1" @click="Activar" color="green darken-1" flat>Activar</v-btn>
              <v-btn v-if="adAccion==2" @click="Desactivar" color="green darken-1" flat>Desactivar</v-btn>
              <v-btn color="orange darken-1" flat @click="activarDesactivarCerrar">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-toolbar>
      <v-data-table :headers="headers" :items="usuarios" :search="search" class="elevation-1">
        <template v-slot:items="props">
          <td class="justify-center layout px-0">
            <v-icon small class="mr-2" color="blue darken-1" @click="editItem(props.item)">edit</v-icon>
            <template v-if="props.item.condicion">
              <v-icon
                small
                color="red darken-1"
                @click="activarDesactivarMostrar(2,props.item)"
              >block</v-icon>
            </template>
            <template v-else>
              <v-icon
                small
                color="blue darken-1"
                @click="activarDesactivarMostrar(1,props.item)"
              >check</v-icon>
            </template>
          </td>
          <td>{{ props.item.nombre }}</td>
          <td>{{ props.item.rol }}</td>
          <td>{{ props.item.tipo_documento }}</td>
          <td>{{ props.item.direccion }}</td>
          <td>{{ props.item.telefono }}</td>
          <td>{{ props.item.email }}</td>
          <td>
            <div v-if=" props.item.condicion">
              <span class="blue--text">Activo</span>
            </div>
            <div v-else>
              <span class="red--text">Inactivo</span>
            </div>
          </td>
        </template>
        <template v-slot:no-data>
          <v-btn color="primary" @click="Listar">Resetear</v-btn>
        </template>
      </v-data-table>
    </v-flex>
  </v-layout>
</template>

<script>
import axios from "axios";
import { log } from "util";

export default {
  data() {
    return {
      valida: 0,
      validaMensaje: [],
      usuarios: [],
      dialog: false,
      search: "",
      headers: [
        { text: "Opciones", value: "opciones", sortable: false },
        { text: "Nombre", value: "nombre" },
        { text: "Rol", value: "rol" },
        { text: "Tipo Documento", value: "tipo_documento", sortable: false },
        { text: "Direccion", value: "direccion", sortable: false },
        { text: "Telefono", value: "telefono", sortable: false },
        { text: "Email", value: "email", sortable: false },
        { text: "Estado", value: "condicion", sortable: false }
      ],
      editedIndex: -1,
      id: "",
      idrol: "",
      Roles: [],
      nombre: "",
      tipo_documento: "",
      documentos: ["DNI", "RUC", "PASAPORTE", "CEDULA"],
      num_documento: "",
      direccion: "",
      telefono: "",
      email: "",
      password: "",
      act_password: false,
      passwordAnt: "",
      adModal: 0,
      adAccion: 0,
      AdNombre: "",
      adId: ""
    };
  },
  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nuevo Usuario" : "Actualizar Usuario";
    }
  },
  watch: {
    dialog(val) {
      val || this.close();
    }
  },
  created() {
    this.Listar();
    this.Select();
  },
  methods: {
    Select() {
      let me = this;
      var rolesArray = [];
      let headers ={"Authorization" : "Bearer " + this.$store.state.token};
      let configuracion = {headers : headers}
      axios
        .get("api/Roles/Select",configuracion)
        .then(resp => {
          rolesArray = resp.data;
          rolesArray.map(x => {
            me.Roles.push({ text: x.nombre, value: x.idrol });
          });
        })
        .catch(error => {
          console.log(error);
        });
    },
    Listar() {
      let me = this;
      let headers ={"Authorization" : "Bearer " + this.$store.state.token};
      let configuracion = {headers : headers}      
      axios
        .get("api/Usuarios/Listar",configuracion)
        .then(resp => {
          me.usuarios = resp.data;
        })
        .catch(error => {
          console.log(error);
        });
    },
    editItem(item) {
      this.editedIndex = 1;
      this.id = item.idusuario;
      this.idrol = item.idrol;
      this.nombre = item.nombre;
      this.tipo_documento = item.tipo_documento;
      this.num_documento = item.num_documento;
      this.direccion = item.direccion;
      this.telefono = item.telefono;
      this.email = item.email;
      this.password = item.password_hash;
      this.passwordAnt = item.password_hash;
      this.dialog = true;
    },
    close() {
      this.dialog = false;
      this.Limpiar();
    },
    Limpiar() {
      this.id = "";
      this.idrol = "";
      this.nombre = "";
      this.passwordAnt = "";
      this.act_password = false;
      this.tipo_documento = 0;
      this.num_documento = 0;
      this.direccion = "";
      this.telefono = "";
      this.email = "";
      this.password = "";
      this.editedIndex = -1;
      this.validaMensaje = [];
    },
    guardar() {
      let me = this;
      if (this.validar()) {
        return;
      }
      let headers ={"Authorization" : "Bearer " + this.$store.state.token};
      let configuracion = {headers : headers}      
      if (me.password != me.passwordAnt) {
        me.act_password = true;
      }
      if (this.editedIndex > -1) {
        //Actualizar datos
        axios
          .put("api/Usuarios/Actualizar", {
            idusuario: me.id,
            idrol: me.idrol,
            nombre: me.nombre,
            tipo_documento: me.tipo_documento,
            num_documento: me.num_documento,
            direccion: me.direccion,
            telefono: me.telefono,
            email: me.email,
            password: me.password,
            act_password: me.act_password
          },configuracion)
          .then(resp => {
            me.close();
            me.Listar();
          })
          .catch(error => {
            console.log(error);
          });
      } else {
        //Guardar datos
        axios
          .post("api/Usuarios/Crear", {
            idrol: me.idrol,
            nombre: me.nombre,
            tipo_documento: me.tipo_documento,
            num_documento: me.num_documento,
            direccion: me.direccion,
            telefono: me.telefono,
            email: me.email,
            password: me.password
          },configuracion)
          .then(resp => {
            me.close();
            me.Listar();
          })
          .catch(error => {
            console.log(error);
          });
      }
      this.close();
    },
    activarDesactivarMostrar(accion, item) {
      this.adModal = 1;
      this.AdNombre = item.nombre;
      this.adId = item.idusuario;
      if (accion === 1) {
        this.adAccion = 1;
      } else if (accion === 2) {
        this.adAccion = 2;
      } else {
        this.adModal = 0;
      }
    },
    activarDesactivarCerrar() {
      this.adModal = 0;
    },
    Activar() {
      let me = this;
      let headers ={"Authorization" : "Bearer " + this.$store.state.token};
      let configuracion = {headers : headers}      
      axios
        .put("api/Usuarios/Activar/" + this.adId, {},configuracion)
        .then(resp => {
          me.adModal = 0;
          me.adAccion = 0;
          me.AdNombre = "";
          me.adId = "";
          me.Listar();
        })
        .catch(error => {
          console.log(error);
        });
    },
    Desactivar() {
      let me = this;
      let headers ={"Authorization" : "Bearer " + this.$store.state.token};
      let configuracion = {headers : headers}      
      axios
        .put("api/Usuarios/Desactivar/" + this.adId, {},configuracion)
        .then(resp => {
          me.adModal = 0;
          me.adAccion = 0;
          me.AdNombre = "";
          me.adId = "";
          me.Listar();
        })
        .catch(error => {
          console.log(error);
        });
    },
    validar() {
      this.valida = 0;
      this.validaMensaje = [];
      if (this.nombre.length < 3 || this.nombre > 100) {
        this.validaMensaje.push(
          "El nombre debe de tener mas 3 caracteres y menos de 100 caracteres."
        );
      }
      if (!this.idrol) {
        this.validaMensaje.push("Debe elejir un Rol.");
      }
      if (!this.tipo_documento) {
        this.validaMensaje.push("Debe elejir un tipo de documento.");
      }
      if (!this.email) {
        this.validaMensaje.push("Debe ingresar un Email.");
      }
      if (!this.password) {
        this.validaMensaje.push("Debe ingresar una contraseña.");
      }
      if (this.validaMensaje.length) {
        this.valida = 1;
      }
      return this.valida;
    }
  }
};
</script>