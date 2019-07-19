<template>
  <v-layout align-start>
    <v-flex>
      <v-toolbar flat color="white">
        <v-toolbar-title>Categorias</v-toolbar-title>
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
            <v-btn color="primary" dark class="mb-2" v-on="on">Nueva Categoria</v-btn>
          </template>
          <v-card>
            <v-card-title>
              <span class="headline">{{ formTitle }}</span>
            </v-card-title>
            <v-card-text>
              <v-container grid-list-md>
                <v-layout wrap>
                  <v-flex xs12 sm12 md12>
                    <v-text-field v-model="nombre" label="Nombre"></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm12 md12>
                    <v-text-field v-model="descripcion" label="Descripción"></v-text-field>
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
              la categoria "{{AdNombre}}"
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
      <v-data-table :headers="headers" :items="categorias" :search="search" class="elevation-1">
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
          <td>{{ props.item.descripcion }}</td>
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
      categorias: [],
      dialog: false,
      search: "",
      headers: [
        { text: "Opciones", value: "opciones", sortable: false },
        { text: "Nombre", value: "nombre" },
        { text: "Descripción", value: "descripcion", sortable: false },
        { text: "Condición", value: "condicion", sortable: false }
      ],
      editedIndex: -1,
      id: "",
      nombre: "",
      descripcion: "",
      adModal: 0,
      adAccion: 0,
      AdNombre: "",
      adId: ""
    };
  },
  computed: {
    formTitle() {
      return this.editedIndex === -1
        ? "Nueva Categoria"
        : "Actualizar Categoria";
    }
  },
  watch: {
    dialog(val) {
      val || this.close();
    }
  },
  created() {
    this.Listar();
  },
  methods: {
    Listar() {
      let me = this;
      let headers ={"Authorization" : "Bearer " + this.$store.state.token};
      let configuracion = {headers : headers}
      axios
        .get("api/Categorias/Listar",configuracion)
        .then(resp => {
          me.categorias = resp.data;
        })
        .catch(error => {
          console.log(error);
        });
    },
    editItem(item) {
      this.editedIndex = 1;
      this.id = item.idcategoria;
      this.nombre = item.nombre;
      this.descripcion = item.descripcion;
      this.dialog = true;
    },
    deleteItem(item) {
      const index = this.desserts.indexOf(item);
      confirm("Are you sure you want to delete this item?") &&
        this.desserts.splice(index, 1);
    },
    close() {
      this.dialog = false;
      this.Limpiar();
    },
    Limpiar() {
      this.id = "";
      this.nombre = "";
      this.descripcion = "";
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
      if (this.editedIndex > -1) {
        //Actualizar datos
        axios
          .put("api/Categorias/Actualizar", {
            idcategoria: me.id,
            nombre: me.nombre,
            descripcion: me.descripcion
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
          .post("api/Categorias/Crear", {
            nombre: me.nombre,
            descripcion: me.descripcion
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
      this.adId = item.idcategoria;
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
        .put("api/Categorias/Activar/" + this.adId, {},configuracion)
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
        .put("api/Categorias/Desactivar/" + this.adId, {}, configuracion)
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
      if (this.nombre.length < 3 || this.nombre > 50) {
        this.validaMensaje.push(
          "El nombre debe de tener mas 3 caracteres y menos de 50 caracteres."
        );
      }
      if (this.validaMensaje.length) {
        this.valida = 1;
      }
      return this.valida;
    }
  }
};
</script>