using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebAdminPKX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly WebPkxContext _context;
        public HomeController(WebPkxContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var tongDonHang = _context.TblDonHangs.Count();
            ViewBag.TongDonHang = tongDonHang;

            var donHangChuaDuyet = _context.TblDonHangs.Where(p => p.IdTrangThai == 2).Count();
            ViewBag.DonHangChuaDuyet = donHangChuaDuyet;

            var tongKH = _context.TblKhachHangs.Count();
            ViewBag.TongKH = tongKH;

            var tongSP = _context.TblChiTietDonHangs.Sum(ctdh => ctdh.ISoLuong);
            ViewBag.TongSP = tongSP;

            ViewBag.ChiTieu = 20;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
