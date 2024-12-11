using ASP.NET.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP.NET.Controllers
{
    public class CategoryController : Controller
    {
        ASP_NETEntities4 objASPNETEntities = new ASP_NETEntities4();
        public ActionResult AllCategory()
        {
            var lstAllCategory = objASPNETEntities.categories.ToList();
            return View(lstAllCategory);
        }
    }
}