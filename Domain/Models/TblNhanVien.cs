using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain.Models;

public partial class TblNhanVien
{
    public TblNhanVien() {
        TblPhieuNhapKhos = new HashSet<TblPhieuNhapKho>();
    }
    [DisplayName("Mã nhân viên")]
    public int IdNhanVien { get; set; }
    [DisplayName("Số điện thoại")]
    public string? SSdt { get; set; }
    [DisplayName("Email")]
    public string? SEmail { get; set; }
    [DisplayName("Mật khẩu")]
    public string? SPassword { get; set; }
    [DisplayName("Họ tên")]
    public string? SHoTen { get; set; }
    [DisplayName("Mã quyền")]
    public int? IdQuyenHan { get; set; }

    public virtual TblQuyenHan? IdQuyenHanNavigation { get; set; }


    public virtual ICollection<TblPhieuNhapKho> TblPhieuNhapKhos { get; set; } = new List<TblPhieuNhapKho>();
}
