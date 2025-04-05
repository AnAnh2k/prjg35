// File: Models/ViewModels/EmployeeAccountViewModel.cs
using Web_CuaHangCafe.Models;

namespace Web_CuaHangCafe.Areas.Admin.ViewModels
{
    public class EmployeeAccountViewModel
    {
        public TbNhanVien Employee { get; set; } = new TbNhanVien();
        public TbTaiKhoan Account { get; set; } = new TbTaiKhoan();
    }
}
