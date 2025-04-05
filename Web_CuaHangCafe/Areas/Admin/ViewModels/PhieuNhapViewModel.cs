using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_CuaHangCafe.ViewModels
{
    public class PhieuNhapViewModel
    {
        [Display(Name = "Ngày nhập")]
        [DataType(DataType.Date)]
        public DateTime NgayNhap { get; set; } = DateTime.Now;

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; } = string.Empty;

        [Display(Name = "Nhân viên")]
        public int MaNhanVien { get; set; }

        [Display(Name = "Quán")]
        public int MaQuan { get; set; }

        [Display(Name = "Nhà cung cấp")]
        public int MaNhaCungCap { get; set; }

        public List<PhieuNhapChiTietViewModel> ChiTietNhap { get; set; } = new List<PhieuNhapChiTietViewModel>();
    }

    public class PhieuNhapChiTietViewModel
    {
        public int MaNguyenLieu { get; set; }
        public string TenNguyenLieu { get; set; } = string.Empty;

        [Display(Name = "Số lượng nhập")]
        public decimal SoLuongNhap { get; set; }

        [Display(Name = "Đơn giá nhập")]
        public decimal DonGiaNhap { get; set; }
    }
}
