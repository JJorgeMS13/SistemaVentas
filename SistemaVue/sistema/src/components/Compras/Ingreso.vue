<template>
  <v-layout align-start>
    <v-flex>
      <v-toolbar flat color="white">
        <v-toolbar-title>Ingresos</v-toolbar-title>
        <v-divider class="mx-2" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <v-text-field
          v-if="verNuevo==0"
          class="text-xs-center"
          v-model="search"
          append-icon="search"
          label="Busqueda"
          single-line
          hide-details
        ></v-text-field>
        <v-btn
          v-if="verNuevo == 0"
          @click="Listar"
          color="primary"
          dark
          class="mb-2"
        >Buscar</v-btn>        
        <v-spacer></v-spacer>
        <v-btn
          v-if="verNuevo == 0"
          @click="MostrarNuevo"
          color="primary"
          dark
          class="mb-2"
        >Nuevo Ingreso</v-btn>
        <v-dialog v-model="verArticulo" max-width="1000px">
          <v-card>
            <v-card-title>
              <span class="headline">Seleccione un Artículo</span>
            </v-card-title>
            <v-card-text>
              <v-container grid-list-md>
                <v-layout wrap>
                  <v-flex xs12 sm12 md12 lg12 xl12>
                    <v-text-field
                      append-icon="search"
                      class="text-xs-center"
                      v-model="texto"
                      label="Ingrese artículo a buscar"
                      @keyup.enter="ListarArticulo()"
                    ></v-text-field>
                    <template>
                      <v-data-table
                        :headers="cabeceraArticulos"
                        :items="articulos"
                        class="elevation-1"
                      >
                        <template v-slot:items="props">
                          <td class="justify-center layout px-0">
                            <v-icon
                              small
                              class="mr-2"
                              color="blue darken-1"
                              @click="agregarDetalle(props.item)"
                            >add</v-icon>
                          </td>
                          <td>{{ props.item.nombre }}</td>
                          <td>{{props.item.categoria}}</td>
                          <td>{{props.item.descripcion}}</td>
                          <td>{{props.item.stock}}</td>
                          <td>{{props.item.precio_venta}}</td>
                        </template>
                        <template v-slot:no-data>
                          <h3>No hay articulo para mostrar</h3>
                        </template>
                      </v-data-table>
                    </template>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn @click="ocultarArticulos()" color="blue darken" flat>Cancelar</v-btn>
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
              <span v-if="adAccion==2">Anular</span>
              el usuario "{{AdNombre}}"
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn v-if="adAccion==1" @click="Activar" color="green darken-1" flat>Activar</v-btn>
              <v-btn v-if="adAccion==2" @click="Desactivar" color="green darken-1" flat>Anular</v-btn>
              <v-btn color="orange darken-1" flat @click="activarDesactivarCerrar">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-toolbar>
      <v-data-table
        :headers="headers"
        :items="ingresos"
        :search="search"
        class="elevation-1"
        v-if="verNuevo == 0"
      >
        <template v-slot:items="props">
          <td class="justify-center layout px-0">
            <v-icon small class="mr-2" color="blue darken-1" @click="verDetalles(props.item)">tab</v-icon>
            <template v-if="props.item.estado== 'Aceptado'">
              <v-icon
                small
                color="red darken-1"
                @click="activarDesactivarMostrar(2,props.item)"
              >block</v-icon>
            </template>
          </td>
          <td>{{ props.item.usuario }}</td>
          <td>{{ props.item.proveedor }}</td>
          <td>{{ props.item.tipo_comprobante }}</td>
          <td>{{ props.item.serie_comprobante }}</td>
          <td>{{ props.item.num_comprobante }}</td>
          <td>{{ props.item.fecha_hora }}</td>
          <td>{{ props.item.impuesto }}</td>
          <td>{{ props.item.total }}</td>
          <td>
            <div v-if=" props.item.estado== 'Aceptado'">
              <span class="blue--text">Aceptado</span>
            </div>
            <div v-else>
              <span class="red--text">{{props.item.estado}}</span>
            </div>
          </td>
        </template>
        <template v-slot:no-data>
          <v-btn color="primary" @click="Listar">Resetear</v-btn>
        </template>
      </v-data-table>
      <v-container grid-list-sm class="pa-4 white" v-if="verNuevo == 1">
        <v-layout row wrap>
          <v-flex xs12 sm4 md4 lg4 xl4>
            <v-select v-model="tipo_comprobante" :items="comprobantes" label="Tipo Comprobante"></v-select>
          </v-flex>
          <v-flex xs12 sm4 md4 lg4 xl4>
            <v-text-field v-model="serie_comprobante" label="Serie Comprobante"></v-text-field>
          </v-flex>
          <v-flex xs12 sm4 md4 lg4 xl4>
            <v-text-field v-model="num_comprobante" label="Número Comprobante"></v-text-field>
          </v-flex>
          <v-flex xs12 sm8 md8 lg8 xl8>
            <v-select v-model="idproveedor" :items="proveedores" label="Proveedor"></v-select>
          </v-flex>
          <v-flex xs12 sm4 md4 lg4 xl4>
            <v-text-field type="number" v-model="impuesto" label="Impuesto"></v-text-field>
          </v-flex>
          <v-flex xs8 sm8 md8 lg8 xl8>
            <v-text-field @keyup.enter="buscarCodigo()" v-model="codigo" label="Codigo"></v-text-field>
          </v-flex>
          <v-flex xs4 sm4 md4 lg4 xl4>
            <v-btn @click="mostrarArticulos()" small fab dark color="teal">
              <v-icon dark>list</v-icon>
            </v-btn>
          </v-flex>
          <v-flex xs4 sm4 md4 lg4 xl4 v-if="errorArticulo">
            <div class="red--text" v-text="errorArticulo"></div>
          </v-flex>
          <v-flex xs12 sm12 md12 lg12 xl12>
            <v-data-table
              :headers="cabeceraDetalles"
              :items="detalles"
              hide-actions
              class="elevation-1"
            >
              <template v-slot:items="props">
                <td class="justify-center layout px-0">
                  <v-icon
                    small
                    class="mr-2"
                    color="blue darken-1"
                    @click="eliminarDetalle(detalles,props.item)"
                  >delete</v-icon>
                </td>
                <td>{{ props.item.articulo }}</td>
                <td>
                  <v-text-field type="number" v-model="props.item.cantidad"></v-text-field>
                </td>
                <td>
                  <v-text-field type="number" v-model="props.item.precio"></v-text-field>
                </td>
                <td>${{ props.item.cantidad * props.item.precio }}</td>
              </template>
              <template v-slot:no-data>
                <h3>No hay articulo agregados al detalle</h3>
              </template>
            </v-data-table>
            <v-flex class="text-xs-right">
              <strong>Total Parcial:</strong>
              $ {{totalParcial =(total- totalImpuesto).toFixed(2)}}
            </v-flex>
            <v-flex class="text-xs-right">
              <strong>Total Impuesto:</strong>
              $ {{totalImpuesto = ((total * impuesto)/(100+impuesto)).toFixed(2)}}
            </v-flex>
            <v-flex class="text-xs-right">
              <strong>Total Neto:</strong>
              $ {{total = (calcularTotal).toFixed(2)}}
            </v-flex>
          </v-flex>
          <v-flex xs12 sm12 md12 lg12 xl12>
            <div class="red--text" v-for="v in validaMensaje" :key="v" v-text="v"></div>
          </v-flex>
          <v-flex xs12 sm12 md12 lg12 xl12>
            <v-btn @click="OcultarNuevo" color="blue darken-1" flat>Cancelar</v-btn>
            <v-btn v-if="verDet==0" @click="guardar()" color="success">Guardar</v-btn>
          </v-flex>
        </v-layout>
      </v-container>
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
      ingresos: [],
      dialog: false,
      search: "",
      headers: [
        { text: "Opciones", value: "opciones", sortable: false },
        { text: "Usuario", value: "usuario" },
        { text: "Proveedor", value: "proveedor" },
        { text: "Tipo Comprobante", value: "tipo_comprobante" },
        {
          text: "Serie Comprobante",
          value: "serie_comprobante",
          sortable: false
        },
        {
          text: "Número Comprobante",
          value: "num_comprobante",
          sortable: false
        },
        { text: "Fecha", value: "fecha_hora", sortable: false },
        { text: "Impuestos", value: "impuestos", sortable: false },
        { text: "Total", value: "total", sortable: false },
        { text: "Estado", value: "estado", sortable: false }
      ],
      cabeceraDetalles: [
        { text: "Borrar", value: "borrar", sortable: false },
        { text: "Artículo", value: "articulo", sortable: false },
        { text: "Cantidad", value: "cantidad", sortable: false },
        { text: "Precio", value: "precio", sortable: false },
        { text: "Subtotal", value: "subtotal", sortable: false }
      ],
      detalles: [],
      id: "",
      idproveedor: "",
      proveedores: [],
      tipo_comprobante: "",
      comprobantes: ["FACTURA", "BOLETA", "TICKET", "GUIA"],
      serie_comprobante: "",
      num_comprobante: "",
      impuesto: 16,
      codigo: "",
      verNuevo: 0,
      errorArticulo: null,
      totalParcial: 0,
      totalImpuesto: 0,
      total: 0,
      cabeceraArticulos: [
        { text: "Seleccionar", value: "seleccionar", sortable: false },
        { text: "Artículo", value: "articulo" },
        { text: "Categoría", value: "categoria" },
        { text: "Descripcion", value: "descripcion", sortable: false },
        { text: "Stock", value: "stock", sortable: false },
        { text: "Precio Venta", value: "precio_venta", sortable: false }
      ],
      articulos: [],
      texto: "",
      verArticulo: 0,
      verDet: 0,
      adModal: 0,
      adAccion: 0,
      AdNombre: "",
      adId: ""
    };
  },
  computed: {
    calcularTotal: function() {
      var resultado = 0.0;
      for (var i = 0; i < this.detalles.length; i++) {
        resultado =
          resultado + this.detalles[i].precio * this.detalles[i].cantidad;
      }
      return resultado;
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
      var proveedoresArray = [];
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .get("api/Personas/SelectProveedores", configuracion)
        .then(resp => {
          proveedoresArray = resp.data;
          proveedoresArray.map(x => {
            me.proveedores.push({ text: x.nombre, value: x.idpersona });
          });
        })
        .catch(error => {
          console.log(error);
        });
    },
    MostrarNuevo() {
      this.verNuevo = 1;
    },
    OcultarNuevo() {
      this.verNuevo = 0;
      this.Limpiar();
    },
    buscarCodigo() {
      let me = this;
      me.errorArticulo = null;
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .get("api/Articulos/BuscarCodigoIngreso/" + this.codigo, configuracion)
        .then(resp => {
          this.agregarDetalle(resp.data);
        })
        .catch(error => {
          console.log(error);
          me.errorArticulo = "No existe el articulo";
        });
    },
    ListarArticulo() {
      let me = this;
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .get("api/Articulos/ListarIngreso/" + me.texto, configuracion)
        .then(resp => {
          me.articulos = resp.data;
        })
        .catch(error => {
          console.log(error);
        });
    },
    mostrarArticulos() {
      this.verArticulo = 1;
    },
    ocultarArticulos() {
      this.verArticulo = 0;
    },
    agregarDetalle(data = []) {
      this.errorArticulo = null;
      console.log(this.encuentra(data["idarticulo"]));

      if (this.encuentra(data["idarticulo"])) {
        this.errorArticulo = "El articulo ya ha sido agregado.";
      } else {
        this.detalles.push({
          idarticulo: data["idarticulo"],
          articulo: data["nombre"],
          cantidad: 1,
          precio: 1
        });
      }
      this.verArticulo = 0;
      this.texto = "";
      this.articulos = [];
    },
    encuentra(id) {
      var sw = 0;
      for (var i = 0; i < this.detalles.length; i++) {
        if (this.detalles[i].idarticulo == id) {
          sw = 1;
        }
      }
      return sw;
    },
    eliminarDetalle(arr, item) {
      var i = arr.indexOf(item);
      if (i !== -1) {
        arr.splice(i, 1);
      }
    },
    Listar() {
      let me = this;
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      let url = '';
      if(!me.search){
        url ='api/Ingresos/Listar';
      }else{
        url = 'api/Ingresos/ListarFiltro'+me.search;
      }
      axios
        .get(url, configuracion)
        .then(resp => {
          me.ingresos = resp.data;
        })
        .catch(error => {
          console.log(error);
        });
    },
    ListarDetalles(id) {
      let me = this;
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .get("api/Ingresos/ListarDetalles/"+id, configuracion)
        .then(resp => {
          me.detalles = resp.data;
        })
        .catch(error => {
          console.log(error);
        });
    },
    verDetalles(item){
      this.Limpiar();
      this.tipo_comprobante = item.tipo_comprobante;
      this.serie_comprobante = item.serie_comprobante;
      this.num_comprobante = item.num_comprobante;
      this.idproveedor = item.idproveedor;
      this.impuesto = item.impuesto;
      this.ListarDetalles(item.idingreso);
      this.verNuevo=1;
      this.verDet=1;
    },
    Limpiar() {
      this.id = "";
      this.idproveedor = "";
      this.tipo_comprobante = "";
      this.serie_comprobante = "";
      this.num_comprobante = "false";
      this.impuesto = 18;
      this.codigo = "";
      this.detalles = [];
      this.total = 0;
      this.totalImpuesto = 0;
      this.totalParcial = 0;
      this.verDet=0;
    },
    guardar() {
      let me = this;
      if (this.validar()) {
        return;
      }
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      if (me.password != me.passwordAnt) {
        me.act_password = true;
      }
      //Guardar datos
      axios
        .post(
          "api/Ingresos/Crear",
          {
            'idproveedor': me.idproveedor,
            'idusuario': me.$store.state.usuario.idusuario,
            'tipo_comprobante': me.tipo_comprobante,
            'serie_comprobante': me.serie_comprobante,
            'num_comprobante': me.num_comprobante,
            'impuesto': me.impuesto,
            'total': me.total,
            'detalles': me.detalles
          },
          configuracion
        )
        .then(resp => {
          me.OcultarNuevo();
          me.Listar();
          me.Limpiar();
        })
        .catch(error => {
          console.log(error);
        });

      //this.close();
    },
    activarDesactivarMostrar(accion, item) {
      this.adModal = 1;
      this.AdNombre = item.num_comprobante;
      this.adId = item.idingreso;
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
    /*
    Activar() {
      let me = this;
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .put("api/Usuarios/Activar/" + this.adId, {}, configuracion)
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
    },*/
    Desactivar() {
      let me = this;
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .put("api/Ingresos/Anular/" + this.adId, {}, configuracion)
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
      if (!this.idproveedor) {
        this.validaMensaje.push("Seleccione un proveedor.");
      }
      if (!this.tipo_comprobante) {
        this.validaMensaje.push("Seleccione un tipo de comprobante.");
      }
      if (!this.num_comprobante) {
        this.validaMensaje.push("Ingrese el número del comprobante.");
      }
      if (!this.impuesto || this.impuesto < 0) {
        this.validaMensaje.push("Debe ingresar un impuesto valido.");
      }
      if (this.detalles.length <= 0) {
        this.validaMensaje.push("Ingrese al menos un articulo al detalle.");
      }
      if (this.validaMensaje.length) {
        this.valida = 1;
      }
      return this.valida;
    }
  }
};
</script>