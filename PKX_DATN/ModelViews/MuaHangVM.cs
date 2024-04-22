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

        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        public string sHoTen { get; set; }
        public string sEmail { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string sSDT { get; set; }
        [Required(ErrorMessage = "Địa chỉ nhận hàng")]
        public string sDiaChi { get; set; }
       
        public string sGhiChu { get; set; }
    }
}
