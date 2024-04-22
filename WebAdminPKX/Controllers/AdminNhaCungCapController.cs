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
    public class AdminNhaCungCapController : Controller
    {
        private readonly WebPkxContext _context;

        public AdminNhaCungCapController(WebPkxContext context)
        {
            _context = context;
        }

        // GET: AdminNhaCungCap
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblNhaCungCaps.ToListAsync());
        }

        // GET: AdminNhaCungCap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblNhaCungCap = await _context.TblNhaCungCaps
                .FirstOrDefaultAsync(m => m.IdNhaCungCap == id);
            if (tblNhaCungCap == null)
            {
                return NotFound();
            }

            return View(tblNhaCungCap);
        }

        // GET: AdminNhaCungCap/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminNhaCungCap/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNhaCungCap,STenNhaCungCap,SDiaChi")] TblNhaCungCap tblNhaCungCap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblNhaCungCap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblNhaCungCap);
        }

        // GET: AdminNhaCungCap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblNhaCungCap = await _context.TblNhaCungCaps.FindAsync(id);
            if (tblNhaCungCap == null)
            {
                return NotFound();
            }
            return View(tblNhaCungCap);
        }

        // POST: AdminNhaCungCap/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNhaCungCap,STenNhaCungCap,SDiaChi")] TblNhaCungCap tblNhaCungCap)
        {
            if (id != tblNhaCungCap.IdNhaCungCap)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblNhaCungCap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblNhaCungCapExists(tblNhaCungCap.IdNhaCungCap))
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
            return View(tblNhaCungCap);
        }

        // GET: AdminNhaCungCap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblNhaCungCap = await _context.TblNhaCungCaps
                .FirstOrDefaultAsync(m => m.IdNhaCungCap == id);
            if (tblNhaCungCap == null)
            {
                return NotFound();
            }

            return View(tblNhaCungCap);
        }

        // POST: AdminNhaCungCap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblNhaCungCap = await _context.TblNhaCungCaps.FindAsync(id);
            if (tblNhaCungCap != null)
            {
                _context.TblNhaCungCaps.Remove(tblNhaCungCap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblNhaCungCapExists(int id)
        {
            return _context.TblNhaCungCaps.Any(e => e.IdNhaCungCap == id);
        }
    }
}
