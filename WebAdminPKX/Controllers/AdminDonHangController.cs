using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Domain.Models;
using static NuGet.Packaging.PackagingConstants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebAdminPKX.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminDonHangController : Controller
    {
        private readonly WebPkxContext _context;

        public AdminDonHangController(WebPkxContext context)
        {
            _context = context;
        }

        // GET: AdminDonHang
        public IActionResult Index(int? page, int? status)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var donHangs = _context.TblDonHangs.Include(t => t.IdKhachHangNavigation).Include(t => t.IdTrangThaiNavigation)
                .AsNoTracking().OrderByDescending(t=>t.DNgayTao);
            if (status != null)
            {
                donHangs = donHangs.Where(t => t.IdTrangThai == status)
                                   .OrderByDescending(t => t.DNgayTao);
            }
            PagedList<TblDonHang> models = new PagedList<TblDonHang>(donHangs, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            var trangthai = _context.TblTrangThaiDonHangs.AsNoTracking().ToList();
            ViewBag.TrangThai = trangthai;
            return View(models);
        }
        [HttpGet]
        public IActionResult LoadProductByStatus(int idTrangThai)
        {

            // Lấy danh sách đơn hàng dựa trên idTrangThai
            var donHangs = _context.TblDonHangs
                .Include(t => t.IdKhachHangNavigation)
                .Include(t => t.IdTrangThaiNavigation)
                .AsNoTracking();
            if(idTrangThai != 0)
            {
                donHangs = donHangs.Where(t => t.IdTrangThai == idTrangThai)
                    .OrderByDescending(t=>t.DNgayTao);
            }

            // Trả về một phần tử HTML hoặc một PartialView chứa danh sách đơn hàng
            return PartialView("_OrderListPartial", donHangs.ToList());
        }

        // GET: AdminDonHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblDonHang = await _context.TblDonHangs
                .Include(t => t.IdKhachHangNavigation)
                .Include(t => t.IdTrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdDonHang == id);
            if (tblDonHang == null)
            {
                return NotFound();
            }
            var ChiTietDonHang = _context.TblChiTietDonHangs

                .AsNoTracking().Where(x => x.IdDonHang == tblDonHang.IdDonHang).OrderBy(x => x.IdChiTietDonHang).ToList();
            ViewBag.ChiTiet = ChiTietDonHang;

            return View(tblDonHang);
        }

        // GET: AdminDonHang/Create
        public IActionResult Create()
        {
            ViewData["IdKhachHang"] = new SelectList(_context.TblKhachHangs, "IdKhachHang", "IdKhachHang");
            ViewData["IdTrangThai"] = new SelectList(_context.TblTrangThaiDonHangs, "IdTrangThai", "STrangThai");
            return View();
        }

        // POST: AdminDonHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDonHang,IdKhachHang,IdTrangThai,SGhiChu,FPhiVanChuyen,FTongTien,SDiaChi,IdSanPham,DNgayTao")] TblDonHang tblDonHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblDonHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKhachHang"] = new SelectList(_context.TblKhachHangs, "IdKhachHang", "IdKhachHang", tblDonHang.IdKhachHang);
            ViewData["IdTrangThai"] = new SelectList(_context.TblTrangThaiDonHangs, "IdTrangThai", "IdTrangThai", tblDonHang.IdTrangThai);
            return View(tblDonHang);
        }

        // GET: AdminDonHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblDonHang = await _context.TblDonHangs.FindAsync(id);
            if (tblDonHang == null)
            {
                return NotFound();
            }
            ViewData["IdKhachHang"] = new SelectList(_context.TblKhachHangs, "IdKhachHang", "IdKhachHang");
            ViewData["IdTrangThai"] = new SelectList(_context.TblTrangThaiDonHangs, "IdTrangThai", "STrangThai");
            var ChiTietDonHang = _context.TblChiTietDonHangs

                .AsNoTracking().Where(x => x.IdDonHang == tblDonHang.IdDonHang).OrderBy(x => x.IdChiTietDonHang).ToList();
            ViewBag.ChiTiet = ChiTietDonHang;
            return View(tblDonHang);
        }

        // POST: AdminDonHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDonHang,IdKhachHang,IdTrangThai,SGhiChu,FPhiVanChuyen,FTongTien,SDiaChi,IdSanPham,DNgayTao")] TblDonHang tblDonHang)
        {
            if (id != tblDonHang.IdDonHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblDonHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblDonHangExists(tblDonHang.IdDonHang))
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
            ViewData["IdKhachHang"] = new SelectList(_context.Account, "IdKhachHang", "IdKhachHang", tblDonHang.IdKhachHang);
            ViewData["IdTrangThai"] = new SelectList(_context.TblTrangThaiDonHangs, "IdTrangThai", "IdTrangThai", tblDonHang.IdTrangThai);
            return View(tblDonHang);
        }

        public async Task<IActionResult> CapNhatTrangThai(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.TblDonHangs
                .Include(o => o.IdKhachHangNavigation)
                .Include(o => o.IdTrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdDonHang == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["TrangThai"] = new SelectList(_context.TblTrangThaiDonHangs, "IdTrangThai", "STrangThai", order.IdTrangThai);

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(int id, [Bind("IdDonHang,IdKhachHang,IdTrangThai,SGhiChu,FPhiVanChuyen,FTongTien,SDiaChi,IdSanPham,DNgayTao,IdTrangThai")] TblDonHang order)
        {
            if (id != order.IdDonHang)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var donhang = await _context.TblDonHangs.AsNoTracking().Include(x => x.IdKhachHangNavigation).FirstOrDefaultAsync(m => m.IdDonHang == id);
                    if (donhang != null)
                    {
                        donhang.IdTrangThai = order.IdTrangThai;
                    }
                    _context.Update(donhang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblDonHangExists(order.IdDonHang))
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
            ViewData["TrangThai"] = new SelectList(_context.TblTrangThaiDonHangs, "TransactStatusId", "Status", order.IdTrangThai);
            return View("Index", order);
        }

        // GET: AdminDonHang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblDonHang = await _context.TblDonHangs
                .Include(t => t.IdKhachHangNavigation)
                .Include(t => t.IdTrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdDonHang == id);
            if (tblDonHang == null)
            {
                return NotFound();
            }

            return View(tblDonHang);
        }

        // POST: AdminDonHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblDonHang = await _context.TblDonHangs.FindAsync(id);
            if (tblDonHang != null)
            {
                _context.TblDonHangs.Remove(tblDonHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblDonHangExists(int id)
        {
            return _context.TblDonHangs.Any(e => e.IdDonHang == id);
        }
    }
}
