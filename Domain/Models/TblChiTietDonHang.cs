using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblChiTietDonHang
{
    public int IdChiTietDonHang { get; set; }

    public int? IdDonHang { get; set; }

    public int? IdSanPham { get; set; }

    public int ISoLuong { get; set; }

    public int FTongTien { get; set; }

    public virtual TblDonHang? IdDonHangNavigation { get; set; }
    public virtual TblSanPham? IdSanPhamNavigation { get; set; }
}
