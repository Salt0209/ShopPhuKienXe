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
using WebAdminPKX.ModelView;
using Microsoft.AspNetCore.Http;
using WebAdminPKX.Extension;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Domain.Helpers;

namespace WebAdminPKX.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminDonHangController : Controller
    {
        private readonly WebPkxContext _context;
        public INotyfService _notyfService { get; }

        public AdminDonHangController(WebPkxContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public List<SanPhamGioHang> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<SanPhamGioHang>>("GioHang");
                if (gh == default(List<SanPhamGioHang>))
                {
                    gh = new List<SanPhamGioHang>();
                }
                return gh;
            }
        }

        // GET: AdminDonHang
        public IActionResult Index(int? page, int? status)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var donHangs = _context.TblDonHangs.Include(t => t.IdKhachHangNavigation).Include(t => t.IdTrangThaiNavigation)
                .AsNoTracking().OrderBy(t=>t.IdTrangThai);
            if (status != null)
            {
                donHangs = donHangs.Where(t => t.IdTrangThai == status)
                                   .OrderBy(t => t.IdTrangThai);
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
                .OrderBy(t=>t.IdTrangThai)
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
                .Include(x=>x.IdSanPhamNavigation)
                .AsNoTracking().Where(x => x.IdDonHang == tblDonHang.IdDonHang).OrderBy(x => x.IdChiTietDonHang).ToList();
            ViewBag.ChiTiet = ChiTietDonHang;

            return View(tblDonHang);
        }

        // GET: AdminDonHang/Create
        public IActionResult Create(int id)
        {
            var ttKH = _context.TblKhachHangs
                .FirstOrDefault(i=>i.IdKhachHang == id);
            ViewBag.KhachHang = ttKH;

            var chitietdh = _context.TblChiTietDonHangs
                .Include(i=>i.IdSanPhamNavigation)
                .AsNoTracking().Where(i=>i.IdDonHang == 1).ToList();
            ViewBag.ChiTiet = chitietdh;

            var don=_context.TblDonHangs.FirstOrDefault(i=>i.IdDonHang==1);
            return View(ttKH);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(int IdKhachHang)
        {
            //lay gio hang ra de xu ly
            var giohang = HttpContext.Session.Get<List<SanPhamGioHang>>("GioHang");

            try
            {
                //khoi tao don hang
                TblDonHang donhang = new TblDonHang();
                donhang.IdKhachHang = IdKhachHang;
                donhang.DNgayTao = DateTime.Now;
                donhang.IdTrangThai = 3; //don hang giao thành công          
                donhang.FTongTien = Convert.ToInt32(giohang.Sum(x => x.TotalMoney));
                _context.Add(donhang);
                _context.SaveChanges();

                //Tao danh sach don hang
                foreach (var item in giohang)
                {
                    TblChiTietDonHang orderDetail = new TblChiTietDonHang();
                    orderDetail.IdDonHang = donhang.IdDonHang;
                    orderDetail.IdSanPham = item.SanPham.IdSanPham;
                    orderDetail.ISoLuong = item.amount;
                    var gia = _context.TblSanPhams.AsNoTracking()
                        .FirstOrDefault(x=>x.IdSanPham==orderDetail.IdSanPham)
                        .FGiaTien.GetValueOrDefault(0);
                    orderDetail.FTongTien = item.amount * gia;
                    _context.Add(orderDetail);
                }
                _context.SaveChanges();
                //Clear cart
                HttpContext.Session.Remove("GioHang");
                _notyfService.Success("Đơn hàng đặt thành công");
                //cap nhat thong tin khach hang
                return RedirectToAction("Index","AdminDonHang");
            }
            catch
            {
                ViewBag.GioHang = giohang;
                var kh = _context.TblKhachHangs
                .FirstOrDefault(i => i.IdKhachHang == IdKhachHang);
                return View(kh);
            }
        }
        [HttpPost]
        public JsonResult RemoveOrder(int id)
        {
            try
            {
                var order = _context.TblChiTietDonHangs.Where(b => b.IdChiTietDonHang == id).FirstOrDefault();
                _context.TblChiTietDonHangs.Remove(order);
                _context.SaveChanges();

                var donhang = _context.TblDonHangs.Where(b=>b.IdDonHang == order.IdDonHang).FirstOrDefault();
                donhang.FTongTien -=order.FTongTien;
                _context.Update(donhang);
                _context.SaveChanges();

                return Json(new { code = 200, msg = "Xóa thành công sản phẩm" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 501, msg = "Xóa thất bại " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult DSCart()
        {
            try
            {
                var dsc = GioHang;

                return Json(new { code = 200, dsc = dsc, msg = "Lấy dữ liệu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy dữ liệu thất bại:" + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddToCart(int productID, int? amount)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Người dùng đã đăng nhập
                return Json(new { success = false, message = "Bạn chưa đăng nhập" }); ;
            }
            List<SanPhamGioHang> gioHang = GioHang;
            try
            {
                //them sp vao gio hang
                SanPhamGioHang item = GioHang.SingleOrDefault(p => p.SanPham.IdSanPham == productID);
                if (item != null)//da co --> capnhat so luong
                {
                    _notyfService.Information("Sản phẩm đã có ở danh sách");
                    return Json(new { success = true });

                }
                else
                {
                    TblSanPham hh = _context.TblSanPhams.SingleOrDefault(p => p.IdSanPham == productID);
                    item = new SanPhamGioHang
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        SanPham = hh
                    };
                    gioHang.Add(item);//them vao gio

                }
                //luu lai Session
                HttpContext.Session.Set<List<SanPhamGioHang>>("GioHang", gioHang);
                _notyfService.Success("Thêm sản phẩm thành công");
                int cartItemCount = gioHang.Count();
                return Json(new { success = true, itemCount = cartItemCount });
            }
            catch
            {
                return Json(new { success = false, msg = "Lỗi thực hiện thêm vào session" });
            }
        }
        [HttpPost]
        public ActionResult RemoveFromCart(int productID)
        {
            try
            {
                var cart = HttpContext.Session.Get<List<SanPhamGioHang>>("GioHang") ?? new List<SanPhamGioHang>();

                var itemToUpdate = cart.SingleOrDefault(item => item.SanPham.IdSanPham == productID);
                if (itemToUpdate != null)
                {
                    cart.Remove(itemToUpdate);
                }
                // luu lai session
                HttpContext.Session.Set<List<SanPhamGioHang>>("GioHang", cart);
                return Json(new { success = true ,msg="Xoá thành công"});
            }
            catch
            {
                return Json(new { success = false ,msg="Xoá thất bai"});
            }
        }

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
                .Include(x=>x.IdSanPhamNavigation)
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
            var check = _context.TblChiTietDonHangs
                .AsNoTracking().FirstOrDefault(i => i.IdDonHang == id);
            if (check != null)
            {
                _notyfService.Error("Đơn hàng có chứa sản phẩm ở trong!");
                return RedirectToAction(nameof(Index));
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
