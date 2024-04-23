using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblDonHang
{
    public TblDonHang() 
    {
        TblChiTietDonHangs = new HashSet<TblChiTietDonHang>();
    }
    public TblDonHang(int id,int trangthai)
    {
        IdTrangThai = trangthai;
    }
    public int IdDonHang { get; set; }

    public int? IdKhachHang { get; set; }

    public int IdTrangThai { get; set; }

    public string? SGhiChu { get; set; }

    public int? FPhiVanChuyen { get; set; }

    public int? FTongTien { get; set; }

    public string? SDiaChi { get; set; }

    public DateTime? DNgayTao { get; set; }

    public virtual TblKhachHang? IdKhachHangNavigation { get; set; }

    public virtual TblTrangThaiDonHang? IdTrangThaiNavigation { get; set; }

    public virtual ICollection<TblChiTietDonHang> TblChiTietDonHangs { get; set; } = new List<TblChiTietDonHang>();
}
