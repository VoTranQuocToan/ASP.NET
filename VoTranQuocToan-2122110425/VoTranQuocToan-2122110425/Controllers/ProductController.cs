using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VoTranQuocToan_2122110425.Context;

namespace VoTranQuocToan_2122110425.Controllers
{
    public class ProductController : Controller
    {
        web_aspEntities objweb_aspEntities = new web_aspEntities();
        public ActionResult Index(int Id)
        {
            var lstProduct = objweb_aspEntities.Products.Where(n => n.Id == Id).FirstOrDefault();
            return View(lstProduct);
        }
    }
}