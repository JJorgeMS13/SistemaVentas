<template>
  <v-container grid-list-m>
    <v-layout wrap>
      <v-flex xs12 sm12 md12>
        <div>
          <canvas id="myChart">

          </canvas>
        </div>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import axios from 'axios';
import Chart from 'chart.js';

export default {
  data() {
    return {
      meseValores: null,
      nombreMeses: [],
      totalMeses: []
    };
  },
  methods: {
    loadProductosMasVendidos() {
      let me=this;
      let mesesNombre= '';

      me.meseValores.map(function(x){
        switch(parseInt(x.etiqueta)){
          case 1:
            mesesNombre = 'Enero';
            break;
          case 2:
            mesesNombre = 'Febrero';
            break;
          case 3:
            mesesNombre = 'Marzo';
            break;
          case 4:
            mesesNombre = 'Abril';
            break;
          case 5:
            mesesNombre = 'Mayo';
            break;
          case 6:
            mesesNombre = 'Junio';
            break;
          case 7:
            mesesNombre = 'Julio';
            break;              
          case 8:
            mesesNombre = 'Agosto';
            break;
          case 9:
            mesesNombre = 'Septiembre';
            break;
          case 10:
            mesesNombre = 'Octubre';
            break;
          case 11:
            mesesNombre = 'Noviembre';
            break;
          case 12:
            mesesNombre = 'Diciembre';
            break;      
            default:
              mesesNombre = 'Error';


        }
        me.nombreMeses.push(mesesNombre);
        me.totalMeses.push(x.valor);
      });
      var ctx = document.getElementById("myChart");
      var myChart = new Chart(ctx, {
        type: "bar",
        data: {
          labels: me.nombreMeses,
          datasets: [
            {
              label: "Ventas en los últimos 12 Meses",
              data: me.totalMeses,
              backgroundColor: [
                "rgba(255, 99, 132, 0.2)",
                "rgba(54, 162, 235, 0.2)",
                "rgba(255, 206, 86, 0.2)",
                "rgba(75, 192, 192, 0.2)",
                "rgba(153, 102, 255, 0.2)",
                "rgba(255, 159, 64, 0.2)",
                "rgba(255, 99, 132, 0.2)",
                "rgba(54, 162, 235, 0.2)",
                "rgba(255, 206, 86, 0.2)",
                "rgba(75, 192, 192, 0.2)",
                "rgba(153, 102, 255, 0.2)",
                "rgba(255, 159, 64, 0.2)"                
              ],
              borderColor: [
                "rgba(255, 99, 132, 1)",
                "rgba(54, 162, 235, 1)",
                "rgba(255, 206, 86, 1)",
                "rgba(75, 192, 192, 1)",
                "rgba(153, 102, 255, 1)",
                "rgba(255, 159, 64, 1)",
                "rgba(255, 99, 132, 1)",
                "rgba(54, 162, 235, 1)",
                "rgba(255, 206, 86, 1)",
                "rgba(75, 192, 192, 1)",
                "rgba(153, 102, 255, 1)",
                "rgba(255, 159, 64, 1)"                
              ],
              borderWidth: 1
            }
          ]
        },
        options: {
          scales: {
            yAxes: [
              {
                ticks: {
                  beginAtZero: true
                }
              }
            ]
          }
        }
      });
    },
    getProductosMasVendidos(){
      let me = this;
      let headers ={"Authorization" : "Bearer " + this.$store.state.token};
      let configuracion = {headers : headers}
      axios
        .get("api/Ventas/VentasMes12",configuracion)
        .then(resp => {
          me.meseValores = resp.data;
          me.loadProductosMasVendidos();
        })
        .catch(error => {
          console.log(error);
        });
    }
  },
  mounted() {
    this.getProductosMasVendidos();
  }
};
</script>

<style>
</style>
