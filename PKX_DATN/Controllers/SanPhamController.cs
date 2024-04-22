using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Domain.Models;

namespace PKX_DATN.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly WebPkxContext _context;

        public SanPhamController(WebPkxContext context)
        {
            _context = context;
        }
        [Route("/shop.html", Name = "ShopProduct")]
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
        {

            ViewData["CurrentFilter"] = searchString;
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var p = from s in _context.TblSanPhams.Where(p=>p.BTrangThai==true)
                    select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                p = p.Where(s=>s.BTrangThai==true).Where(s => s.STenSanPham.Contains(searchString)
                                       || s.FGiaTien.ToString().Contains(searchString));
            }
            int pageSize = 6;

            var lsDanhMuc = _context.TblDanhMucs.ToList();
            ViewBag.DanhMuc = lsDanhMuc;
            return View(await PaginatedList<TblSanPham>.CreateAsync(p.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        [Route("/{CatID}", Name = "ListProduct")]
        public IActionResult List(int CatID, int page = 1)
        {
            try
            {
                var pageSize = 20;
                var danhmuc = _context.TblDanhMucs.AsNoTracking().SingleOrDefault(x => x.IdDanhMuc == CatID);
                if (danhmuc != null)
                {
                    var lsTinTuc = _context.TblSanPhams.AsNoTracking()
                  .Where(x => x.IdDanhMuc == danhmuc.IdDanhMuc)
                  .Where(x=>x.BTrangThai == true)
                  .OrderByDescending(x => x.DNgayTao);
                    PagedList<TblSanPham> models = new PagedList<TblSanPham>(lsTinTuc, page, pageSize);
                    ViewBag.CurrentPage = page;
                    ViewBag.CurrentCat = danhmuc;
                    var lsDanhMuc = _context.TblDanhMucs.ToList();
                    ViewBag.DanhMuc = lsDanhMuc;
                    return View(models);
                }
                return RedirectToAction("Index", "Home");


            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }


        }


        [Route("/{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.TblSanPhams.Include(x => x.IdDanhMucNavigation).FirstOrDefault(x => x.IdSanPham == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }

                var lsProduct = _context.TblSanPhams
                    .AsNoTracking()
                    .Where(x => x.IdDanhMuc == product.IdDanhMuc && x.IdSanPham != id && x.BTrangThai == true)
                    .OrderByDescending(x => x.DNgayTao)
                    .Take(4)
                    .ToList();
                ViewBag.SanPham = lsProduct;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }


        }
    }
}
