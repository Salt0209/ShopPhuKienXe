using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace WebAdminPKX.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminSanPhamController : Controller
    {
        private readonly WebPkxContext _context;

        public AdminSanPhamController(WebPkxContext context)
        {
            _context = context;
        }

        // GET: AdminSanPham
        public async Task<IActionResult> Index()
        {
            var webPkxContext = _context.TblSanPhams.Include(t => t.IdDanhMucNavigation);
            return View(await webPkxContext.ToListAsync());
        }
        [HttpGet]
        public IActionResult LoadProductByStatus(int status)
        {
            // Lấy danh sách đơn hàng dựa trên idTrangThai
            var sanPhams = _context.TblSanPhams
                .Include(t => t.IdDanhMucNavigation)
                .AsNoTracking();
            switch (status)
            {
                case 1: sanPhams = sanPhams.Where(t => t.BTrangThai == true)
                        .OrderByDescending(t=>t.DNgaySua);
                break;
                case 2: sanPhams = sanPhams.Where(t => t.BTrangThai == false)
                        .OrderByDescending(t=>t.DNgaySua);
                break;
        }

            // Trả về một phần tử HTML hoặc một PartialView chứa danh sách đơn hàng
            return PartialView("_ProductListPartial", sanPhams.ToList());
        }

        // GET: AdminSanPham/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblSanPham = await _context.TblSanPhams
                .Include(t => t.IdDanhMucNavigation)
                .FirstOrDefaultAsync(m => m.IdSanPham == id);
            if (tblSanPham == null)
            {
                return NotFound();
            }

            return View(tblSanPham);
        }

        // GET: AdminSanPham/Create
        public IActionResult Create()
        {
            ViewData["IdDanhMuc"] = new SelectList(_context.TblDanhMucs, "IdDanhMuc", "STenDanhMuc");
            return View();
        }

        // POST: AdminSanPham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSanPham,STenSanPham,SMoTa,SMoTaChiTiet,IdDanhMuc,FGiaTien,DNgayTao,DNgaySua,BTrangThai,Alias,ISoLuongTonKho")] TblSanPham tblSanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblSanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDanhMuc"] = new SelectList(_context.TblDanhMucs, "IdDanhMuc", "IdDanhMuc", tblSanPham.IdDanhMuc);
            return View(tblSanPham);
        }

        // GET: AdminSanPham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblSanPham = await _context.TblSanPhams.FindAsync(id);
            if (tblSanPham == null)
            {
                return NotFound();
            }
            ViewData["IdDanhMuc"] = new SelectList(_context.TblDanhMucs, "IdDanhMuc", "STenDanhMuc", tblSanPham.IdDanhMuc);
            return View(tblSanPham);
        }

        // POST: AdminSanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSanPham,STenSanPham,SMoTa,SMoTaChiTiet,IdDanhMuc,FGiaTien,DNgayTao,DNgaySua,BTrangThai,Alias,ISoLuongTonKho,SHinhAnh")] TblSanPham tblSanPham, IFormFile? SHinhAnh)
        {
            if (id != tblSanPham.IdSanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (SHinhAnh != null && SHinhAnh.Length > 0)
                    {
                        // Đường dẫn lưu file ảnh
                        var filePath = Path.Combine("wwwroot/images/products/", SHinhAnh.FileName);

                        // Lưu file ảnh
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await SHinhAnh.CopyToAsync(stream);
                        }

                        // Lưu đường dẫn file ảnh vào tblSanPham.SHinhAnh
                        tblSanPham.SHinhAnh = SHinhAnh.FileName;
                    }
                    tblSanPham.DNgaySua = DateTime.Now;
                    if (tblSanPham.ISoLuongTonKho > 0)
                    {
                        tblSanPham.BTrangThai = true;
                    }
                    else
                    {
                        tblSanPham.BTrangThai = false;
                    }
                    _context.Update(tblSanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblSanPhamExists(tblSanPham.IdSanPham))
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
            ViewData["IdDanhMuc"] = new SelectList(_context.TblDanhMucs, "IdDanhMuc", "IdDanhMuc", tblSanPham.IdDanhMuc);
            return View(tblSanPham);
        }

        public IActionResult EditTrangThai(int id)
        {
            TblSanPham sanPham = _context.TblSanPhams.FirstOrDefault(x => x.IdSanPham == id);
            sanPham.BTrangThai = !sanPham.BTrangThai;
            try
            {
                _context.Update(sanPham);
                _context.SaveChanges();
                var response = new
                {
                    success = true,
                    message = "Sửa trạng thái thành công về " + sanPham.BTrangThai,
                    trangThai = sanPham.BTrangThai 
                };

                // Trả về phản hồi JSON
                return Json(response);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { success = false, message = "Sửa trạng thái lỗi: "+ sanPham.BTrangThai });
            }
        }

        // GET: AdminSanPham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblSanPham = await _context.TblSanPhams
                .Include(t => t.IdDanhMucNavigation)
                .FirstOrDefaultAsync(m => m.IdSanPham == id);
            if (tblSanPham == null)
            {
                return NotFound();
            }

            return View(tblSanPham);
        }

        // POST: AdminSanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblSanPham = await _context.TblSanPhams.FindAsync(id);
            if (tblSanPham != null)
            {
                _context.TblSanPhams.Remove(tblSanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblSanPhamExists(int id)
        {
            return _context.TblSanPhams.Any(e => e.IdSanPham == id);
        }
    }
}
