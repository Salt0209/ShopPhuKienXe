using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain.Models;

public partial class TblQuyenHan
{
    public TblQuyenHan() 
    {
        TblNhanViens = new HashSet<TblNhanVien>();
    }
    public int IdQuyenHan { get; set; }

    [DisplayName("Tên quyền hạn")]
    public string? STenQuyenHan { get; set; }

    [DisplayName("Mô tả")]
    public string? SMoTa { get; set; }

    [DisplayName("Mức độ sử dụng")]
    public int? IMucDoSuDung { get; set; }

    public virtual ICollection<TblNhanVien> TblNhanViens { get; set; } = new List<TblNhanVien>();
}
