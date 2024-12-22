﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanQuanAo.Filters;
using WebsiteBanQuanAo.Models;

namespace DoAnChuyenNganh.Controllers
{
    [MyAuthenFilter]
    [UserAuthorization]
    public class OrderController : Controller
    {
        // GET: Order
        private readonly ShopQuanAoEntities db = new ShopQuanAoEntities();

        // GET: Order
        public ActionResult Index(int page = 1, string sortOrder = "desc")
        {
            CheckUserLoggedIn();
            int userId = GetCurrentUserId();
            List<DonHang> lst = db.DonHangs.Where(x => x.NguoiDungID == userId).ToList();
            List<GioHang> cart = db.GioHangs.Where(g => g.NguoiDungID == userId).ToList();
            int totalQuantity = 0;
            if (cart == null || !cart.Any())
            {
                ViewBag.SLSP = 0;
            }
            if (cart.Any())
            {
                foreach (var item in cart)
                {
                    totalQuantity += item.SoLuong;
                }
            }
            int NoOfRecordPerPage = 5;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(lst.Count) / NoOfRecordPerPage));
            int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;

            // Sort by DateCreated, either descending or ascending
            if (sortOrder == "asc")
            {
                lst = lst.OrderBy(d => d.NgayDatHang).ToList();  // Oldest first
            }
            else
            {
                lst = lst.OrderByDescending(d => d.NgayDatHang).ToList();  // Newest first
            }

            ViewBag.Page = page;
            ViewBag.NoOfPages = NoOfPages;
            lst = lst.Skip(NoOfRecordToSkip).Take(NoOfRecordPerPage).ToList();
            ViewBag.SLSP = totalQuantity;
            ViewBag.SortOrder = sortOrder;
            return View(lst);
        }
        public ActionResult Details(int id)
        {
            CheckUserLoggedIn();

            int userId = GetCurrentUserId();
            List<GioHang> cart = db.GioHangs.Where(g => g.NguoiDungID == userId).ToList();
            int totalQuantity = 0;
            if (cart == null || !cart.Any())
            {
                ViewBag.SLSP = 0;
            }
            if (cart.Any())
            {
                foreach (var item in cart)
                {
                    totalQuantity += item.SoLuong;
                }
            }

            ViewBag.SLSP = totalQuantity;

            // Lấy thông tin đơn hàng của người dùng
            var order = db.DonHangs
                          .Where(o => o.DonHangID == id && o.NguoiDungID == userId)
                          .FirstOrDefault();

            if (order == null)
            {
                return HttpNotFound();
            }

            // Lấy chi tiết đơn hàng
            List<ChiTietDonHang> orderDetails = db.ChiTietDonHangs.Where(x => x.DonHangID == id).ToList();

            // Trả lại view cùng với thông tin đơn hàng và chi tiết đơn hàng
            ViewBag.Order = order;
            return View(orderDetails);
        }
        public ActionResult Cancel(int id)
        {
            var order = db.DonHangs.Find(id);
            if (order != null && order.TinhTrangDonHang != "Đã Xác Nhận")
            {
                // Xử lý hủy đơn hàng
                order.TinhTrangDonHang = "Đã Hủy"; // Hoặc trạng thái phù hợp
                db.SaveChanges();
            }
            List<ChiTietDonHang> lst = db.ChiTietDonHangs.Where(x => x.DonHangID == id).ToList();
            foreach (ChiTietDonHang a in lst)
            {
                a.ChiTietSanPham.SoLuongTonKho += a.SoLuong;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private ActionResult CheckUserLoggedIn()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            return null;
        }
        private int GetCurrentUserId()
        {
            var authCookie = Request.Cookies["auth"];
            if (authCookie != null)
            {
                string tenDangNhap = authCookie.Value;
                var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == tenDangNhap);
                if (user != null)
                {
                    return user.NguoiDungID;
                }
            }
            return 0;
        }
    }
}