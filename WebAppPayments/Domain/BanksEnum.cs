using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppPayments.Domain
{
    public enum BanksEnum
    {
        [Display(Name = "Banco BBVA")] 
        BBVA = 1,
        [Display(Name = "Banco BANORTE")]
        BANORTE = 2
    }
}