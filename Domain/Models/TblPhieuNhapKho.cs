using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblPhieuNhapKho
{
    public TblPhieuNhapKho()
    {
        TblChiTietPhieuNhapKhos = new HashSet<TblChiTietPhieuNhapKho>();
    }
    public int IdPhieuNhapKho { get; set; }

    public int? IdNhanVien { get; set; }
    public int IdNhaCungCap { get; set; }

    public string? SGhiChu { get; set; }

    public bool BTrangThai { get; set; }

    public DateTime DNgaySua { get; set; }

    public virtual TblNhanVien? IdNhanVienNavigation { get; set; }
    public virtual TblNhaCungCap? IdNhaCungCapNavigation { get; set; }

    public virtual ICollection<TblChiTietPhieuNhapKho> TblChiTietPhieuNhapKhos { get; set; } = new List<TblChiTietPhieuNhapKho>();
}
