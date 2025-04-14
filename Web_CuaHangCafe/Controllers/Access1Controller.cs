using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Web_CuaHangCafe.Controllers
{
    public class Access1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Access1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public JsonResult CheckUserName(string tenTaiKhoan)
        {
            // Kiểm tra xem tên tài khoản có tồn tại trong bảng tài khoản hay không
            var exists = _context.TbTaiKhoanKhs.Any(x => x.TenTaiKhoan == tenTaiKhoan);
            // Nếu tồn tại, trả về false -> hiển thị lỗi, nếu không trả về true.
            return Json(!exists);
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



        // GET: /Access1/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Access1/Register (Đăng ký tài khoản khách hàng)
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (model.MatKhau != model.XacNhanMatKhau)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp!");
                return View(model);
            }

            // Băm mật khẩu trước khi lưu vào DB
            string hashPassword = HashPassword(model.MatKhau);

            // Kiểm tra trùng tên tài khoản khách hàng trong bảng tbTaiKhoanKhs
            var existKH = _context.TbTaiKhoanKhs
                .FirstOrDefault(x => x.TenTaiKhoan == model.TenTaiKhoan);
            if (existKH != null)
            {
                ModelState.AddModelError("", "Tên tài khoản đã tồn tại!");
                return View(model);
            }

            // Tạo record cho khách hàng trong bảng tbKhachHang
            TbKhachHang kh = new TbKhachHang
            {
                TenKhachHang = model.TenKhachHang,
                SdtkhachHang = model.SDTKhachHang,
                DiaChi = model.DiaChi
            };
            _context.TbKhachHangs.Add(kh);
            _context.SaveChanges();

            // Lấy mã khách hàng vừa được tạo
            int newMaKH = kh.MaKhachHang;

            // Tạo tài khoản khách hàng (tbTaiKhoanKhs)
            TbTaiKhoanKh tkKH = new TbTaiKhoanKh
            {
                TenTaiKhoan = model.TenTaiKhoan,
                MatKhau = hashPassword,
                MaKhachHang = newMaKH,
                MaQuyen = 2  // Role "User"
            };
            _context.TbTaiKhoanKhs.Add(tkKH);
            _context.SaveChanges();

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login1");
        }

        // GET: /Access1/Login
        // GET: /Access1/Login
        [HttpGet]
        public IActionResult Login1()
        {
            // Lấy số lần đăng nhập thất bại từ session (nếu chưa có thì là 0)
            int failedCount = HttpContext.Session.GetInt32("FailedLoginCount") ?? 0;
            ViewBag.FailedCount = failedCount;
            return View();
        }

        // POST: /Access1/Login (Kiểm tra tài khoản trong cả 2 bảng)
        [HttpPost]
        public IActionResult Login1(LoginViewModel model)
        {
            // Nếu account đang bị khóa
            var lockoutUntilStr = HttpContext.Session.GetString("LockoutUntil");
            if (!string.IsNullOrEmpty(lockoutUntilStr))
            {
                if (DateTime.TryParse(lockoutUntilStr, out DateTime lockoutUntil))
                {
                    if (DateTime.Now < lockoutUntil)
                    {
                        double minutesLeft = (lockoutUntil - DateTime.Now).TotalMinutes;
                        ModelState.AddModelError("", $"Chức năng đăng nhập bị khóa do đăng nhập sai 3 lần, vui lòng thử lại sau {Math.Ceiling(minutesLeft)} phút.");
                        ViewBag.FailedCount = HttpContext.Session.GetInt32("FailedLoginCount") ?? 0;
                        return View(model);
                    }
                    else
                    {
                        // Hết thời gian khóa, reset lại
                        HttpContext.Session.Remove("LockoutUntil");
                        HttpContext.Session.SetInt32("FailedLoginCount", 0);
                    }
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            // Băm mật khẩu của người dùng nhập
            string hashPassword = HashPassword(model.MatKhau);

            // Kiểm tra tài khoản trong bảng tbTaiKhoan (Admin và Employee)
            var tkAdmin = _context.TbTaiKhoans
                .FirstOrDefault(x => x.TenTaiKhoan == model.TenTaiKhoan && x.MatKhau == hashPassword);
            if (tkAdmin != null)
            {
                // Reset số lần đăng nhập thất bại nếu đăng nhập thành công
                HttpContext.Session.Remove("FailedLoginCount");
                HttpContext.Session.Remove("LockoutUntil");

                if (tkAdmin.MaQuyen == 1)
                {
                    HttpContext.Session.SetString("TenTaiKhoan", tkAdmin.TenTaiKhoan);
                    HttpContext.Session.SetString("Role", "Admin");
                    HttpContext.Session.SetString("MaNhanVien", tkAdmin.MaNhanVien.ToString());
                    return RedirectToAction("Index", "Home");
                }
                else if (tkAdmin.MaQuyen == 3)
                {
                    HttpContext.Session.SetString("TenTaiKhoan", tkAdmin.TenTaiKhoan);
                    HttpContext.Session.SetString("Role", "Employee");
                    HttpContext.Session.SetString("MaNhanVien", tkAdmin.MaNhanVien.ToString());
                    return RedirectToAction("Index", "Home");
                }
            }

            // Kiểm tra tài khoản khách hàng trong bảng tbTaiKhoanKhs
            var tkKH = _context.TbTaiKhoanKhs
                .FirstOrDefault(x => x.TenTaiKhoan == model.TenTaiKhoan && x.MatKhau == hashPassword);
            if (tkKH != null)
            {
                HttpContext.Session.Remove("FailedLoginCount");
                HttpContext.Session.Remove("LockoutUntil");

                HttpContext.Session.SetString("TenTaiKhoan", tkKH.TenTaiKhoan);
                HttpContext.Session.SetString("Role", "User");
                HttpContext.Session.SetString("MaKhachHang", tkKH.MaKhachHang.ToString());
                return RedirectToAction("Index", "Home");
            }

            // Nếu không tìm thấy tài khoản nào phù hợp, tăng số lần đăng nhập thất bại
            int failedCount = HttpContext.Session.GetInt32("FailedLoginCount") ?? 0;
            failedCount++;
            HttpContext.Session.SetInt32("FailedLoginCount", failedCount);
            ViewBag.FailedCount = failedCount;

            if (failedCount >= 3)
            {
                // Khi đăng nhập sai 3 lần trở lên, khóa đăng nhập trong 30 phút
                var lockoutUntil = DateTime.Now.AddMinutes(30);
                HttpContext.Session.SetString("LockoutUntil", lockoutUntil.ToString());
                ModelState.AddModelError("", "Bạn đã đăng nhập sai 3 lần. Tài khoản bị khóa trong 30 phút.");
            }
            else
            {
                ModelState.AddModelError("", $"Tài khoản hoặc mật khẩu không đúng! ({failedCount}/3)");
            }

            return View(model);
        }





        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login1", "Access1");
        }




    }
}