using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class TblDanhMuc
{
    public TblDanhMuc() 
    { 
        TblSanPhams = new HashSet<TblSanPham>();
    }
    public int IdDanhMuc { get; set; }

    public string? STenDanhMuc { get; set; }

    public string? SMoTa { get; set; }

    public bool BTrangThai { get; set; }

    public virtual ICollection<TblSanPham> TblSanPhams { get; set; } = new HashSet<TblSanPham>();
}
