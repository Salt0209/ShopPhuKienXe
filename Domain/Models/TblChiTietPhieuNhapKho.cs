using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblChiTietPhieuNhapKho
{
    public int IdChiTietPhieuNhapKho { get; set; }

    public int? IdPhieuNhapKho { get; set; }
    public int? IdSanPham { get; set; }

    public int ISoLuongNhap { get; set; }

    public int FDonGiaNhap { get; set; }

    public string? SGhiChu { get; set; }

    public virtual TblPhieuNhapKho? IdPhieuNhapKhoNavigation { get; set; }
    public virtual TblSanPham? IdSanPhamNavigation { get; set; }
}
