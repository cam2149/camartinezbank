using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dataAccessPayments;

namespace ApiAccounts.Controllers
{
    public class AutoAccountsController : Controller
    {
        private AppApaymentsEntities db = new AppApaymentsEntities();

        // GET: AutoAccounts
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.Clients);
            return View(accounts.ToList());
        }

        // GET: AutoAccounts/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accounts accounts = db.Accounts.Find(id);
            if (accounts == null)
            {
                return HttpNotFound();
            }
            return View(accounts);
        }

        // GET: AutoAccounts/Create
        public ActionResult Create()
        {
            ViewBag.id_customer = new SelectList(db.Clients, "id_customer", "customer_code");
            return View();
        }

        // POST: AutoAccounts/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_account,number_account,id_customer,current_account_balance,bank_account,exemption_gmf")] Accounts accounts)
        {
            if (ModelState.IsValid)
            {
                accounts.id_account = Guid.NewGuid();
                db.Accounts.Add(accounts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_customer = new SelectList(db.Clients, "id_customer", "customer_code", accounts.id_customer);
            return View(accounts);
        }

        // GET: AutoAccounts/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accounts accounts = db.Accounts.Find(id);
            if (accounts == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_customer = new SelectList(db.Clients, "id_customer", "customer_code", accounts.id_customer);
            return View(accounts);
        }

        // POST: AutoAccounts/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_account,number_account,id_customer,current_account_balance,bank_account,exemption_gmf")] Accounts accounts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accounts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_customer = new SelectList(db.Clients, "id_customer", "customer_code", accounts.id_customer);
            return View(accounts);
        }

        // GET: AutoAccounts/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accounts accounts = db.Accounts.Find(id);
            if (accounts == null)
            {
                return HttpNotFound();
            }
            return View(accounts);
        }

        // POST: AutoAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Accounts accounts = db.Accounts.Find(id);
            db.Accounts.Remove(accounts);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
