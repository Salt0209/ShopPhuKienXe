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
    public class AdminKhachHangController : Controller
    {
        private readonly WebPkxContext _context;

        public AdminKhachHangController(WebPkxContext context)
        {
            _context = context;
        }

        // GET: AdminKhachHang
        public async Task<IActionResult> Index()
        {
            return View(await _context.Account.ToListAsync());
        }

        // GET: AdminKhachHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblKhachHang = await _context.Account
                .FirstOrDefaultAsync(m => m.IdKhachHang == id);
            if (tblKhachHang == null)
            {
                return NotFound();
            }

            return View(tblKhachHang);
        }

        // GET: AdminKhachHang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminKhachHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKhachHang,STenKhachHang,SDiaChi,SEmail,SSdt,SPassword")] TblKhachHang tblKhachHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblKhachHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblKhachHang);
        }

        // GET: AdminKhachHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblKhachHang = await _context.Account.FindAsync(id);
            if (tblKhachHang == null)
            {
                return NotFound();
            }
            return View(tblKhachHang);
        }

        // POST: AdminKhachHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKhachHang,STenKhachHang,SDiaChi,SEmail,SSdt,SPassword")] TblKhachHang tblKhachHang)
        {
            if (id != tblKhachHang.IdKhachHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblKhachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblKhachHangExists(tblKhachHang.IdKhachHang))
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
            return View(tblKhachHang);
        }

        // GET: AdminKhachHang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblKhachHang = await _context.Account
                .FirstOrDefaultAsync(m => m.IdKhachHang == id);
            if (tblKhachHang == null)
            {
                return NotFound();
            }

            return View(tblKhachHang);
        }

        // POST: AdminKhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblKhachHang = await _context.Account.FindAsync(id);
            if (tblKhachHang != null)
            {
                _context.Account.Remove(tblKhachHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblKhachHangExists(int id)
        {
            return _context.Account.Any(e => e.IdKhachHang == id);
        }
    }
}
