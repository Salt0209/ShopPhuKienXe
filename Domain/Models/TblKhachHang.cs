using System.ComponentModel;

namespace Domain.Models;

public partial class TblKhachHang
{
    public TblKhachHang() 
    {
        TblDonHangs = new HashSet<TblDonHang>();
    }
    [DisplayName("Mã khách hàng")]
    public int IdKhachHang { get; set; }
    [DisplayName("Tên khách hàng")]
    public string? STenKhachHang { get; set; }
    [DisplayName("Địa chỉ")]
    public string? SDiaChi { get; set; }
    [DisplayName("Email")]
    public string? SEmail { get; set; }
    [DisplayName("Số điện thoại")]
    public string? SSdt { get; set; }
    [DisplayName("Mật khẩu")]
    public string? SPassword { get; set; }

    public virtual ICollection<TblDonHang> TblDonHangs { get; set; } = new List<TblDonHang>();
}
