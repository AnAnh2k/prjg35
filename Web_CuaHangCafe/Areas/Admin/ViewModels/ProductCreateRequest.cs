using System.ComponentModel.DataAnnotations;

public class ProductCreateRequest
{
    public string TenSanPham { get; set; }
    public decimal GiaBan { get; set; }
    public int MaNhomSp { get; set; }
    public string MoTa { get; set; }
    public string GhiChu { get; set; }
    //[StringLength(10, MinimumLength = 10, ErrorMessage = "VerifyKey phải có đúng 10 ký tự")]
    //[RegularExpression(@"^\d.*$", ErrorMessage = "VerifyKey phải bắt đầu bằng chữ số")]
    public string VerifyKey { get; set; }
    public IFormFile ImageFile { get; set; }
}