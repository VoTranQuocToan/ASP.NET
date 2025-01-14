using VoTranQuocToan_2122110425.Context;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using System.Data.Entity;
namespace VoTranQuocToan_2122110425.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Admin/Category
        web_aspEntities objWebsiteASP_NETEntities = new web_aspEntities();
        public ActionResult Index(string searchTerm, int? page)
        {
            var lstCategory = objWebsiteASP_NETEntities.Categories.AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(searchTerm))
            {
                lstCategory = lstCategory.Where(p => p.Name.Contains(searchTerm));
            }

            ViewBag.CurrentFilter = searchTerm;

            // Pagination settings
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Ensure you call ToPagedList() on the IQueryable to get IPagedList
            var pagedList = lstCategory.OrderBy(p => p.Name).ToPagedList(pageNumber, pageSize);

            return View(pagedList);  // Return IPagedList<Product> to the view
        }

        public ActionResult Details(int Id)
        {
            var objCategory = objWebsiteASP_NETEntities.Categories.Where(n => n.Id == Id).FirstOrDefault();
            return View(objCategory);
        }

        [HttpGet]
        public ActionResult Delete(int Id)
        {
            var objCategory = objWebsiteASP_NETEntities.Categories.Where(n => n.Id == Id).FirstOrDefault();
            return View(objCategory);
        }

        [HttpPost]
        public ActionResult Delete(Category objCate)
        {
            var objCategory = objWebsiteASP_NETEntities.Categories.Where(n => n.Id == objCate.Id).FirstOrDefault();

            objWebsiteASP_NETEntities.Categories.Remove(objCategory);
            objWebsiteASP_NETEntities.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category objCategory)
        {
            try
            {
                if (objCategory.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(objCategory.ImageUpload.FileName);
                    string extension = Path.GetExtension(objCategory.ImageUpload.FileName);
                    fileName = fileName + extension;
                    objCategory.Avatar = fileName;
                    objCategory.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/category/"), fileName));
                }

                objWebsiteASP_NETEntities.Categories.Add(objCategory);
                objWebsiteASP_NETEntities.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        // GET: Admin/Product
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) return HttpNotFound();

            var category = objWebsiteASP_NETEntities.Categories.Find(id);
            if (category == null) return HttpNotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category objCategory)
        {
            try
            {
                var existingCategory = objWebsiteASP_NETEntities.Categories.Find(objCategory.Id);
                if (existingCategory == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra và xử lý tệp tải lên
                if (objCategory.ImageUpload != null && objCategory.ImageUpload.ContentLength > 0)
                {
                    // Xử lý ảnh
                    string fileName = Path.GetFileNameWithoutExtension(objCategory.ImageUpload.FileName);
                    string extension = Path.GetExtension(objCategory.ImageUpload.FileName);
                    fileName = fileName + extension; // Thêm timestamp để tránh trùng tên
                    string filePath = Path.Combine(Server.MapPath("~/Content/images/category/"), fileName);

                    objCategory.ImageUpload.SaveAs(filePath);

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingCategory.Avatar))
                    {
                        string oldFilePath = Path.Combine(Server.MapPath("~/Content/images/category/"), existingCategory.Avatar);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Cập nhật đường dẫn ảnh mới
                    existingCategory.Avatar = fileName;
                }

                // Cập nhật các trường khác
                existingCategory.Name = objCategory.Name;
                existingCategory.Slug = objCategory.Slug;
                existingCategory.DisplayOrder = objCategory.DisplayOrder;
                existingCategory.ShowOnHomePage = objCategory.ShowOnHomePage;
                existingCategory.Deleted = objCategory.Deleted;

                // Đánh dấu thực thể là đã chỉnh sửa
                objWebsiteASP_NETEntities.Entry(existingCategory).State = EntityState.Modified;

                // Lưu thay đổi vào cơ sở dữ liệu
                objWebsiteASP_NETEntities.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết (sử dụng thư viện log như NLog hoặc Serilog)
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình cập nhật sản phẩm. Vui lòng thử lại.";
                return RedirectToAction("Edit", new { id = objCategory.Id });
            }
        }
    }
}