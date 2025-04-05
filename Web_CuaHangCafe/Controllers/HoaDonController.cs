using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Microsoft.AspNetCore.Http;

namespace Web_CuaHangCafe.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /HoaDon/Index
        // Hiển thị danh sách hóa đơn của khách hàng đang đăng nhập
        public async Task<IActionResult> Index()
        {
            // Lấy mã khách hàng từ session (loại int)
            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangStr))
            {
                return RedirectToAction("Login1", "Access1");
            }
            int maKhachHang = int.Parse(maKhachHangStr);

            // Lấy danh sách hóa đơn của khách hàng, sắp xếp theo Ngày lập giảm dần
            var listHoaDon = await _context.TbHoaDonBans
                .Where(hd => hd.MaKhachHang == maKhachHang)
                .OrderByDescending(hd => hd.NgayLap)
                .ToListAsync();

            return View(listHoaDon);
        }

        // GET: /HoaDon/Details/{id}
        // Hiển thị chi tiết hóa đơn (bao gồm danh sách chi tiết hóa đơn và thông tin sản phẩm)
        public async Task<IActionResult> Details(Guid id)
        {
            // Kiểm tra khách hàng đăng nhập
            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangStr))
            {
                return RedirectToAction("Login1", "Access1");
            }
            int maKhachHang = int.Parse(maKhachHangStr);

            // Lấy hóa đơn theo id và đảm bảo hóa đơn là của khách hàng đang đăng nhập
            var hoaDon = await _context.TbHoaDonBans
                .Include(hd => hd.TbChiTietHoaDonBans)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                .FirstOrDefaultAsync(hd => hd.MaHoaDon == id && hd.MaKhachHang == maKhachHang);

            if (hoaDon == null)
            {
                return NotFound("Hóa đơn không tồn tại hoặc không thuộc về tài khoản của bạn.");
            }
            return View(hoaDon);
        }
    }
}
