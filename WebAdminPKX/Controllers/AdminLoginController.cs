using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAdminPKX.ModelViews;
using Microsoft.EntityFrameworkCore;

namespace WebAdminPKX.Areas.Admin.Controllers
{
    public class AdminLoginController : Controller
    {
        private readonly WebPkxContext _context;
        public INotyfService _notifyService { get; }
        public AdminLoginController(WebPkxContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        [HttpGet]
        public ActionResult AdminLogin()
        {
            //if() dang nhap thanh cong, thi khong cho vao trang nay

            //else

            return View();
        }
        //[Bind] 
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin(LoginViewModel model, string returnUrl = null)
        {
            // username = anet  
            // var user = new Users().GetUsers().Where(u => u.UserName == userModel.UserName).Where(u => u.Password == userModel.Password).SingleOrDefault();

            var user = _context.TblNhanViens.Include(u => u.IdQuyenHanNavigation).Where(u => u.SEmail == model.SEmail).Where(u => u.SPassword == model.SPassword).SingleOrDefault();


            if (user != null)
            {
                if (user.IdQuyenHan == 0)
                {
                    TempData["ErrorMessage"] = "Tài khoản bị khóa vui lòng lien hệ quản trị viên";
                    return Redirect("/AdminLogin/AdminLogin");
                }
                else
                {
                    var userClaims = new List<Claim>()
                    {
                        new Claim("FullName", user.SHoTen),
                        new Claim("Email", user.SEmail),
                        new Claim("Phone", user.SSdt),
                        new Claim("RoleId", user.IdQuyenHan.ToString()),
                        new Claim(ClaimTypes.Name, user.SHoTen.ToString()),
                        new Claim(ClaimTypes.Role, user.IdQuyenHanNavigation.STenQuyenHan),
                    };

                    var userIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });

                    // Đăng nhập và tạo phiên
                    await HttpContext.SignInAsync(userPrincipal);

                    // Lưu trữ thông tin về người dùng trong Session hoặc Cookie

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl); // Chuyển hướng đến returnUrl nếu tồn tại và là một URL local hợp lệ
                    }
                    else
                    {
                        return RedirectToAction("Index", "HomeAdmin"); // Chuyển hướng đến trang chính của ứng dụng nếu không có returnUrl hoặc returnUrl không hợp lệ
                    }
                }
            }
            TempData["ErrorMessage"] = "Sai thông tin tài khoản hoặc mật khẩu";
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult AccessDeny()
        {
            return View();
        }

        public ActionResult Getuser()
        {
            var a = HttpContext.User;

            string b = a.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            string e = a.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            string g = a.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            ViewBag.Gmail = b;
            ViewBag.Name = e;
            ViewBag.Role = g;
            return Content("abc");
        }
        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            ////SignOutAsync is Extension method for SignOut    
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ////Redirect to home page  


            _notifyService.Error("Đăng xuất thành công");

            return Redirect("/AdminLogin/AdminLogin");
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
