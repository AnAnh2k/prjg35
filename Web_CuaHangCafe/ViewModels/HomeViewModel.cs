using Web_CuaHangCafe.Models;
using System.Collections.Generic;

namespace Web_CuaHangCafe.ViewModels
{
    public class HomeViewModel
    {
        public List<TbSanPham> lstSanPham { get; set; } = new List<TbSanPham>();
        public List<TbTinTuc> lstTinTuc { get; set; } = new List<TbTinTuc>();

        // Thêm giỏ hàng từ db (tbGioHang)
        public List<TbGioHang> CartItems { get; set; } = new List<TbGioHang>();
    }
}
