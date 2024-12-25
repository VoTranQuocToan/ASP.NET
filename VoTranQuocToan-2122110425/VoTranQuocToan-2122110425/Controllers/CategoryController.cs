using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VoTranQuocToan_2122110425.Context;

namespace VoTranQuocToan_2122110425.Controllers
{
    public class CategoryController : Controller
    {
        web_aspEntities objweb_aspEntities = new web_aspEntities();
        // GET: Category
        public ActionResult Index()
        {
            var lstCategory = objweb_aspEntities.Categories.ToList();
            return View(lstCategory);
        }
        public ActionResult ProductCategory(int Id)
        {
            var listProduct = objweb_aspEntities.Products.Where(n=>n.CategoryId == Id).ToList();
            return View(listProduct);
        }
    }
}