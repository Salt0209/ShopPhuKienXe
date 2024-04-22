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
    public class AdminQuyenHanController : Controller
    {
        private readonly WebPkxContext _context;

        public AdminQuyenHanController(WebPkxContext context)
        {
            _context = context;
        }

        // GET: AdminQuyenHan
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblQuyenHans.ToListAsync());
        }

        // GET: AdminQuyenHan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblQuyenHan = await _context.TblQuyenHans
                .FirstOrDefaultAsync(m => m.IdQuyenHan == id);
            if (tblQuyenHan == null)
            {
                return NotFound();
            }

            return View(tblQuyenHan);
        }

        // GET: AdminQuyenHan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminQuyenHan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdQuyenHan,STenQuyenHan,SMoTa,IMucDoSuDung")] TblQuyenHan tblQuyenHan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblQuyenHan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblQuyenHan);
        }

        // GET: AdminQuyenHan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblQuyenHan = await _context.TblQuyenHans.FindAsync(id);
            if (tblQuyenHan == null)
            {
                return NotFound();
            }
            return View(tblQuyenHan);
        }

        // POST: AdminQuyenHan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdQuyenHan,STenQuyenHan,SMoTa,IMucDoSuDung")] TblQuyenHan tblQuyenHan)
        {
            if (id != tblQuyenHan.IdQuyenHan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblQuyenHan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblQuyenHanExists(tblQuyenHan.IdQuyenHan))
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
            return View(tblQuyenHan);
        }

        // GET: AdminQuyenHan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblQuyenHan = await _context.TblQuyenHans
                .FirstOrDefaultAsync(m => m.IdQuyenHan == id);
            if (tblQuyenHan == null)
            {
                return NotFound();
            }

            return View(tblQuyenHan);
        }

        // POST: AdminQuyenHan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblQuyenHan = await _context.TblQuyenHans.FindAsync(id);
            if (tblQuyenHan != null)
            {
                _context.TblQuyenHans.Remove(tblQuyenHan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblQuyenHanExists(int id)
        {
            return _context.TblQuyenHans.Any(e => e.IdQuyenHan == id);
        }
    }
}
