using VoTranQuocToan_2122110425.Context;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VoTranQuocToan_2122110425.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        web_aspEntities objWebsiteASP_NETEntities = new web_aspEntities();
        // GET: Admin/Brand
        public ActionResult Index(string searchTerm, int? page)
        {
            // Get all products as IQueryable
            var lstUser = objWebsiteASP_NETEntities.Users.AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(searchTerm))
            {
                lstUser = lstUser.Where(p => p.FirstName.Contains(searchTerm)
                                          || p.LastName.Contains(searchTerm)
                                          || p.Email.Contains(searchTerm));
            }

            ViewBag.CurrentFilter = searchTerm;

            // Pagination settings
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Sorting and Pagination
            var pagedList = GetPagedList(lstUser, pageSize, pageNumber);

            return View(pagedList);  // Return IPagedList<Product> to the view
        }

        private static object GetPagedList(IQueryable<User> lstUser, int pageSize, int pageNumber)
        {
            return lstUser
                            .OrderBy(p => p.FirstName)
                            .ThenBy(p => p.LastName)
                            .ThenBy(p => p.Email)
                            .ToPagedList(pageNumber, pageSize);
        }

        public ActionResult Details(int Id)
        {
            var objUser = objWebsiteASP_NETEntities.Users.Where(n => n.Id == Id).FirstOrDefault();
            return View(objUser);
        }

        [HttpGet]
        public ActionResult Delete(int Id)
        {
            var objUser = objWebsiteASP_NETEntities.Users.Where(n => n.Id == Id).FirstOrDefault();
            return View(objUser);
        }

        [HttpPost]
        public ActionResult Delete(User objUse)
        {
            var objUser = objWebsiteASP_NETEntities.Users.Where(n => n.Id == objUse.Id).FirstOrDefault();

            objWebsiteASP_NETEntities.Users.Remove(objUser);
            objWebsiteASP_NETEntities.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(User objUser)
        {
            try
            {
                // Mã hóa mật khẩu
                if (!string.IsNullOrEmpty(objUser.Password))
                {
                    objUser.Password = HashPassword(objUser.Password);
                }

                // Thêm người dùng vào cơ sở dữ liệu
                objWebsiteASP_NETEntities.Users.Add(objUser);
                objWebsiteASP_NETEntities.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // Hàm mã hóa mật khẩu
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // GET: Admin/Product
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) return HttpNotFound();

            var user = objWebsiteASP_NETEntities.Users.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User objUser)
        {
            try
            {
                var existingUser = objWebsiteASP_NETEntities.Users.Find(objUser.Id);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật các trường khác
                existingUser.FirstName = objUser.FirstName;
                existingUser.LastName = objUser.LastName;
                existingUser.Email = objUser.Email;
                existingUser.Password = objUser.Password;
                existingUser.IsAdmin = objUser.IsAdmin;

                // Đánh dấu thực thể là đã chỉnh sửa
                objWebsiteASP_NETEntities.Entry(existingUser).State = EntityState.Modified;

                // Lưu thay đổi vào cơ sở dữ liệu
                objWebsiteASP_NETEntities.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết (sử dụng thư viện log như NLog hoặc Serilog)
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình cập nhật sản phẩm. Vui lòng thử lại.";
                return RedirectToAction("Edit", new { id = objUser.Id });
            }
        }


    }
}