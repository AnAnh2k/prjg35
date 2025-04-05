namespace Web_CuaHangCafe.ViewModels
{
    public class CheckoutViewModel
    {
        public IEnumerable<Web_CuaHangCafe.Models.TbGioHang> CartItems { get; set; }
        public Web_CuaHangCafe.Models.TbKhachHang Customer { get; set; }
        public string Total { get; set; }
    }
}
