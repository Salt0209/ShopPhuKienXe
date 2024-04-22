using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PKX_DATN.Extension;
using PKX_DATN.ModelViews;

namespace PKX_DATN.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<SanPhamGioHang>>("GioHang");
            return View(cart);
        }
    }
}
