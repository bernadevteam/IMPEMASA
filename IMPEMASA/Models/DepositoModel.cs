using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMPEMASA.Models
{
    public class DepositoModel
    {
        public int Id { get; set; }
        public int IdCuenta { get; set; }
        public int IdDepositoTipo { get; set; }
        public int IdVenta { get; set; }
        public decimal Monto { get; set; }
        public string Cuenta { get; set; }
        public string Banco { get; set; }
        public string Tipo { get; set; }
        public string Fecha { get; set; }
    }
}