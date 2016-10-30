using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMPEMASA.Models
{
    public class VentaModel
    {

        public int Id { get; set; }
        public int IdVentaTipo { get; set; }
        public int IdCliente { get; set; }
        public int NoFactura { get; set; }
        public bool PagoPendiente { get; set; }
        public decimal ITBIS { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string Fecha { get; set; }
        public string RNC { get; set; }
        public string FechaVencimiento { get; set; }
        public string Cliente { get; set; }
        public string Tipo { get; set; }

        public DepositoModel[] Depositos { get; set; }
    }
}