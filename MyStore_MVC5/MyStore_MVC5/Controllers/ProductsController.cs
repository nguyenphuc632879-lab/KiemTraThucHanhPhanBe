using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyStore_MVC5.Models;

namespace MyStore_MVC5.Controllers
{
    public class ProductsController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product = db.Products.Include(p => p.Categories).FirstOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
