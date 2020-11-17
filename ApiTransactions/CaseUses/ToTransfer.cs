using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dataAccessPayments;


namespace ApiTransactions.CaseUses
{
    public class ToTransfer : IMovement
    {
        private string type_transaction_debito = "DEBIT";

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
            Generate_movement(transactions);
            Apply_gmf_client(transactions.id_transaction);
        }

        public void Generate_movement(Transactions transaction)
        {
            
                Transactions NewTransaction = transaction;
                NewTransaction.id_transaction = new Guid();
                NewTransaction.type_transaction = type_transaction_debito;
                NewTransaction.id_account = (Guid)transaction.destination_account;
                NewTransaction.destination_account = null;
                NewTransaction.source_bank = transaction.destination_bank;
                NewTransaction.destination_bank = transaction.destination_bank;
                NewTransaction.id_account = (Guid)transaction.destination_account;
                NewTransaction.destination_account = (Guid)transaction.id_account;

                dbAppApaymenstsContext.Transactions.Add(NewTransaction);
                dbAppApaymenstsContext.SaveChanges();

                this.Adjust_account_balance(NewTransaction.id_transaction);
            
            
        }

        public bool IsValid(Transactions transaction)
        {
            Accounts accounts = dbAppApaymenstsContext.Accounts.Find(transaction.id_account);

            return transaction.transaction_amount < accounts.current_account_balance;
        }
    }
}