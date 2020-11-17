using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppPayments.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

 

namespace WebAppPayments.Controllers
{
    public class PaymentsController : Controller
    {
        string Baseurl = "http://localhost:63280/";
        // GET: Payments
        public async Task<ActionResult> Index()
        {
            List<Payments> paymentsInfo = new List<Payments>();

            using ( var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/Transactions");

                if (Res.IsSuccessStatusCode)
                {
                    var PaymentsResponse = Res.Content.ReadAsStringAsync().Result;

                    paymentsInfo = JsonConvert.DeserializeObject<List<Payments>>(PaymentsResponse);
                }
            }

                return View(paymentsInfo);
        }

        public ActionResult create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult create(Payments payments)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var postTask = client.PostAsJsonAsync<Payments>("api/Transactions", payments);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError(string.Empty, "Se ha prensentado un error cargando el formulario");
            return View(payments);
        }
    }
}