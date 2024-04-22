using System.Collections.Generic;
using Domain.Models;

namespace WebAdminPKX.ModelViews
{
    public class IndexViewModel
    {
        public List<TblSanPham> Products { get; set; }
        public List<TblDanhMuc> Categories { get; set; }
    }
}
