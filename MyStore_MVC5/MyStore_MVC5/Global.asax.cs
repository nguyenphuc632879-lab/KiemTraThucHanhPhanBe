using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MyStore_MVC5.Models;

namespace MyStore_MVC5
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Tự động tạo LocalDB + seed dữ liệu mẫu khi chạy lần đầu
            Database.SetInitializer(new DbInitializer());
            using (var db = new DBEntities())
            {
                db.Database.Initialize(force: false);
            }
        }
    }
}
