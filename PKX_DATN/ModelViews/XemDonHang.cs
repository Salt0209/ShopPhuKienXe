using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace PKX_DATN.ModelViews
{
    public class XemDonHang
    {
        public TblDonHang DonHang { get; set; }
        public List<TblChiTietDonHang> ChiTietDonHang { get; set; }
    }
}
