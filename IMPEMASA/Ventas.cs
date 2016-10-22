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
    
    public partial class Ventas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ventas()
        {
            this.Depositos = new HashSet<Depositos>();
        }
    
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdVentaTipo { get; set; }
        public int NoFactura { get; set; }
        public decimal SubTotal { get; set; }
        public System.DateTime Fecha { get; set; }
        public decimal ITBIS { get; set; }
        public decimal Total { get; set; }
        public string RNC { get; set; }
        public Nullable<bool> PagoPendiente { get; set; }
    
        public virtual Clientes Clientes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Depositos> Depositos { get; set; }
        public virtual VentaTipos VentaTipos { get; set; }
    }
}
