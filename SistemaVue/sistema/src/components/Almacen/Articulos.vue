<template>
  <v-layout align-start>
    <v-flex>
      <v-toolbar flat color="white">
        <v-btn @click="CrearPDF()">
          <v-icon>print</v-icon>
        </v-btn>
        <v-toolbar-title>Articulos</v-toolbar-title>
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
            <v-btn color="primary" dark class="mb-2" v-on="on">Nuevo Articulo</v-btn>
          </template>
          <v-card>
            <v-card-title>
              <span class="headline">{{ formTitle }}</span>
            </v-card-title>
            <v-card-text>
              <v-container grid-list-md>
                <v-layout wrap>
                  <v-flex xs4 sm4 md4>
                    <v-text-field v-model="codigo" label="Código"></v-text-field>
                  </v-flex>
                  <v-flex xs8 sm8 md8>
                    <v-select v-model="idcategoria" :items="categorias" label="Categoria"></v-select>
                  </v-flex>
                  <v-flex xs12 sm12 md12>
                    <v-text-field v-model="nombre" label="Nombre"></v-text-field>
                  </v-flex>
                  <v-flex xs6 sm6 md6>
                    <v-text-field v-model="stock" type="number" label="Stock"></v-text-field>
                  </v-flex>
                  <v-flex xs6 sm6 md6>
                    <v-text-field v-model="precio_venta" type="number" label="Precio Venta"></v-text-field>
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
              el articulo "{{AdNombre}}"
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
      <v-data-table :headers="headers" :items="articulos" :search="search" class="elevation-1">
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
          <td>{{ props.item.codigo }}</td>
          <td>{{ props.item.nombre }}</td>
          <td>{{ props.item.categoria }}</td>
          <td>{{ props.item.stock }}</td>
          <td>{{ props.item.precio_venta }}</td>
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
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import { domainToUnicode } from 'url';

export default {
  data() {
    return {
      valida: 0,
      validaMensaje: [],
      articulos: [],
      dialog: false,
      search: "",
      headers: [
        { text: "Opciones", value: "opciones", sortable: false },
        { text: "Código", value: "codigo", sortable: false },
        { text: "Nombre", value: "nombre" },
        { text: "Categoria", value: "categoria" },
        { text: "Stock", value: "stock", sortable: false },
        { text: "Precio Venta", value: "precio_venta", sortable: false },
        { text: "Descripción", value: "descripcion", sortable: false },
        { text: "Condición", value: "condicion", sortable: false }
      ],
      editedIndex: -1,
      id: "",
      idcategoria: "",
      categorias: [],
      codigo: "",
      nombre: "",
      stock: 0,
      precio_venta: 0,
      descripcion: "",
      adModal: 0,
      adAccion: 0,
      AdNombre: "",
      adId: ""
    };
  },
  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "Nuevo Articulo" : "Actualizar Articulo";
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
    CrearPDF() {
      var columns = [
        {title: "Nombre", dataKey: "nombre"},
        {title: "Códido", dataKey: "codigo"},
        {title: "Categoria", dataKey: "categoria"},
        {title: "Stock", dataKey: "stock"},
        {title: "Precio Venta", dataKey: "precio_venta"}
      ];
      var rows = [];
    
       this.articulos.map(function (x){
          rows.push({nombre:x.nombre,codigo:x.codigo,categoria:x.categoria,stock: x.stock,precio_venta:x.precio_venta});
       });
        var doc = new jsPDF('p', 'pt');
        doc.autoTable(columns, rows, {
          margin: {top: 60},
          addPageContent: function(data){
            doc.text("Listado Articulos", 40, 30);
          }
        });
        doc.save("Articulos.pdf");
    },
    Select() {
      let me = this;
      var categoriasArray = [];
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .get("api/Categorias/Select", configuracion)
        .then(resp => {
          categoriasArray = resp.data;
          categoriasArray.map(x => {
            me.categorias.push({ text: x.nombre, value: x.idcategoria });
          });
        })
        .catch(error => {
          console.log(error);
        });
    },
    Listar() {
      let me = this;
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .get("api/Articulos/Listar", configuracion)
        .then(resp => {
          me.articulos = resp.data;
        })
        .catch(error => {
          console.log(error);
        });
    },
    editItem(item) {
      this.editedIndex = 1;
      this.codigo = item.codigo;
      this.id = item.idarticulo;
      this.idcategoria = item.idcategoria;
      this.nombre = item.nombre;
      this.stock = item.stock;
      this.precio_venta = item.precio_venta;
      this.descripcion = item.descripcion;
      this.dialog = true;
    },
    close() {
      this.dialog = false;
      this.Limpiar();
    },
    Limpiar() {
      this.id = "";
      this.idcategoria = "";
      this.codigo = "";
      this.nombre = "";
      this.stock = 0;
      this.precio_venta = 0;
      this.descripcion = "";
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
          .put(
            "api/Articulos/Actualizar",
            {
              idarticulo: me.id,
              idcategoria: me.idcategoria,
              codigo: me.codigo,
              nombre: me.nombre,
              stock: me.stock,
              precio_venta: me.precio_venta,
              descripcion: me.descripcion
            },
            configuracion
          )
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
          .post(
            "api/Articulos/Crear",
            {
              codigo: me.codigo,
              nombre: me.nombre,
              idcategoria: me.idcategoria,
              stock: me.stock,
              precio_venta: me.precio_venta,
              descripcion: me.descripcion
            },
            configuracion
          )
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
      this.adId = item.idarticulo;
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
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .put("api/Articulos/Activar/" + this.adId, {}, configuracion)
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
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .put("api/Articulos/Desactivar/" + this.adId, {}, configuracion)
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
      if (!this.idcategoria) {
        this.validaMensaje.push("Debe elejir una categoria.");
      }
      if (!this.stock || this.stock == 0) {
        this.validaMensaje.push("Ingrese el stock inicial del articulo.");
      }
      if (!this.precio_venta || this.precio_venta == 0) {
        this.validaMensaje.push("Ingrese el precio de venta del articulo.");
      }
      if (this.validaMensaje.length) {
        this.valida = 1;
      }
      return this.valida;
    }
  }
};
</script>