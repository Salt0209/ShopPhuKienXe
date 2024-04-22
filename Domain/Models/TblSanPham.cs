using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblSanPham
{
    public TblSanPham() 
    {
        TblChiTietPhieuNhapKhos = new HashSet<TblChiTietPhieuNhapKho>();
    }
    public int IdSanPham { get; set; }

    public string? STenSanPham { get; set; }

    public string? SMoTa { get; set; }

    public string? SMoTaChiTiet { get; set; }

    public int? IdDanhMuc { get; set; }

    public int? FGiaTien { get; set; }

    public string? SHinhAnh { get; set; }

    public DateTime? DNgayTao { get; set; }

    public DateTime? DNgaySua { get; set; }

    public bool BTrangThai { get; set; }

    public string? Alias { get; set; }

    public int? ISoLuongTonKho { get; set; }

    public virtual TblDanhMuc? IdDanhMucNavigation { get; set; }
    public virtual ICollection<TblChiTietPhieuNhapKho> TblChiTietPhieuNhapKhos { get; set; } = new List<TblChiTietPhieuNhapKho>();
}
