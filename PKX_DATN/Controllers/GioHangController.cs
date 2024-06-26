﻿using AspNetCoreHero.ToastNotification.Abstractions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PKX_DATN.Extension;
using Domain.Models;
using PKX_DATN.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace PKX_DATN.Controllers
{
    [Authorize]
    public class GioHangController : Controller
    {
        private readonly WebPkxContext _context;
        public INotyfService _notyfService { get; }


        public GioHangController(WebPkxContext context, INotyfService notyfService)
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

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Người dùng đã đăng nhập
                return Json(new { success = false, message="Bạn chưa đăng nhập" }) ; ;
            }
            List<SanPhamGioHang> gioHang = GioHang;
            try
            {
                //them sp vao gio hang
                SanPhamGioHang item = GioHang.SingleOrDefault(p => p.SanPham.IdSanPham == productID);
                if (item != null)//da co --> capnhat so luong
                {
                    _notyfService.Information("Sản phẩm đã có trong giỏ hàng");
                    return Json(new { success = true });

                }
                else
                {
                    TblSanPham hh = _context.TblSanPhams.SingleOrDefault(p => p.IdSanPham == productID);
                    item = new SanPhamGioHang
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        SanPham = hh
                    };
                    gioHang.Add(item);//them vao gio

                }
                //luu lai Session
                HttpContext.Session.Set<List<SanPhamGioHang>>("GioHang", gioHang);
                _notyfService.Success("Thêm sản phẩm thành công");
                int cartItemCount = gioHang.Count();
                return Json(new { success = true, itemCount = cartItemCount });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        public IActionResult UpdateCart(int productId, int? amount)
        {
            var cart = HttpContext.Session.Get<List<SanPhamGioHang>>("GioHang") ?? new List<SanPhamGioHang>();

            SanPhamGioHang itemToUpdate = cart.SingleOrDefault(item => item.SanPham.IdSanPham == productId);
            if (itemToUpdate != null)
            {
                if (itemToUpdate != null && amount.HasValue)
                {
                    if (amount == 0)
                    {
                        cart.Remove(itemToUpdate);
                    }
                    itemToUpdate.amount += amount.Value;
                }
            }

            HttpContext.Session.Set<List<SanPhamGioHang>>("GioHang", cart);

            return Json(new { success = true });
        }


        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {
            try
            {
                var cart = HttpContext.Session.Get<List<SanPhamGioHang>>("GioHang") ?? new List<SanPhamGioHang>();

                var itemToUpdate = cart.SingleOrDefault(item => item.SanPham.IdSanPham == productID);
                if (itemToUpdate != null)
                {
                    cart.Remove(itemToUpdate);
                }
                // luu lai session
                HttpContext.Session.Set<List<SanPhamGioHang>>("GioHang", cart);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [Route("/cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
        [HttpGet]
        public JsonResult TrangGioHang(int id)
        {
            if (id != 0)
            {
                var tcn = _context.TblChiTietDonHangs
                .Where(t => t.IdDonHang == id)
                .Select(t=> new
                {
                    idSanPham = t.IdSanPham,
                    tenSanPham = t.IdSanPhamNavigation.STenSanPham,
                    iSoLuong = t.ISoLuong,
                    fTongTien = t.FTongTien
                })
                .AsNoTracking().ToList();
                return Json(new { code = 200, tcn = tcn, msg = "thành công" });
            }
            else
            {
                return Json(new { code = 500, msg = "thất bại" });
            }
        }
    }
}
