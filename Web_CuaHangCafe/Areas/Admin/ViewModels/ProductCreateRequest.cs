public class ProductCreateRequest
{
    public string TenSanPham { get; set; }
    public decimal GiaBan { get; set; }
    public int MaNhomSp { get; set; }
    public string MoTa { get; set; }
    public string GhiChu { get; set; }
    public IFormFile ImageFile { get; set; }
}