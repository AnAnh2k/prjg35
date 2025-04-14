using System.ComponentModel.DataAnnotations;

namespace Web_CuaHangCafe.ViewModels
{
    public class ProductViewModel
    {
        public int MaSanPham { get; set; }

        public string TenSanPham { get; set; } = null!;

        public decimal? GiaBan { get; set; }

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        public string? GhiChu { get; set; }
        //[StringLength(10, MinimumLength = 10, ErrorMessage = "VerifyKey phải có đúng 10 ký tự")]
        //[RegularExpression(@"^\d.*$", ErrorMessage = "VerifyKey phải bắt đầu bằng chữ số")]
        public string? VerifyKey { get; set; }

        public string LoaiSanPham { get; set; } = null!;
    }
}
