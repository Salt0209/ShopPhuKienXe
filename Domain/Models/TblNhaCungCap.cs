using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain.Models;

public partial class TblNhaCungCap
{
    public TblNhaCungCap() 
    {
        TblPhieuNhapKhos = new HashSet<TblPhieuNhapKho>();
    }
    [DisplayName("Mã nhà cung cấp")]
    public int IdNhaCungCap { get; set; }
    [DisplayName("Tên nhà cung cấp")]
    public string STenNhaCungCap { get; set; } = null!;
    [DisplayName("Địa chỉ")]
    public string? SDiaChi { get; set; }

    public virtual ICollection<TblPhieuNhapKho> TblPhieuNhapKhos { get; set; } = new List<TblPhieuNhapKho>();

}
