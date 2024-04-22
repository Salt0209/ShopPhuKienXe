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
    [Authorize(Roles = "SuperAdmin")]
    public class AdminNhanVienController : Controller
    {
        private readonly WebPkxContext _context;

        public AdminNhanVienController(WebPkxContext context)
        {
            _context = context;
        }

        // GET: AdminNhanVien
        public async Task<IActionResult> Index()
        {
            var webPkxContext = _context.TblNhanViens.Include(t => t.IdQuyenHanNavigation);
            return View(await webPkxContext.ToListAsync());
        }

        // GET: AdminNhanVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblNhanVien = await _context.TblNhanViens
                .Include(t => t.IdQuyenHanNavigation)
                .FirstOrDefaultAsync(m => m.IdNhanVien == id);
            if (tblNhanVien == null)
            {
                return NotFound();
            }

            return View(tblNhanVien);
        }

        // GET: AdminNhanVien/Create
        public IActionResult Create()
        {
            ViewData["IdQuyenHan"] = new SelectList(_context.TblQuyenHans, "IdQuyenHan", "STenQuyenHan");
            return View();
        }

        // POST: AdminNhanVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNhanVien,SSdt,SEmail,SPassword,SHoTen,IdQuyenHan")] TblNhanVien tblNhanVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblNhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdQuyenHan"] = new SelectList(_context.TblQuyenHans, "IdQuyenHan", "STenQuyenHan", tblNhanVien.IdQuyenHan);
            return View(tblNhanVien);
        }

        // GET: AdminNhanVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblNhanVien = await _context.TblNhanViens.FindAsync(id);
            if (tblNhanVien == null)
            {
                return NotFound();
            }
            ViewData["IdQuyenHan"] = new SelectList(_context.TblQuyenHans, "IdQuyenHan", "STenQuyenHan", tblNhanVien.IdQuyenHan);
            return View(tblNhanVien);
        }

        // POST: AdminNhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNhanVien,SSdt,SEmail,SPassword,SHoTen,IdQuyenHan")] TblNhanVien tblNhanVien)
        {
            if (id != tblNhanVien.IdNhanVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblNhanVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblNhanVienExists(tblNhanVien.IdNhanVien))
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
            ViewData["IdQuyenHan"] = new SelectList(_context.TblQuyenHans, "IdQuyenHan", "STenQuyenHan", tblNhanVien.IdQuyenHan);
            return View(tblNhanVien);
        }

        // GET: AdminNhanVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblNhanVien = await _context.TblNhanViens
                .Include(t => t.IdQuyenHanNavigation)
                .FirstOrDefaultAsync(m => m.IdNhanVien == id);
            if (tblNhanVien == null)
            {
                return NotFound();
            }

            return View(tblNhanVien);
        }

        // POST: AdminNhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblNhanVien = await _context.TblNhanViens.FindAsync(id);
            if (tblNhanVien != null)
            {
                _context.TblNhanViens.Remove(tblNhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblNhanVienExists(int id)
        {
            return _context.TblNhanViens.Any(e => e.IdNhanVien == id);
        }
    }
}
