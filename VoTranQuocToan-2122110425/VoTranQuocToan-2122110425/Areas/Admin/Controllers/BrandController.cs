using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VoTranQuocToan_2122110425.Context;

namespace VoTranQuocToan_2122110425.Areas.Admin.Controllers
{
    public class BrandController : Controller
    {
        web_aspEntities objweb_aspEntities = new web_aspEntities();
        // GET: Admin/Brand
        public ActionResult Index(string searchTerm, int? page)
        {
            // Get all products as IQueryable
            var lstBrand = objweb_aspEntities.Brands.AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(searchTerm))
            {
                lstBrand = lstBrand.Where(p => p.Name.Contains(searchTerm));
            }

            ViewBag.CurrentFilter = searchTerm;

            // Pagination settings
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Ensure you call ToPagedList() on the IQueryable to get IPagedList
            var pagedList = lstBrand.OrderBy(p => p.Name).ToPagedList(pageNumber, pageSize);

            return View(pagedList);  // Return IPagedList<Product> to the view
        }
        public ActionResult Details(int Id)
        {
            var objBrand = objweb_aspEntities.Brands.Where(n => n.Id == Id).FirstOrDefault();
            return View(objBrand);
        }

        [HttpGet]
        public ActionResult Delete(int Id)
        {
            var objBrand = objweb_aspEntities.Brands.Where(n => n.Id == Id).FirstOrDefault();
            return View(objBrand);
        }

        [HttpPost]
        public ActionResult Delete(Brand objBra)
        {
            var objBrand = objweb_aspEntities.Brands.Where(n => n.Id == objBra.Id).FirstOrDefault();

            objweb_aspEntities.Brands.Remove(objBrand);
            objweb_aspEntities.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Brand objBrand)
        {
            try
            {
                if (objBrand.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(objBrand.ImageUpload.FileName);
                    string extension = Path.GetExtension(objBrand.ImageUpload.FileName);
                    fileName = fileName + extension;
                    objBrand.Avatar = fileName;
                    objBrand.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/images/brand/"), fileName));
                }

                objweb_aspEntities.Brands.Add(objBrand);
                objweb_aspEntities.SaveChanges();

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

            var brand = objweb_aspEntities.Brands.Find(id);
            if (brand == null) return HttpNotFound();

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Brand objBrand)
        {
            try
            {
                var existingBrand = objweb_aspEntities.Brands.Find(objBrand.Id);
                if (existingBrand == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra và xử lý tệp tải lên
                if (objBrand.ImageUpload != null && objBrand.ImageUpload.ContentLength > 0)
                {
                    // Xử lý ảnh
                    string fileName = Path.GetFileNameWithoutExtension(objBrand.ImageUpload.FileName);
                    string extension = Path.GetExtension(objBrand.ImageUpload.FileName);
                    fileName = fileName + extension; // Thêm timestamp để tránh trùng tên
                    string filePath = Path.Combine(Server.MapPath("~/Content/images/brand/"), fileName);    

                    objBrand.ImageUpload.SaveAs(filePath);

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingBrand.Avatar))
                    {
                        string oldFilePath = Path.Combine(Server.MapPath("~/Content/images/brand/"), existingBrand.Avatar);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Cập nhật đường dẫn ảnh mới
                    existingBrand.Avatar = fileName;
                }

                // Cập nhật các trường khác
                existingBrand.Name = objBrand.Name;
                existingBrand.Slug = objBrand.Slug;
                existingBrand.DisplayOrder = objBrand.DisplayOrder;
                existingBrand.ShowOnHomePage = objBrand.ShowOnHomePage;
                existingBrand.Deleted = objBrand.Deleted;

                // Đánh dấu thực thể là đã chỉnh sửa
                objweb_aspEntities.Entry(existingBrand).State = EntityState.Modified;

                // Lưu thay đổi vào cơ sở dữ liệu
                objweb_aspEntities.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết (sử dụng thư viện log như NLog hoặc Serilog)
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình cập nhật sản phẩm. Vui lòng thử lại.";
                return RedirectToAction("Edit", new { id = objBrand.Id });
            }
        }

    }
}