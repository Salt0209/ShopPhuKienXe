using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblTrangThaiDonHang
{
    public TblTrangThaiDonHang() 
    {
        TblDonHangs = new HashSet<TblDonHang>();
    }
    public int IdTrangThai { get; set; }

    public string? STrangThai { get; set; }

    public string? SMoTa { get; set; }

    public virtual ICollection<TblDonHang> TblDonHangs { get; set; } = new List<TblDonHang>();
}
