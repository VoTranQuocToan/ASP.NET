using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VoTranQuocToan_2122110425.Context;
using VoTranQuocToan_2122110425.Models;

namespace VoTranQuocToan_2122110425.Controllers
{
    public class HomeController : Controller
    {
        web_aspEntities objweb_aspEntities = new web_aspEntities();
        public ActionResult Index()
        {
            HomeModel objHomeModel = new HomeModel();
            objHomeModel.ListProduct = objweb_aspEntities.Products.ToList();
            objHomeModel.ListCategory = objweb_aspEntities.Categories.ToList();
            return View(objHomeModel);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User _user)
        {
            if (ModelState.IsValid)
            {
                var check = objweb_aspEntities.Users.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    _user.Password = GetMD5(_user.Password);
                    objweb_aspEntities.Configuration.ValidateOnSaveEnabled = false;
                    _user.Id = objweb_aspEntities.Users.Max(u => u.Id) + 1;
                    objweb_aspEntities.Users.Add(_user);
                    objweb_aspEntities.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }


            }
            return View();


        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            StringBuilder byte2String = new StringBuilder();
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String.Append(targetData[i].ToString("x2"));
            }
            return byte2String.ToString();

        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {


                var f_password = GetMD5(password);
                var data = objweb_aspEntities.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["FirstName"] = data.FirstOrDefault().FirstName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["idUser"] = data.FirstOrDefault().Id;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }


        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }
        public ActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("SearchResults", new List<Product>()); // Không có kết quả
            }

            // Tìm kiếm sản phẩm theo tên hoặc mô tả
            var products = objweb_aspEntities.Products
                .Where(p => p.Name.Contains(query) || p.ShortDes.Contains(query))
                .ToList();

            // Trả về view với danh sách sản phẩm tìm được
            return View("SearchResults", products);
        }
    }
}
