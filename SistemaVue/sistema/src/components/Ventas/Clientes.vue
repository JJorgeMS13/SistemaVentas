<template>
  <v-layout align-start>
    <v-flex>
      <v-toolbar flat color="white">
        <v-toolbar-title>Clientes</v-toolbar-title>
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
            <v-btn color="primary" dark class="mb-2" v-on="on">Nuevo Cliente</v-btn>
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
                    <v-select v-model="tipo_documento" :items="documentos" label="Tipo Documento"></v-select>
                  </v-flex>
                  <v-flex xs12 sm6 md6>
                    <v-text-field v-model="num_documento" label="Numero Documento"></v-text-field>
                  </v-flex>
                  <v-flex xs6 sm6 md6>
                    <v-text-field v-model="direccion" label="Dirección"></v-text-field>
                  </v-flex>
                  <v-flex xs6 sm6 md6>
                    <v-text-field v-model="telefono" label="Telefono"></v-text-field>
                  </v-flex>
                  <v-flex xs12 sm6 md6>
                    <v-text-field v-model="email" label="Email"></v-text-field>
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
      </v-toolbar>
      <v-data-table :headers="headers" :items="clientes" :search="search" class="elevation-1">
        <template v-slot:items="props">
          <td class="justify-center layout px-0">
            <v-icon small class="mr-2" color="blue darken-1" @click="editItem(props.item)">edit</v-icon>
          </td>
          <td>{{ props.item.nombre }}</td>
          <td>{{ props.item.tipo_documento }}</td>
          <td>{{ props.item.num_documento }}</td>
          <td>{{ props.item.direccion }}</td>
          <td>{{ props.item.telefono }}</td>
          <td>{{ props.item.email }}</td>
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
      clientes: [],
      dialog: false,
      search: "",
      headers: [
        { text: "Opciones", value: "opciones", sortable: false },
        { text: "Nombre", value: "nombre" },
        { text: "Tipo Documento", value: "tipo_documento" },
        { text: "Numero Documento", value: "num_documento", sortable: false },
        { text: "Dirección", value: "direccion", sortable: false },
        { text: "Telefono", value: "telefono", sortable: false },
        { text: "Email", value: "email", sortable: false }
      ],
      editedIndex: -1,
      idpersona: "",
      tipo_persona: "",
      documentos: ["DNI", "RUC", "PASAPORTE", "CEDULA"],
      nombre: "",
      tipo_documento: "",
      num_documento: "",
      direccion: "",
      telefono: "",
      email: ""
    };
  },
  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nuevo Cliente" : "Actualizar Cliente";
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
        .get("api/Personas/ListarClientes",configuracion)
        .then(resp => {
          me.clientes = resp.data;
        })
        .catch(error => {
          console.log(error);
        });
    },
    editItem(item) {
      this.editedIndex = 1;
      this.idpersona = item.idpersona;
      this.nombre = item.nombre;
      this.tipo_documento = item.tipo_documento;
      this.num_documento = item.num_documento;
      this.direccion = item.direccion;
      this.telefono = item.telefono;
      this.email = item.email;
      this.dialog = true;
    },
    close() {
      this.dialog = false;
      this.Limpiar();
    },
    Limpiar() {
      this.idpersona = "";
      this.nombre = "";
      this.tipo_documento = "";
      this.num_documento = "";
      this.direccion = "";
      this.telefono = "";
      this.email = "";
      this.editedIndex = -1;
      this.validaMensaje = [];
    },
    guardar() {
      let me = this;
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      if (this.validar()) {
        return;
      }
      if (this.editedIndex > -1) {
        //Actualizar datos
        axios
          .put("api/Personas/Actualizar", {
            idpersona: me.idpersona,
            tipo_persona: "Cliente",
            nombre: me.nombre,
            tipo_documento: me.tipo_documento,
            num_documento: me.num_documento,
            direccion: me.direccion,
            telefono: me.telefono,
            email: me.email
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
          .post("api/Personas/Crear", {
            tipo_persona: "Cliente",
            nombre: me.nombre,
            tipo_documento: me.tipo_documento,
            num_documento: me.num_documento,
            direccion: me.direccion,
            telefono: me.telefono,
            email: me.email
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
    validar() {
      this.valida = 0;
      this.validaMensaje = [];
      if (this.nombre.length < 3 || this.nombre > 100) {
        this.validaMensaje.push(
          "El nombre debe de tener mas 3 caracteres y menos de 100 caracteres."
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