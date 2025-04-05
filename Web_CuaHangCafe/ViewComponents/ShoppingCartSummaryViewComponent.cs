using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;

public class ShoppingCartSummaryViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public ShoppingCartSummaryViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Lấy mã khách hàng từ session
        string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
        if (string.IsNullOrEmpty(maKhachHangStr))
        {
            return View(0); // Nếu chưa đăng nhập, trả về số lượng 0
        }

        if (!int.TryParse(maKhachHangStr, out int maKhachHang))
        {
            return View(0);
        }

        // Truy vấn các mục giỏ hàng của khách hàng, bao gồm thông tin sản phẩm qua navigation property
        var cartItems = await _context.TbGioHangs
                                      .Include(g => g.MaSanPhamNavigation)
                                      .Where(g => g.MaKhachHang == maKhachHang)
                                      .ToListAsync();

        // Tính tổng số lượng sản phẩm, chuyển đổi về kiểu int
        int cartItemCount = cartItems.Sum(item => (int)item.SoLuong);

        return View(cartItemCount);
    }
}
