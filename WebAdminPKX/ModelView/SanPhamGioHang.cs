using Domain.Models;

namespace WebAdminPKX.ModelView
{
    public class SanPhamGioHang
    {
        public TblSanPham SanPham { get; set; }
        public int amount { get; set; }
        public double TotalMoney => amount * SanPham.FGiaTien.Value;
    }
}
