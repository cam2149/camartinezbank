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

namespace ApiAccounts.Controllers
{
    public class AccountsController : ApiController
    {
        private AppApaymentsEntities dbContext = new AppApaymentsEntities();

        // GET: api/Accounts
        [HttpGet]
        public IEnumerable<Accounts> GetAccounts()
        {
            try
            {
                using (AppApaymentsEntities accountEntities = new AppApaymentsEntities())
                {
                    return accountEntities.Accounts.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("No es posible obtener informacion de las cuentas en este momento, intente de nuevo!"), e);
            }
        }

        // GET: api/Accounts/5
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult GetAccounts(Guid id)
        {
            Accounts accounts = dbContext.Accounts.Find(id);
            if (accounts == null)
            {
                return Content(HttpStatusCode.NotFound,
                    string.Format("No es posible obtener informacion de la cuenta {0} en este momento, intente de nuevo!", id.ToString()));
            }

            return Ok(accounts);
        }

        // PUT: api/Accounts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAccounts(Guid id, Accounts accounts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != accounts.id_account)
            {
                return BadRequest();
            }

            dbContext.Entry(accounts).State = EntityState.Modified;

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!AccountsExists(id))
                {
                    return Content(HttpStatusCode.NotFound,
                   string.Format("No es posible obtener informacion de la cuenta {0} en este momento, intente de nuevo!", id.ToString()));
                }
                else
                {
                    throw new DbUpdateConcurrencyException(string.Format("No es posible obtener informacion en este momento, intente de nuevo!"), e);
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Accounts
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult PostAccounts(Accounts accounts)
        {
            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.BadRequest,
                   string.Format("No es posible guardar la informacion de la cuenta {0} en este momento, intente de nuevo!", accounts.id_account.ToString()));
            }

            dbContext.Accounts.Add(accounts);

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                if (AccountsExists(accounts.id_account))
                {
                    return Conflict();
                }
                else
                {
                    throw new DbUpdateException(string.Format("No es posible obtener informacion en este momento, intente de nuevo!"), e);
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = accounts.id_account }, accounts);
        }

        // DELETE: api/Accounts/5
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult DeleteAccounts(Guid id)
        {
            Accounts accounts = dbContext.Accounts.Find(id);
            if (accounts == null)
            {
                return Content(HttpStatusCode.NotFound,
               string.Format("No es posible obtener informacion de la cuenta {0} en este momento, intente de nuevo!", id.ToString()));

            }

            dbContext.Accounts.Remove(accounts);
            dbContext.SaveChanges();

            return Ok(accounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountsExists(Guid id)
        {
            return dbContext.Accounts.Count(e => e.id_account == id) > 0;
        }
    }
}