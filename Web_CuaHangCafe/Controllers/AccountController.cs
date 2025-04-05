using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Security.Cryptography;

namespace Web_CuaHangCafe.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/ThongTin
        public async Task<IActionResult> ThongTin()
        {
            string tenTaiKhoan = HttpContext.Session.GetString("TenTaiKhoan") ?? "";
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (string.IsNullOrEmpty(tenTaiKhoan) || string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login1", "Access1");
            }

            var viewModel = new AccountInfoViewModel { Role = role, TenTaiKhoan = tenTaiKhoan };

            if (role == "Admin")
            {
                var tkAdmin = await _context.TbTaiKhoans.FirstOrDefaultAsync(a => a.TenTaiKhoan == tenTaiKhoan);
                if (tkAdmin == null)
                    return NotFound("Tài khoản admin không tồn tại.");
                var nhanVien = await _context.TbNhanViens
                    .Include(nv => nv.MaQuanNavigation)
                    .FirstOrDefaultAsync(nv => nv.MaNhanVien == tkAdmin.MaNhanVien);
                if (nhanVien == null)
                    return NotFound("Thông tin nhân viên không tồn tại.");
                viewModel.NhanVienInfo = nhanVien;
            }
            else if (role == "User")
            {
                var tkKH = await _context.TbTaiKhoanKhs.FirstOrDefaultAsync(a => a.TenTaiKhoan == tenTaiKhoan);
                if (tkKH == null)
                    return NotFound("Tài khoản khách hàng không tồn tại.");
                var khachHang = await _context.TbKhachHangs.FirstOrDefaultAsync(kh => kh.MaKhachHang == tkKH.MaKhachHang);
                if (khachHang == null)
                    return NotFound("Thông tin khách hàng không tồn tại.");
                viewModel.KhachHangInfo = khachHang;
            }

            return View(viewModel);
        }

        // GET: /Account/EditThongTin
        public async Task<IActionResult> EditThongTin()
        {
            // Tương tự như action ThongTin => load dữ liệu hiện tại vào AccountInfoViewModel
            string tenTaiKhoan = HttpContext.Session.GetString("TenTaiKhoan") ?? "";
            string role = HttpContext.Session.GetString("Role") ?? "";
            ViewData["MaQuan"] = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan");
            if (string.IsNullOrEmpty(tenTaiKhoan) || string.IsNullOrEmpty(role))
                return RedirectToAction("Login1", "Access1");

            var viewModel = new AccountInfoViewModel { Role = role, TenTaiKhoan = tenTaiKhoan };

            if (role == "Admin")
            {
                var tkAdmin = await _context.TbTaiKhoans.FirstOrDefaultAsync(a => a.TenTaiKhoan == tenTaiKhoan);
                if (tkAdmin == null)
                    return NotFound("Tài khoản admin không tồn tại.");
                var nhanVien = await _context.TbNhanViens
                    .Include(nv => nv.MaQuanNavigation)
                    .FirstOrDefaultAsync(nv => nv.MaNhanVien == tkAdmin.MaNhanVien);
                if (nhanVien == null)
                    return NotFound("Thông tin nhân viên không tồn tại.");
                viewModel.NhanVienInfo = nhanVien;
            }
            else if (role == "User")
            {
                var tkKH = await _context.TbTaiKhoanKhs.FirstOrDefaultAsync(a => a.TenTaiKhoan == tenTaiKhoan);
                if (tkKH == null)
                    return NotFound("Tài khoản khách hàng không tồn tại.");
                var khachHang = await _context.TbKhachHangs.FirstOrDefaultAsync(kh => kh.MaKhachHang == tkKH.MaKhachHang);
                if (khachHang == null)
                    return NotFound("Thông tin khách hàng không tồn tại.");
                viewModel.KhachHangInfo = khachHang;
            }

            return View(viewModel);
        }

        // Hàm băm mật khẩu dùng SHA-256
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }


        // POST: /Account/EditThongTin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditThongTin(AccountInfoViewModel model)
        {
            // Lấy thông tin đăng nhập từ Session
            string tenTaiKhoan = HttpContext.Session.GetString("TenTaiKhoan") ?? "";
       
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (string.IsNullOrEmpty(tenTaiKhoan) || string.IsNullOrEmpty(role))
                return RedirectToAction("Login1", "Access1");

            if (!ModelState.IsValid)
                return View(model);

            if (role == "Admin")
            {
                // Tìm tài khoản admin theo tên tài khoản
                var tkAdmin = await _context.TbTaiKhoans.FirstOrDefaultAsync(a => a.TenTaiKhoan == tenTaiKhoan);
                if (tkAdmin == null)
                    return NotFound("Tài khoản admin không tồn tại.");

                // Lấy thông tin nhân viên liên quan của tài khoản admin
                var nhanVien = await _context.TbNhanViens.FirstOrDefaultAsync(nv => nv.MaNhanVien == tkAdmin.MaNhanVien);
                if (nhanVien == null)
                    return NotFound("Thông tin nhân viên không tồn tại.");

                // Cập nhật thông tin tài khoản chung
                tkAdmin.TenTaiKhoan = string.IsNullOrEmpty(model.TenTaiKhoan) ? tkAdmin.TenTaiKhoan : model.TenTaiKhoan;
                tkAdmin.MatKhau = string.IsNullOrEmpty(model.MatKhau) ? tkAdmin.MatKhau : model.MatKhau;
                // Cập nhật các trường của thông tin nhân viên nếu có dữ liệu (model.NhanVienInfo không null)
                if (model.NhanVienInfo != null)
                {
                    nhanVien.HoTen = string.IsNullOrEmpty(model.NhanVienInfo.HoTen) ? nhanVien.HoTen : model.NhanVienInfo.HoTen;
                    nhanVien.DiaChi = string.IsNullOrEmpty(model.NhanVienInfo.DiaChi) ? nhanVien.DiaChi : model.NhanVienInfo.DiaChi;
                    nhanVien.Email = string.IsNullOrEmpty(model.NhanVienInfo.Email) ? nhanVien.Email : model.NhanVienInfo.Email;
                    nhanVien.NgaySinh = model.NhanVienInfo.NgaySinh ?? nhanVien.NgaySinh;
                    nhanVien.GioiTinh = string.IsNullOrEmpty(model.NhanVienInfo.GioiTinh) ? nhanVien.GioiTinh : model.NhanVienInfo.GioiTinh;
                    nhanVien.ChucVu = string.IsNullOrEmpty(model.NhanVienInfo.ChucVu) ? nhanVien.ChucVu : model.NhanVienInfo.ChucVu;
                    nhanVien.Sdt = string.IsNullOrEmpty(model.NhanVienInfo.Sdt) ? nhanVien.Sdt : model.NhanVienInfo.Sdt;
                    nhanVien.SoCccd = string.IsNullOrEmpty(model.NhanVienInfo.SoCccd) ? nhanVien.SoCccd : model.NhanVienInfo.SoCccd;
                    nhanVien.LuongCoBan = model.NhanVienInfo.LuongCoBan != 0 ? model.NhanVienInfo.LuongCoBan : nhanVien.LuongCoBan;
                    nhanVien.HeSoLuong = model.NhanVienInfo.HeSoLuong != 0 ? model.NhanVienInfo.HeSoLuong : nhanVien.HeSoLuong;
                    nhanVien.MaQuan = model.NhanVienInfo.MaQuan != 0 ? model.NhanVienInfo.MaQuan : nhanVien.MaQuan;
                }

                _context.TbTaiKhoans.Update(tkAdmin);
                _context.TbNhanViens.Update(nhanVien);
            }
            else if (role == "User")
            {
                // Tìm tài khoản khách hàng theo tên tài khoản
                var tkKH = await _context.TbTaiKhoanKhs.FirstOrDefaultAsync(a => a.TenTaiKhoan == tenTaiKhoan);
                if (tkKH == null)
                    return NotFound("Tài khoản khách hàng không tồn tại.");

                // Lấy thông tin khách hàng liên quan
                var khachHang = await _context.TbKhachHangs.FirstOrDefaultAsync(kh => kh.MaKhachHang == tkKH.MaKhachHang);
                if (khachHang == null)
                    return NotFound("Thông tin khách hàng không tồn tại.");

                // Cập nhật thông tin tài khoản chung
                tkKH.TenTaiKhoan = string.IsNullOrEmpty(model.TenTaiKhoan) ? tkKH.TenTaiKhoan : model.TenTaiKhoan;
                tkKH.MatKhau = string.IsNullOrEmpty(model.MatKhau) ? tkKH.MatKhau : model.MatKhau;
                // Cập nhật các trường thông tin khách hàng nếu có dữ liệu
                if (model.KhachHangInfo != null)
                {
                    khachHang.TenKhachHang = string.IsNullOrEmpty(model.KhachHangInfo.TenKhachHang) ? khachHang.TenKhachHang : model.KhachHangInfo.TenKhachHang;
                    khachHang.DiaChi = string.IsNullOrEmpty(model.KhachHangInfo.DiaChi) ? khachHang.DiaChi : model.KhachHangInfo.DiaChi;
                    khachHang.SdtkhachHang = string.IsNullOrEmpty(model.KhachHangInfo.SdtkhachHang) ? khachHang.SdtkhachHang : model.KhachHangInfo.SdtkhachHang;
                }

                _context.TbTaiKhoanKhs.Update(tkKH);
                _context.TbKhachHangs.Update(khachHang);
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["Message"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("ThongTin");
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật, vui lòng thử lại.");
                return View(model);
            }
        }

    }
}
