<template>
  <v-layout align-start>
    <v-flex>
      <v-toolbar flat color="white">
        <v-toolbar-title>Ventas</v-toolbar-title>
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
        <v-btn v-if="verNuevo == 0" @click="Listar" color="primary" dark class="mb-2">Buscar</v-btn>
        <v-spacer></v-spacer>
        <v-btn
          v-if="verNuevo == 0"
          @click="MostrarNuevo"
          color="primary"
          dark
          class="mb-2"
        >Nueva Venta</v-btn>
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
              la venta "{{AdNombre}}"
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn v-if="adAccion==1" @click="Activar" color="green darken-1" flat>Activar</v-btn>
              <v-btn v-if="adAccion==2" @click="Desactivar" color="green darken-1" flat>Anular</v-btn>
              <v-btn color="orange darken-1" flat @click="activarDesactivarCerrar">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <v-dialog v-model="comprobanteModal" max-width="1000px">
          <v-card>
            <v-card-text>
              <v-btn @click="crearPDF()">
                <v-icon>print</v-icon>
              </v-btn>
              <div id="factura">
                <header>
                  <div id="logo">
                    <img src="data:image/png;base64,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" id="imagen" />
                  </div>
                  <div id="datos">
                    <p id="encabezado">
                      <b>IncanatoIT</b>
                      <br />José Gálvez 1368, Chongoyape - Chiclayo, Perú
                      <br />Telefono:(+51)931742904
                      <br />Email:jcarlos.ad7@gmail.com
                    </p>
                  </div>
                  <div id="fact">
                    <p>
                      {{tipo_comprobante}}
                      <br />{{serie_comprobante}}-{{num_comprobante}}
                      <br />{{fecha_hora}}
                    </p>
                  </div>
                </header>
                <br />
                <section>
                  <div>
                    <table id="facliente">
                      <tbody>
                        <tr>
                          <td id="cliente">
                            <strong>Sr(a). {{cliente}}</strong>
                            <br />
                            <strong>Documento:</strong> {{num_documento}}
                            <br />
                            <strong>Dirección:</strong> {{direccion}}
                            <br />
                            <strong>Teléfono:</strong> {{telefono}}
                            <br />
                            <strong>Email:</strong> {{email}}
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </section>
                <br />
                <section>
                  <div>
                    <table id="facarticulo">
                      <thead>
                        <tr id="fa">
                          <th>CANTIDAD</th>
                          <th>DESCRIPCIÓN</th>
                          <th>PRECIO UNIT</th>
                          <th>DESC.</th>
                          <th>PRECIO TOTAL</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr v-for="det in detalles" :key="det.iddetalle_venta">
                          <td style="text-align:center;">{{det.cantidad}}</td>
                          <td>{{det.articulo}}</td>
                          <td style="text-align:right;">{{det.precio.toFixed(2)}}</td>
                          <td style="text-align:right;">{{det.descuento}}</td>
                          <td style="text-align:right;">{{(det.cantidad * det.precio - det.descuento).toFixed(2)}}</td>
                        </tr>
                      </tbody>
                      <tfoot>
                        <tr>
                          <th></th>
                          <th></th>
                          <th></th>
                          <th style="text-align:right;">SUBTOTAL</th>
                          <th style="text-align:right;">${{totalParcial =(total -totalImpuesto).toFixed(2)}}</th>
                        </tr>
                        <tr>
                          <th></th>
                          <th></th>
                          <th></th>
                          <th style="text-align:right;">IGV ({{impuesto}}%)</th>
                          <th style="text-align:right;">${{totalImpuesto = ((total *impuesto) / (100+impuesto)).toFixed(2)}}</th>
                        </tr>
                        <tr>
                          <th></th>
                          <th></th>
                          <th></th>
                          <th style="text-align:right;">TOTAL</th>
                          <th style="text-align:right;">{{total = (calcularTotal).toFixed(2)}}</th>
                        </tr>
                      </tfoot>
                    </table>
                  </div>
                </section>
                <br />
                <br />
                <footer>
                  <div id="gracias">
                    <p>
                      <b>Gracias por su compra!</b>
                    </p>
                  </div>
                </footer>
              </div>
              <v-btn @click="ocultarComprobante()" color="blue darken-1" flat>Cancelar</v-btn>
            </v-card-text>
          </v-card>
        </v-dialog>
      </v-toolbar>
      <v-data-table
        :headers="headers"
        :items="ventas"
        :search="search"
        class="elevation-1"
        v-if="verNuevo == 0"
      >
        <template v-slot:items="props">
          <td class="justify-center layout px-0">
            <v-icon small class="mr-2" color="blue darken-1" @click="verDetalles(props.item)">tab</v-icon>
            <v-icon
              small
              class="mr-2"
              color="blue darken-1"
              @click="mostrarComprobante(props.item)"
            >print</v-icon>
            <template v-if="props.item.estado== 'Aceptado'">
              <v-icon
                small
                color="red darken-1"
                @click="activarDesactivarMostrar(2,props.item)"
              >block</v-icon>
            </template>
          </td>
          <td>{{ props.item.usuario }}</td>
          <td>{{ props.item.clientes }}</td>
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
            <v-select v-model="idcliente" :items="cliente" label="Cliente"></v-select>
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
                <td>
                  <v-text-field type="number" v-model="props.item.descuento"></v-text-field>
                </td>
                <td>${{ props.item.cantidad * props.item.precio - props.item.descuento}}</td>
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
import jsPDF from "jspdf";
import html2canvas from "html2canvas";

