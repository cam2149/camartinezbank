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

namespace ApiCustomers.Controllers
{
    public class ClientsController : ApiController
    {
        private AppApaymentsEntities dbAppApaymenstsContext = new AppApaymentsEntities();


        // GET: api/Clients
        [HttpGet]
        public IEnumerable<Clients> GetClients()
        {
            try
            {
                using (AppApaymentsEntities accountEntities = new AppApaymentsEntities())
                {
                    return accountEntities.Clients.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("No es posibcaacara$89le obtener informacion en este momento, intente de nuevo!"), e);
            }
        }

        // GET: api/Clients/5
        [ResponseType(typeof(Clients))]
        public IHttpActionResult GetClients(Guid id)
        {
            Clients clients = dbAppApaymenstsContext.Clients.Find(id);
            if (clients == null)
            {
                return NotFound();
            }

            return Ok(clients);
        }

        // PUT: api/Clients/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClients(Guid id, Clients clients)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != clients.id_customer)
            {
                return BadRequest();
            }

            dbAppApaymenstsContext.Entry(clients).State = EntityState.Modified;

            try
            {
                dbAppApaymenstsContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientsExists(id))
                {
                    return Content(HttpStatusCode.NotFound, string.Format("No se ha encontrado información de la cuanta {0}", id.ToString()));
                }
                else
                {
                    throw new Exception(string.Format("No es posible obtener informacion del cliente {0} en este momento, intente de nuevo!", id.ToString()));
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Clients
        [ResponseType(typeof(Clients))]
        public IHttpActionResult PostClients(Clients clients)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dbAppApaymenstsContext.Clients.Add(clients);

            try
            {
                dbAppApaymenstsContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ClientsExists(clients.id_customer))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = clients.id_customer }, clients);
        }

        // DELETE: api/Clients/5
        [ResponseType(typeof(Clients))]
        public IHttpActionResult DeleteClients(Guid id)
        {
            Clients clients = dbAppApaymenstsContext.Clients.Find(id);
            if (clients == null)
            {
                return NotFound();
            }

            dbAppApaymenstsContext.Clients.Remove(clients);
            dbAppApaymenstsContext.SaveChanges();

            return Ok(clients);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbAppApaymenstsContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientsExists(Guid id)
        {
            return dbAppApaymenstsContext.Clients.Count(e => e.id_customer == id) > 0;
        }
    }
}