//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace dataAccessPayments
{
    using System;
    using System.Collections.Generic;
    
    public partial class Transactions
    {
        public System.Guid id_transaction { get; set; }
        public System.DateTime date_transaction { get; set; }
        public decimal transaction_amount { get; set; }
        public decimal valuebasegmf_transaccion { get; set; }
        public decimal gmf_transaction_value { get; set; }
        public Nullable<System.Guid> destination_account { get; set; }
        public System.Guid id_account { get; set; }
        public System.Guid id_customer { get; set; }
        public string source_bank { get; set; }
        public string destination_bank { get; set; }
        public string type_transaction { get; set; }
        public string type_movement { get; set; }
    
        public virtual Accounts Accounts { get; set; }
        public virtual Clients Clients { get; set; }
    }
}