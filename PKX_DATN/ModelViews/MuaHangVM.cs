using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PKX_DATN.ModelViews
{
    public class MuaHangVM
    {
        [Key]
        public int ID_KhachHang { get; set; }

        public string sHoTen { get; set; }
        public string sEmail { get; set; }

        public string sSDT { get; set; }

        public string sDiaChi { get; set; }

        public string sGhiChu { get; set; }
    }
}
