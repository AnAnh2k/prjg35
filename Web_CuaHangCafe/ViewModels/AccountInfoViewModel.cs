using Web_CuaHangCafe.Models;

namespace Web_CuaHangCafe.ViewModels
{
    public class AccountInfoViewModel
    {
        // Loại tài khoản: "Admin" hoặc "User"
        public string Role { get; set; } = string.Empty;

        // Thông tin tài khoản chung
        public string TenTaiKhoan { get; set; } = string.Empty;

        // Thông tin chi tiết: nếu admin, đây là thông tin nhân viên; nếu user, là thông tin khách hàng
        public TbNhanVien? NhanVienInfo { get; set; }
        public TbKhachHang? KhachHangInfo { get; set; }
        public string? MatKhau { get; internal set; }
    }
}
