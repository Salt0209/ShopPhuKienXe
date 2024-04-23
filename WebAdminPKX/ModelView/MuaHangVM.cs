using System.ComponentModel.DataAnnotations;

namespace WebAdminPKX.ModelView
{
    public class MuaHangVM
    {
        public int ID_KhachHang { get; set; }

        public string sHoTen { get; set; }
        public string sEmail { get; set; }

        public string sSDT { get; set; }

        public string sDiaChi { get; set; }

        public string sGhiChu { get; set; }
    }
}
