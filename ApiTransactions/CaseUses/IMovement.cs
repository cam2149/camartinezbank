using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dataAccessPayments;

namespace ApiTransactions.CaseUses
{
    public interface IMovement
    {
        int Adjust_account_balance(Guid id_transaction);

        int Apply_gmf_client(Guid id_transaction);

        void Generate_movement(Transactions transaction);

        bool IsValid(Transactions transaction);

        void CompleteTransactions(Transactions transactions);
    }
}
