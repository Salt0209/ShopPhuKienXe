using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Domain.Models;
using WebAdminPKX.ModelViews;

namespace WebAdminPKX.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminPhieuNhapKhoController : Controller
    {
        private readonly WebPkxContext _context;
        public INotyfService _notyfService { get; }

        public AdminPhieuNhapKhoController(WebPkxContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: AdminPhieuNhapKho
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var webPkxContext = _context.TblPhieuNhapKhos.Include(t => t.IdNhanVienNavigation).Include(t => t.IdNhaCungCapNavigation)
                .AsNoTracking().OrderByDescending(t => t.DNgaySua);
            PagedList<TblPhieuNhapKho> models = new PagedList<TblPhieuNhapKho>(webPkxContext, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }

        // GET: AdminPhieuNhapKho/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblPhieuNhapKho = await _context.TblPhieuNhapKhos
                .Include(t => t.IdNhanVienNavigation)
                .Include(p => p.TblChiTietPhieuNhapKhos)
                .FirstOrDefaultAsync(m => m.IdPhieuNhapKho == id);
            if (tblPhieuNhapKho == null)
            {
                return NotFound();
            }
            var ChiTietNhapKho = _context.TblChiTietPhieuNhapKhos

                .AsNoTracking().Where(x => x.IdPhieuNhapKho == tblPhieuNhapKho.IdPhieuNhapKho).OrderBy(x => x.IdChiTietPhieuNhapKho).ToList();
            ViewBag.ChiTiet = ChiTietNhapKho;

            return View(tblPhieuNhapKho);
        }

        // GET: AdminPhieuNhapKho/Create
        public IActionResult Create()
        {
            ViewData["IdNhanVien"] = new SelectList(_context.TblNhanViens, "IdNhanVien", "IdNhanVien");
            ViewData["IdNhaCungCap"] = new SelectList(_context.TblNhaCungCaps, "IdNhaCungCap", "STenNhaCungCap");
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem { Text = "Mở", Value = "true" },
                new SelectListItem { Text = "Khoá", Value = "false" }
            };

            ViewBag.TrangThai = new SelectList(items, "Value", "Text");
            return View();
        }

        // POST: AdminPhieuNhapKho/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPhieuNhapKho,IdNhanVien,IdNhaCungCap,SGhiChu,BTrangThai,DNgaySua")] TblPhieuNhapKho tblPhieuNhapKho)
        {
            if (ModelState.IsValid)
            {
                tblPhieuNhapKho.BTrangThai = false;
                _context.Add(tblPhieuNhapKho);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdNhanVien"] = new SelectList(_context.TblNhanViens, "IdNhanVien", "IdNhanVien", tblPhieuNhapKho.IdNhanVien);
            ViewData["IdNhaCungCap"] = new SelectList(_context.TblNhanViens, "IdNhanVien", "IdNhanVien", tblPhieuNhapKho.IdNhanVien);
            return View(tblPhieuNhapKho);
        }

        // GET: AdminPhieuNhapKho/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblPhieuNhapKho = await _context.TblPhieuNhapKhos.FindAsync(id);
            if (tblPhieuNhapKho == null)
            {
                return NotFound();
            }


            if (tblPhieuNhapKho.BTrangThai == true)
            {
                _notyfService.Warning("Phiếu đã khoá");
                return RedirectToAction("Index");
            }

            ViewData["IdNhanVien"] = new SelectList(_context.TblNhanViens, "IdNhanVien", "SHoTen", tblPhieuNhapKho.IdNhanVien);
            ViewData["IdNhaCungCap"] = new SelectList(_context.TblNhaCungCaps, "IdNhaCungCap", "STenNhaCungCap", tblPhieuNhapKho.IdNhaCungCap);
            List<SelectListItem> items = new List<SelectListItem>
    {
        new SelectListItem { Text = "Khoá", Value = "true" },
        new SelectListItem { Text = "Mở", Value = "false" }
    };

            ViewBag.TrangThai = new SelectList(items, "Value", "Text");

            var ChiTietNhapKho = _context.TblChiTietPhieuNhapKhos

                .AsNoTracking().Where(x => x.IdPhieuNhapKho == tblPhieuNhapKho.IdPhieuNhapKho).OrderBy(x => x.IdChiTietPhieuNhapKho).ToList();
            ViewBag.ChiTiet = ChiTietNhapKho;
            return View(tblPhieuNhapKho);
        }

        // POST: AdminPhieuNhapKho/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPhieuNhapKho,IdNhanVien,IdNhaCungCap,SGhiChu,BTrangThai,DNgaySua")] TblPhieuNhapKho tblPhieuNhapKho)
        {
            if (id != tblPhieuNhapKho.IdPhieuNhapKho)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblPhieuNhapKho);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblPhieuNhapKhoExists(tblPhieuNhapKho.IdPhieuNhapKho))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdNhanVien"] = new SelectList(_context.TblNhanViens, "IdNhanVien", "IdNhanVien", tblPhieuNhapKho.IdNhanVien);
            ViewData["IdNhaCungCap"] = new SelectList(_context.TblNhaCungCaps, "IdNhaCungCap", "IdNhaCungCap", tblPhieuNhapKho.IdNhaCungCap);
            return View(tblPhieuNhapKho);
        }

        // GET: AdminPhieuNhapKho/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblPhieuNhapKho = await _context.TblPhieuNhapKhos
                .Include(t => t.IdNhanVienNavigation)
                .FirstOrDefaultAsync(m => m.IdPhieuNhapKho == id);
            if (tblPhieuNhapKho == null)
            {
                return NotFound();
            }

            return View(tblPhieuNhapKho);
        }

        // POST: AdminPhieuNhapKho/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblPhieuNhapKho = await _context.TblPhieuNhapKhos.FindAsync(id);
            if (tblPhieuNhapKho != null)
            {
                _context.TblPhieuNhapKhos.Remove(tblPhieuNhapKho);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblPhieuNhapKhoExists(int id)
        {
            return _context.TblPhieuNhapKhos.Any(e => e.IdPhieuNhapKho == id);
        }


        [HttpGet]
        public IActionResult ThemSanPham(int? id)
        {
            ViewData["IdSanPham"] = new SelectList(_context.TblSanPhams, "IdSanPham", "STenSanPham");
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSanPham([Bind("IdChiTietPhieuNhapKho,IdPhieuNhapKho,IdSanPham,ISoLuongNhap,FDonGiaNhap,SGhiChu")] TblChiTietPhieuNhapKho tblChiTietPhieuNhapKho)
        {
            if (ModelState.IsValid)
            {
                var id = tblChiTietPhieuNhapKho.IdSanPham;
                var updateProduct = _context.TblSanPhams.FirstOrDefault(p=> p.IdSanPham==id);
                updateProduct.ISoLuongTonKho += tblChiTietPhieuNhapKho.ISoLuongNhap;
                _context.Update(updateProduct);
                _context.Add(tblChiTietPhieuNhapKho);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", new { id = tblChiTietPhieuNhapKho.IdPhieuNhapKho });
            }
            ViewData["IdSanPham"] = new SelectList(_context.TblSanPhams, "IdSanPham", "STenSanPham");
            return View(tblChiTietPhieuNhapKho);
        }

        [HttpGet]
        public async Task<IActionResult> SuaSanPham(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblCTNhapKhos = await _context.TblChiTietPhieuNhapKhos.FindAsync(id);
            if (tblCTNhapKhos == null)
            {
                return NotFound();
            }
            ViewData["IdSanPham"] = new SelectList(_context.TblSanPhams, "IdSanPham", "STenSanPham");
            ViewBag.Id = id;
            return View(tblCTNhapKhos);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaSanPham(int id, [Bind("IdChiTietPhieuNhapKho,IdPhieuNhapKho,IdSanPham,ISoLuongNhap,FDonGiaNhap,SGhiChu")] TblChiTietPhieuNhapKho tblChiTietPhieuNhapKho)
        {
            if (id != tblChiTietPhieuNhapKho.IdChiTietPhieuNhapKho)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblChiTietPhieuNhapKho);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblPhieuNhapKhoExists(tblChiTietPhieuNhapKho.IdChiTietPhieuNhapKho))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", new { id = tblChiTietPhieuNhapKho.IdPhieuNhapKho });
            }
            ViewData["IdSanPham"] = new SelectList(_context.TblSanPhams, "IdSanPham", "STenSanPham");
            return View(tblChiTietPhieuNhapKho);
        }
        public async Task<IActionResult> XoaSanPham(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblChiTietPhieuNhapKhos = await _context.TblChiTietPhieuNhapKhos
                .Include(t=>t.IdSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.IdChiTietPhieuNhapKho == id);
            if (tblChiTietPhieuNhapKhos == null)
            {
                return NotFound();
            }
            return View(tblChiTietPhieuNhapKhos);
        }

        // POST: AdminPhieuNhapKho/Delete/5
        [HttpPost, ActionName("XoaSanPham")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaSanPhamConfirmed(int id, [Bind("IdPhieuNhapKho")] TblChiTietPhieuNhapKho idPhieu)
        {
            var tblChiTietPhieuNhapKhos = await _context.TblChiTietPhieuNhapKhos.FindAsync(id);
            if (tblChiTietPhieuNhapKhos != null)
            {
                _context.TblChiTietPhieuNhapKhos.Remove(tblChiTietPhieuNhapKhos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = idPhieu.IdPhieuNhapKho });
        }
    }
}
