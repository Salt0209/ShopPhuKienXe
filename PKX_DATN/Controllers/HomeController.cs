using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using PKX_DATN.ModelViews;

namespace PKX_DATN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly WebPkxContext _context;
        public INotyfService _notifyService { get; }
        public HomeController(ILogger<HomeController> logger, WebPkxContext context, INotyfService notyf)
        {
            _logger = logger;
            _context = context;
            _notifyService = notyf;
        }

        public IActionResult Index(int? page, string searchString, string currentFilter, int? pageNumber)
        {
            HomeViewVM model = new HomeViewVM();

            //List<Product> lsProducts = new List<Product>();
            var lsSanPhams = _context.TblSanPhams.AsNoTracking()
                .Where(x => x.BTrangThai == true)
                .OrderByDescending(x => x.DNgayTao)
                .ToList();
            List<SanPhamHome> lsProductView = new List<SanPhamHome>();
            var lsDanhMucs = _context.TblDanhMucs
                .AsNoTracking()
                .Where(x => x.BTrangThai == true)
                .ToList();
            foreach (var item in lsDanhMucs)
            {
                SanPhamHome productHome = new SanPhamHome();
                productHome.DanhMuc = item;
                productHome.lsSanPhams = lsSanPhams.Where(x => x.IdDanhMuc == item.IdDanhMuc).ToList();
                lsProductView.Add(productHome);
            }

            ViewBag.AllProducts = lsSanPhams;

            return View(model);
        }


        public IActionResult DanhMuc(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 6;
            List<TblSanPham> lsProducts = new List<TblSanPham>();

            if (CatID != 0)
            {
                lsProducts = _context.TblSanPhams
                 .AsNoTracking()
                 .Where(x => x.IdDanhMuc == CatID)
                 .Include(x => x.IdDanhMuc)
                 .OrderByDescending(x => x.IdSanPham).ToList();
            }
            else
            {
                lsProducts = _context.TblSanPhams
                .AsNoTracking()
                .Include(x => x.IdDanhMuc)
                .OrderByDescending(x => x.IdSanPham).ToList();
            }
            var lsCats = _context.TblDanhMucs
                .AsNoTracking()
                .Where(x => x.BTrangThai == true)
                .ToList();
            PagedList<TblSanPham> models = new PagedList<TblSanPham>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;

            ViewData["DanhMuc"] = new SelectList(_context.TblDanhMucs, "CatId", "CatName", CatID);
            return View(models);
        }
        public IActionResult Filtter(int CatID = 0)
        {
            var url = $"/Admin/AdminProducts?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/AdminProducts";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
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
