//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMPEMASA
{
    using System;
    using System.Collections.Generic;
    
    public partial class View_Ventas
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdVentaTipo { get; set; }
        public int NoFactura { get; set; }
        public decimal SubTotal { get; set; }
        public string Fecha { get; set; }
        public string FechaVencimiento { get; set; }
        public decimal ITBIS { get; set; }
        public decimal Total { get; set; }
        public string RNC { get; set; }
        public string Cliente { get; set; }
        public string Tipo { get; set; }
        public Nullable<decimal> Depositado { get; set; }
        public Nullable<int> DiasPendientes { get; set; }
    }
}