export default {
  data() {
    return {
      valida: 0,
      validaMensaje: [],
      ventas: [],
      dialog: false,
      search: "",
      headers: [
        { text: "Opciones", value: "opciones", sortable: false },
        { text: "Usuario", value: "usuario" },
        { text: "Cliente", value: "cliente" },
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
        { text: "Descuento", value: "descuento", sortable: false },
        { text: "Subtotal", value: "subtotal", sortable: false }
      ],
      detalles: [],
      id: "",
      idcliente: "",
      cliente: [],
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
      adId: "",
      comprobanteModal: 0,
      clientes: '',
      fecha_hora: '',
      num_documento: '',
      direccion: '',
      telefono: '',
      email: ''
    };
  },
  computed: {
    calcularTotal: function() {
      var resultado = 0.0;
      for (var i = 0; i < this.detalles.length; i++) {
        resultado =
          resultado +
          this.detalles[i].precio * this.detalles[i].cantidad -
          this.detalles[i].descuento;
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
    crearPDF(){
      var quotes = document.getElementById('factura');
      html2canvas(quotes).then(function (canvas){
        var imgData = canvas.toDataURL('image/png');
        var imgWidth = 210;
        var pageHeight= 295;
        var imgHeight = canvas.height * imgWidth / canvas.width;
        var heightleft = imgHeight;
        var doc = new jsPDF('p','mm', 'a4');
        var position =0;
         doc.addImage(imgData, 'PNG', 0 , position, imgWidth, imgHeight);
         doc.save("ComprobanteVenta.pdf");
      });
    },
    mostrarComprobante(item) {
      this.Limpiar();
      this.tipo_comprobante = item.tipo_comprobante;
      this.serie_comprobante= item.serie_comprobante;
      this.num_comprobante = item.num_comprobante;
      this.clientes = item.cliente;
      this.num_documento = item.num_documento;
      this.direccion = item.direccion;
      this.telefono = item.telefono;
      this.email = item.email;
      this.fecha_hora = item.fecha_hora;
      this.impuesto = item.impuesto;
      this.ListarDetalles(item.idventa)
      this.comprobanteModal = 1;
    },
    ocultarComprobante() {
      this.comprobanteModal = 0;
      this.Limpiar();
    },
    Select() {
      let me = this;
      var clientesArray = [];
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .get("api/Personas/SelectClientes", configuracion)
        .then(resp => {
          clientesArray = resp.data;
          clientesArray.map(x => {
            me.cliente.push({ text: x.nombre, value: x.idpersona });
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
        .get("api/Articulos/BuscarCodigoVenta/" + this.codigo, configuracion)
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
        .get("api/Articulos/ListarVenta/" + me.texto, configuracion)
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
          precio: data["precio_venta"],
          descuento: 0
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
      let url = "";
      if (!me.search) {
        url = "api/Ventas/Listar";
      } else {
        url = "api/Ventas/ListarFiltro" + me.search;
      }
      axios
        .get(url, configuracion)
        .then(resp => {
          me.ventas = resp.data;
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
        .get("api/Ventas/ListarDetalles/" + id, configuracion)
        .then(resp => {
          me.detalles = resp.data;
        })
        .catch(error => {
          console.log(error);
        });
    },
    verDetalles(item) {
      this.Limpiar();
      this.tipo_comprobante = item.tipo_comprobante;
      this.serie_comprobante = item.serie_comprobante;
      this.num_comprobante = item.num_comprobante;
      this.idcliente = item.idcliente;
      this.impuesto = item.impuesto;
      this.ListarDetalles(item.idventa);
      this.verNuevo = 1;
      this.verDet = 1;
    },
    Limpiar() {
      this.id = "";
      this.idcliente = "";
      this.tipo_comprobante = "";
      this.serie_comprobante = "";
      this.num_comprobante = "";
      this.impuesto = 18;
      this.codigo = "";
      this.detalles = [];
      this.total = 0;
      this.totalImpuesto = 0;
      this.totalParcial = 0;
      this.verDet = 0;
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
          "api/Ventas/Crear",
          {
            idcliente: me.idcliente,
            idusuario: me.$store.state.usuario.idusuario,
            tipo_comprobante: me.tipo_comprobante,
            serie_comprobante: me.serie_comprobante,
            num_comprobante: me.num_comprobante,
            impuesto: me.impuesto,
            total: me.total,
            detalles: me.detalles
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
      this.adId = item.idventa;
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
    Desactivar() {
      let me = this;
      let headers = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: headers };
      axios
        .put("api/Ventas/Anular/" + this.adId, {}, configuracion)
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
      if (!this.idcliente) {
        this.validaMensaje.push("Seleccione un cliente.");
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
<style>
#factura {
  padding: 20px;
  font-family: Arial, sans-serif;
  font-size: 16px;
}

#logo {
  float: left;
  margin-left: 2%;
  margin-right: 2%;
}
#imagen {
  width: 100px;
}

#fact {
  font-size: 18px;
  font-weight: bold;
  text-align: center;
}

#datos {
  float: left;
  margin-top: 0%;
  margin-left: 2%;
  margin-right: 2%;
  /*text-align: justify;*/
}

#encabezado {
  text-align: center;
  margin-left: 10px;
  margin-right: 10px;
  font-size: 16px;
}

section {
  clear: left;
}

#cliente {
  text-align: left;
}

#facliente {
  width: 40%;
  border-collapse: collapse;
  border-spacing: 0;
  margin-bottom: 15px;
}

#fa {
  color: #ffffff;
  font-size: 14px;
}

#facarticulo {
  width: 100%;
  border-collapse: collapse;
  border-spacing: 0;
  padding: 20px;
  margin-bottom: 15px;
}

#facarticulo thead {
  padding: 20px;
  background: #2183e3;
  text-align: center;
  border-bottom: 1px solid #ffffff;
}

#gracias {
  text-align: center;
}
</style>