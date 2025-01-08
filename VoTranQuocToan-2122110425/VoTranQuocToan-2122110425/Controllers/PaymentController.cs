using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VoTranQuocToan_2122110425.Context;
using VoTranQuocToan_2122110425.Models;

namespace VoTranQuocToan_2122110425.Controllers
{
    public class PaymentController : Controller
    {
        web_aspEntities objWebsiteASP_NETEntities = new web_aspEntities();

        public ActionResult Index()
        {
            // Kiểm tra người dùng đã đăng nhập hay chưa
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                // Lấy thông tin từ giỏ hàng trong session
                var lstCart = (List<CartModel>)Session["cart"];
                if (lstCart == null || !lstCart.Any())
                {
                    TempData["Error"] = "Giỏ hàng trống. Vui lòng chọn sản phẩm trước khi thanh toán.";
                    return RedirectToAction("Index", "Cart");
                }

                // Tạo dữ liệu cho Order
                Order objOrder = new Order
                {
                    Name = "DonHang-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Userid = int.Parse(Session["idUser"].ToString()),
                    CreatedOnUtc = DateTime.Now,
                    Status = 17
                };

                // Thêm Order vào cơ sở dữ liệu
                objWebsiteASP_NETEntities.Orders.Add(objOrder);

                // Lưu thông tin vào bảng Order
                objWebsiteASP_NETEntities.SaveChanges();

                // Lấy OrderId vừa tạo để lưu vào bảng OrderDetail
                int orderId = objOrder.Id;
                List<OrderDetail> lstOrderDetail = new List<OrderDetail>();

                foreach (var item in lstCart)
                {
                    OrderDetail obj = new OrderDetail
                    {
                        Quantity = item.Quantity,
                        OrderId = orderId,
                        ProductId = item.Product.Id
                    };
                    lstOrderDetail.Add(obj);
                }

                // Lưu thông tin vào bảng OrderDetail
                objWebsiteASP_NETEntities.OrderDetails.AddRange(lstOrderDetail);
                objWebsiteASP_NETEntities.SaveChanges();

                // Xóa giỏ hàng trong session sau khi thanh toán
                Session["cart"] = null; // Giỏ hàng đã thanh toán sẽ bị xóa
                Session["count"] = 0;
            }

            return View();
        }
    }
}