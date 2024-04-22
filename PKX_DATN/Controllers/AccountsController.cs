using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PKX_DATN.Extension;
using Domain.Helpers;
using Domain.Models;
using PKX_DATN.ModelViews;

namespace PKX_DATN.Controllers
{
    public class AccountsController : Controller
    {
        private readonly WebPkxContext _context;
        public INotyfService _notyfService { get; }


        public AccountsController(WebPkxContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return Content("Index");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = _context.TblKhachHangs.AsNoTracking().SingleOrDefault(x => x.SSdt.ToLower() == Phone);
                if (khachhang != null)
                    return Json(data: "Số điện thoại:" + Phone + "đã được sử dụng");
                return Json(data: true);

            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.TblKhachHangs.AsNoTracking().SingleOrDefault(x => x.SEmail.ToLower() == Email);
                if (khachhang != null)
                    return Json(data: "Email:" + Email + "đã được sử dụng");
                return Json(data: true);

            }
            catch
            {
                return Json(data: true);
            }
        }
        [AllowAnonymous]
        public IActionResult Dashboard()
        { 
            //var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var AccountID = HttpContext.Session.GetString("CustomerId");
            if (AccountID != null)
            {
                var khachhang = _context.TblKhachHangs.AsNoTracking().SingleOrDefault(x => x.IdKhachHang == Convert.ToInt32(AccountID));
                if (khachhang != null)
                {
                    var lsDonHang = _context.TblDonHangs
                        .Include(x => x.IdTrangThaiNavigation)
                        .Include(x => x.IdKhachHangNavigation)
                        .AsNoTracking()
                        .Where(x => x.IdKhachHang == khachhang.IdKhachHang)
                        .OrderByDescending(x => x.DNgayTao).ToList();
                    ViewBag.DonHang = lsDonHang;
                    return View(khachhang);
                }
            }
            //ViewBag.GioHang = cart;
            return RedirectToAction("Login");
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("/dang-ky.html", Name = "DangKy")]
        public IActionResult DangKyAccount()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("/dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> DangKyAccount(DangKy Account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    TblKhachHang khachhang = new TblKhachHang
                    {
                        STenKhachHang = Account.FullName,
                        SSdt = Account.Phone.Trim().ToLower(),
                        SEmail = Account.Email.Trim().ToLower(),
                        SPassword = Account.Password.Trim(),
                    };
                    try
                    {
                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        //Luu Session Makh
                        HttpContext.Session.SetString("CustomerId", khachhang.IdKhachHang.ToString());
                        var AccountID = HttpContext.Session.GetString("CustomerId");
                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachhang.STenKhachHang),
                            new Claim("CustomerId", khachhang.IdKhachHang.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch
                    {
                        return View(Account);
                    }
                }
                else
                {
                    return View(Account);
                }
            }
            catch
            {
                return View(Account);
            }
        }

        [AllowAnonymous]
        [Route("/Accounts/Login", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var AccountID = HttpContext.Session.GetString("CustomerId");
            if (AccountID != null)
            {

                return RedirectToAction("Dashboard", "Accounts");

            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/Accounts/Login", Name = "DangNhap")]
        public async Task<IActionResult> Login(DangNhap customer, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);
                    var khachhang = _context.TblKhachHangs.AsNoTracking().SingleOrDefault(x => x.SEmail.Trim() == customer.UserName);
                    if (khachhang == null) return RedirectToAction("DangKyAccount");
                    string pass = customer.sPassword.Trim();
                    if (khachhang.SPassword != pass)
                    {
                        _notyfService.Success("Thông tin đăng nhập chưa chính xác");
                        return View(customer);
                    }
                    //luu session Makh
                    HttpContext.Session.SetString("CustomerId", khachhang.IdKhachHang.ToString());
                    var AccountID = HttpContext.Session.GetString("CustomerId");
                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, khachhang.STenKhachHang),
                        new Claim("CustomerId", khachhang.IdKhachHang.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("Dashboard", "Accounts");
                }
            }
            catch
            {
                return RedirectToAction("DangKyAccount", "Account");
            }
            return View(customer);
        }
        [Route("/dang-xuat.html", Name = "Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // Await the SignOutAsync method
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult ChangePassword(DoiMatKhau model)
        {
            try
            {
                var AccountID = HttpContext.Session.GetString("CustomerId");
                if (AccountID == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var Account = _context.TblKhachHangs.Find(Convert.ToInt32(AccountID));
                    if (Account == null) return RedirectToAction("Login", "Account");

                    var pass = model.PasswordNow.Trim();
                    if (pass == Account.SPassword)
                    {
                        string passnew = model.Password.Trim();
                        Account.SPassword = passnew;
                        _context.Update(Account);
                        _context.SaveChanges();
                        _notyfService.Success("Thay mật khẩu thành công");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
                return RedirectToAction("Dashboard", "Accounts");
            }
            catch
            {
                _notyfService.Success("Thay mật khẩu không thành công");
                return RedirectToAction("Dashboard", "Accounts");
            }
            _notyfService.Success("Thay mật khẩu không thành công");
            return RedirectToAction("Dashboard", "Accounts");
        }
    }
}
