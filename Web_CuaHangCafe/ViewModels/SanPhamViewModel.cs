namespace Web_CuaHangCafe.ViewModels
{
    public class SanPhamViewModel
    {
        
        public string TenSanPham { get; set; }

        public decimal GiaBan { get; set; }
        public string? MoTa { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? GhiChu { get; set; }
        public int MaNhomSp { get; set; }
    }

}
