<template>
  <v-layout align-start>
    <v-flex>
      <v-toolbar flat color="white">
        <v-toolbar-title>Roles</v-toolbar-title>
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
      </v-toolbar>
      <v-data-table :headers="headers" :items="roles" :search="search" class="elevation-1">
        <template v-slot:items="props">
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
      roles: [],
      dialog: false,
      search: "",
      headers: [
        { text: "Nombre", value: "nombre" },
        { text: "Descripción", value: "descripcion", sortable: false },
        { text: "Condición", value: "condicion", sortable: false }
      ]
    };
  },
  computed: {},
  watch: {},
  created() {
    this.Listar();
  },
  methods: {
    Listar() {
      let me = this;
      let headers ={"Authorization" : "Bearer " + this.$store.state.token};
      let configuracion = {headers : headers}
      axios
        .get("api/Roles/Listar",configuracion)
        .then(resp => {
          me.roles = resp.data;
        })
        .catch(error => {
          console.log(error);
        });
    }
  }
};
</script>