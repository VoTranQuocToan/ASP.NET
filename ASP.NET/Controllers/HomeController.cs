using ASP.NET.Context;
using ASP.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP.NET.Controllers
{
    public class HomeController : Controller
    {
        ASP_NETEntities4 objASPNETEntities = new ASP_NETEntities4();
        public ActionResult Index()
        {
            HomeModel objHomeModel = new HomeModel();
            // Lấy danh sách sản phẩm và danh mục từ cơ sở dữ liệu
            objHomeModel.ListProduct = objASPNETEntities.Products.ToList();
            objHomeModel.ListCategory = objASPNETEntities.categories.ToList();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}