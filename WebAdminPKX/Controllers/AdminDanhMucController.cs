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
    public class AdminDanhMucController : Controller
    {
        private readonly WebPkxContext _context;

        public AdminDanhMucController(WebPkxContext context)
        {
            _context = context;
        }

        // GET: AdminDanhMuc
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblDanhMucs.ToListAsync());
        }

        // GET: AdminDanhMuc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblDanhMuc = await _context.TblDanhMucs
                .FirstOrDefaultAsync(m => m.IdDanhMuc == id);
            if (tblDanhMuc == null)
            {
                return NotFound();
            }

            return View(tblDanhMuc);
        }

        // GET: AdminDanhMuc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminDanhMuc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDanhMuc,STenDanhMuc,SMoTa,BTrangThai")] TblDanhMuc tblDanhMuc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblDanhMuc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblDanhMuc);
        }

        // GET: AdminDanhMuc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblDanhMuc = await _context.TblDanhMucs.FindAsync(id);
            if (tblDanhMuc == null)
            {
                return NotFound();
            }
            return View(tblDanhMuc);
        }

        // POST: AdminDanhMuc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDanhMuc,STenDanhMuc,SMoTa,BTrangThai")] TblDanhMuc tblDanhMuc)
        {
            if (id != tblDanhMuc.IdDanhMuc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblDanhMuc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblDanhMucExists(tblDanhMuc.IdDanhMuc))
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
            return View(tblDanhMuc);
        }

        // GET: AdminDanhMuc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblDanhMuc = await _context.TblDanhMucs
                .FirstOrDefaultAsync(m => m.IdDanhMuc == id);
            if (tblDanhMuc == null)
            {
                return NotFound();
            }

            return View(tblDanhMuc);
        }

        // POST: AdminDanhMuc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblDanhMuc = await _context.TblDanhMucs.FindAsync(id);
            if (tblDanhMuc != null)
            {
                _context.TblDanhMucs.Remove(tblDanhMuc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblDanhMucExists(int id)
        {
            return _context.TblDanhMucs.Any(e => e.IdDanhMuc == id);
        }
    }
}
