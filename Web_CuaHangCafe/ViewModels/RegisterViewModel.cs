using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_CuaHangCafe.ViewModels
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
        [Remote(action: "CheckUserName", controller: "Access1", ErrorMessage = "Tên tài khoản đã tồn tại!")]
        public string TenTaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhau { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        public string TenKhachHang { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SDTKhachHang { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string DiaChi { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(MatKhau))
            {
                // Kiểm tra độ dài >= 6 ký tự
                if (MatKhau.Length < 6)
                {
                    yield return new ValidationResult("Ít nhất 6 ký tự", new[] { nameof(MatKhau) });
                }

                // Kiểm tra có ít nhất một chữ số (0-9)
                if (!MatKhau.Any(ch => char.IsDigit(ch)))
                {
                    yield return new ValidationResult("Có ít nhất một chữ số (0-9)", new[] { nameof(MatKhau) });
                }

                // Kiểm tra có ít nhất một chữ cái viết hoa (A-Z)
                if (!MatKhau.Any(ch => char.IsUpper(ch)))
                {
                    yield return new ValidationResult("Có ít nhất một chữ cái viết hoa (A-Z)", new[] { nameof(MatKhau) });
                }

                // Kiểm tra có ít nhất một ký tự đặc biệt (#$%^&*()!)
                string specialChars = "#$%^&*()!";
                if (!MatKhau.Any(ch => specialChars.Contains(ch)))
                {
                    yield return new ValidationResult("Có ít nhất một ký tự đặc biệt (#$%^&*()!)", new[] { nameof(MatKhau) });
                }
            }
        }
    }
}
