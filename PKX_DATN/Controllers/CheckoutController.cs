using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PKX_DATN.Extension;
using Domain.Helpers;
using Domain.Models;
using PKX_DATN.ModelViews;
using Microsoft.AspNetCore.Authorization;

namespace PKX_DATN.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly WebPkxContext _context;
        public INotyfService _notyfService { get; }


        public CheckoutController(WebPkxContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public List<SanPhamGioHang> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<SanPhamGioHang>>("GioHang");
                if (gh == default(List<SanPhamGioHang>))
                {
                    gh = new List<SanPhamGioHang>();
                }
                return gh;
            }
        }

        [Authorize]
        //GET: Checkout/Index
        [Route("/checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            //lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<SanPhamGioHang>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.TblKhachHangs.AsNoTracking().SingleOrDefault(x => x.IdKhachHang == Convert.ToInt32(taikhoanID));
                model.ID_KhachHang = khachhang.IdKhachHang;
                model.sHoTen = khachhang.STenKhachHang;
                model.sEmail = khachhang.SEmail;
                model.sSDT = khachhang.SSdt;
                model.sDiaChi = khachhang.SDiaChi;

            }
            /*ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");*/
            ViewBag.GioHang = cart;
            return View(model);
        }

        [HttpPost]
        [Route("/checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            //lay gio hang ra de xu ly
            var giohang = HttpContext.Session.Get<List<SanPhamGioHang>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID == null)
            {
                var khachhang = _context.TblKhachHangs.AsNoTracking().SingleOrDefault(x => x.IdKhachHang == Convert.ToInt32(taikhoanID));
                model.ID_KhachHang = khachhang.IdKhachHang;
                model.sHoTen = khachhang.STenKhachHang;
                model.sEmail = khachhang.SEmail;
                model.sSDT = khachhang.SSdt;
                model.sDiaChi = khachhang.SDiaChi;

                //khachhang.LocationId = muaHang.TinhThanh;
                //khachhang.District = muaHang.QuanHuyen;
                //khachhang.Ward = muaHang.PhuongXa;
                khachhang.SDiaChi = muaHang.sDiaChi;

                _context.Update(khachhang);
                _context.SaveChanges();
            }
            try
            {
                //khoi tao don hang
                TblDonHang donhang = new TblDonHang();
                donhang.IdKhachHang = Int32.Parse(taikhoanID.ToString());
                donhang.SDiaChi = muaHang.sDiaChi;
                donhang.DNgayTao = DateTime.Now;
                donhang.IdTrangThai = 1; //don hang moi            
                donhang.SGhiChu = Utilities.StripHTML(model.sGhiChu);
                donhang.FTongTien = Convert.ToInt32(giohang.Sum(x => x.TotalMoney));
                _context.Add(donhang);
                _context.SaveChanges();

                //Tao danh sach don hang
                foreach (var item in giohang)
                {
                    TblChiTietDonHang orderDetail = new TblChiTietDonHang();
                    orderDetail.IdDonHang = donhang.IdDonHang;
                    orderDetail.IdSanPham = item.SanPham.IdSanPham;
                    orderDetail.ISoLuong = item.amount;
                    orderDetail.FTongTien = donhang.FTongTien.GetValueOrDefault();
                    _context.Add(orderDetail);
                }
                _context.SaveChanges();
                //Clear cart
                HttpContext.Session.Remove("GioHnag");
                _notyfService.Success("Đơn hàng đặt thành công");
                //cap nhat thong tin khach hang
                return RedirectToAction("Success");
            }
            catch
            {
                /*ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");*/
                ViewBag.GioHang = giohang;
                return View(model);
            }
            /*ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");*/
            ViewBag.GioHang = giohang;
            return View(model);
        }

        [Route("/dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var khachhang = _context.TblKhachHangs.AsNoTracking().SingleOrDefault(x => x.IdKhachHang == Convert.ToInt32(taikhoanID));
                var donhang = _context.TblDonHangs.Where(x => x.IdKhachHang == Convert.ToInt32(taikhoanID)).OrderByDescending(x => x.DNgayTao);
                MuaHangThanhCong successVM = new MuaHangThanhCong();
                successVM.sHoTen = khachhang.STenKhachHang;

                successVM.sSDT = khachhang.SSdt;
                successVM.sDiaChi = khachhang.SDiaChi;

                return View(successVM);

            }
            catch
            {
                return View();
            }
        }
    }
}
