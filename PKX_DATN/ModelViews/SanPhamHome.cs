using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace PKX_DATN.ModelViews
{
    public class SanPhamHome
    {
        public TblDanhMuc DanhMuc { get; set; }
        public List<TblSanPham> lsSanPhams { get; set; }
    }
}
