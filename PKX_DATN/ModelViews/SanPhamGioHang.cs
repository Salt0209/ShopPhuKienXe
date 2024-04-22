using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace PKX_DATN.ModelViews
{
    public class SanPhamGioHang
    {
        public TblSanPham SanPham { get; set; }
        public int amount { get; set; }
        public double TotalMoney => amount * SanPham.FGiaTien.Value;
    }
}
