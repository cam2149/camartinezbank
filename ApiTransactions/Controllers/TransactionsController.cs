using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using dataAccessPayments;
using ApiTransactions.CaseUses;

namespace ApiTransactions.Controllers
{
    public class TransactionsController : ApiController
    {
        private string type_transaction_credito = "CREDIT";
        private string type_transaction_debito = "DEBIT";
        private ToTransfer toTransfer = new ToTransfer();
        private Retire retire = new Retire();
        private Deposit deposit = new Deposit();
        private AppApaymentsEntities dbAppApaymenstsContext = new AppApaymentsEntities();

        // GET: api/Transactions
        public IEnumerable<Transactions> GetTransactions()
        {
            try
            {
                using (AppApaymentsEntities transactionEntities = new AppApaymentsEntities())
                {
                    return transactionEntities.Transactions.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("No es posible obtener informacion en este momento, intente de nuevo!"), e);
            }
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult GetTransactions(Guid id)
        {
            Transactions transactions = dbAppApaymenstsContext.Transactions.Find(id);
            if (transactions == null)
            {
                return Content(HttpStatusCode.NotFound, string.Format("No se ha encontrado información de la transacció {0}", id.ToString()));
            }

            return Ok(transactions);
        }

        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTransactions(Guid id, Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transactions.id_transaction)
            {
                return BadRequest();
            }


            dbAppApaymenstsContext.Entry(transactions).State = EntityState.Modified;

            try
            {
                dbAppApaymenstsContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        private void ApplyTypeTransaction(Transactions transactions)
        {
            switch (transactions.type_movement.Trim())
            {
                case "1":
                    transactions.type_transaction = type_transaction_credito;
                    break;
                case "2":
                    transactions.type_transaction = type_transaction_credito;
                    break;
                case "3":
                    transactions.type_transaction = type_transaction_debito;
                    break;
            }
        }

        // POST: api/Transactions
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult PostTransactions(Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplyTypeTransaction(transactions);
            if (!ValidTransaction(transactions))
            {
                return BadRequest(ModelState);
            }


            dbAppApaymenstsContext.Transactions.Add(transactions);

            try
            {
                dbAppApaymenstsContext.SaveChanges();

                CompleteTransactions(transactions);

            }
            catch (DbUpdateException)
            {
                if (TransactionsExists(transactions.id_transaction))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = transactions.id_transaction }, transactions);
        }

        private bool ValidTransaction(Transactions transactions)
        {
            bool bReturnValue = false;
            switch (transactions.type_movement.Trim())
            {
                case "1":
                    bReturnValue = toTransfer.IsValid(transactions);
                    break;
                case "2":
                    bReturnValue = retire.IsValid(transactions);
                    break;
                case "3":
                    bReturnValue = deposit.IsValid(transactions);
                    break;
            }

            return bReturnValue;
        }

        // DELETE: api/Transactions/5
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult DeleteTransactions(Guid id)
        {
            Transactions transactions = dbAppApaymenstsContext.Transactions.Find(id);
            if (transactions == null)
            {
                return NotFound();
            }

            dbAppApaymenstsContext.Transactions.Remove(transactions);
            dbAppApaymenstsContext.SaveChanges();

            return Ok(transactions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbAppApaymenstsContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionsExists(Guid id)
        {
            return dbAppApaymenstsContext.Transactions.Count(e => e.id_transaction == id) > 0;
        }

        private void CompleteTransactions(Transactions transactions)
        {
            
            switch (transactions.type_movement.Trim())
            {
                case "1":
                    toTransfer.CompleteTransactions(transactions);
                    break;
                case "2":
                    retire.CompleteTransactions(transactions);
                    break;
                case "3":
                    deposit.CompleteTransactions(transactions);
                    break;
            }
        }


    }
}