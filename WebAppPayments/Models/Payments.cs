using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebAppPayments.Domain;

namespace WebAppPayments.Models
{
    public class Payments
    {
        [Required]
        [Display(Name = "Id")] 
        public System.Guid id_transaction { get; set; }
        [Required]
        [Display(Name = "Fecha transacción")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm:ss}")]
        public System.DateTime date_transaction { get; set; }
        [Required]
        [Display(Name = "Monto transacción")] 
        public decimal transaction_amount { get; set; }
        public decimal valuebasegmf_transaccion { get; set; }
        public decimal gmf_transaction_value { get; set; }
        [Required]
        [Display(Name = "Cuenta origen")] 
        public System.Guid id_account { get; set; }
        [Required]
        [Display(Name = "Cliente")] 
        public System.Guid id_customer { get; set; }
        [Required]
        [Display(Name = "Tipo Transacción")]
        public string type_transaction { get; set; }
        [Required]
        [Display(Name = "Tipo Movimiento")] 
        public TypeMovementEnum type_movement { get; set; }
        [Required]
        [Display(Name = "Cuenta destino")] 
        public System.Guid destination_account { get; set; }
        [Required]
        [Display(Name = "Banco origen")] 
        public BanksEnum source_bank { get; set; }
        [Required]
        [Display(Name = "Banco destino")] 
        public BanksEnum destination_bank { get; set; }
    }
}