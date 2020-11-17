using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppPayments.Domain
{
    public enum TypeMovementEnum
    {
        [Display(Name = "Transferecia")]
        TRANSFERENCIA = 1,
        [Display(Name = "Retiro")]
        CREDITO = 2,
        [Display(Name = "Consignación")]
        DEBITO = 3
    }
}