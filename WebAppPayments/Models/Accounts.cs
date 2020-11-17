using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebAppPayments.Domain;
using System.Linq;
using System.Web;

namespace WebAppPayments.Models
{
    public class Accounts
    {
        [Required]
        
        public System.Guid id_account { get; set; }
        [Required]
        public string number_account { get; set; }
        [Required] 
        public System.Guid id_customer { get; set; }
        [Required] 
        public decimal current_account_balance { get; set; }
        [Required] 
        public string bank_account { get; set; }
        [Required] 
        public bool exemption_gmf { get; set; }
    }
}