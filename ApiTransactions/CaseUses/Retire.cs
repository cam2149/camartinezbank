using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dataAccessPayments;

namespace ApiTransactions.CaseUses
{
    public class Retire : IMovement
    {
        private AppApaymentsEntities dbAppApaymenstsContext = new AppApaymentsEntities();

        public int Adjust_account_balance(Guid id_transaction)
        {
            return dbAppApaymenstsContext.adjust__account_balance(new System.Data.Entity.Core.Objects.ObjectParameter("id_transaction", id_transaction));
        }

        public int Apply_gmf_client(Guid id_transaction)
        {
            return dbAppApaymenstsContext.apply_gmf_client(new System.Data.Entity.Core.Objects.ObjectParameter("id_transaction", id_transaction));
        }

        public void CompleteTransactions(Transactions transactions)
        {
            Adjust_account_balance(transactions.id_transaction);
            Apply_gmf_client(transactions.id_transaction);
        }

        public void Generate_movement(Transactions id_transaction)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(Transactions transaction)
        {
            bool bReturnValue = false;
            Accounts accounts = dbAppApaymenstsContext.Accounts.Find(transaction.id_account);
            bReturnValue = transaction.transaction_amount < accounts.current_account_balance;
            bReturnValue = accounts.bank_account == transaction.source_bank;
            bReturnValue = transaction.source_bank == transaction.destination_bank;
            return bReturnValue;

        }
    }
}