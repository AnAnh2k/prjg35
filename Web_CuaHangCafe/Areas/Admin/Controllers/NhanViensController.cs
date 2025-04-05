using System;

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;

using Web_CuaHangCafe.Models.Authentication;
using X.PagedList;
using Web_CuaHangCafe.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Security.Cryptography;


namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/NhanViens")]
    public class NhanViensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhanViensController(ApplicationDbContext context)
        {
            _context = context;
        }

        

        // Danh sách nhân viên với phân trang
        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listEmployee = _context.TbNhanViens.AsNoTracking()
                                   .Include(q => q.MaQuanNavigation)
                                  .OrderBy(x => x.HoTen)
                                  .ToList();
            PagedList<TbNhanVien> pagedList = new PagedList<TbNhanVien>(listEmployee, pageNumber, pageSize);
            return View(pagedList);
        }

        // Tìm kiếm nhân viên theo tên hoặc số điện thoại
        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            if (string.IsNullOrWhiteSpace(search))
            {
                return RedirectToAction("Index");
            }
            search = search.ToLower();
            ViewBag.search = search;

            var results = _context.TbNhanViens.AsNoTracking()
                             .Where(x => x.HoTen.ToLower().Contains(search) || (x.Sdt != null && x.Sdt.ToLower().Contains(search)))
                             .OrderBy(x => x.HoTen)
                             .ToList();
            var pagedList = new PagedList<TbNhanVien>(results, pageNumber, pageSize);
            return View(pagedList);
        }

        // Xem chi tiết nhân viên (bao gồm thông tin, tài khoản, phiếu nhập hàng)
        [Route("Details/{id}")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var employee = _context.TbNhanViens
                .Include(x => x.TbTaiKhoans) // Bao gồm tài khoản của nhân viên
                .Include(x => x.MaQuanNavigation) // Bao gồm thông tin quán mà nhân viên thuộc về
                .Include(x => x.TbHoaDonBans) // Bao gồm danh sách hóa đơn bán của nhân viên
                    .ThenInclude(hd => hd.TbChiTietHoaDonBans) // Bao gồm chi tiết hóa đơn bán
                .Include(x => x.TbPhieuNhapHangs) // Bao gồm danh sách phiếu nhập của nhân viên
                    .ThenInclude(ph => ph.TbPhieuNhapChiTiets) // Bao gồm chi tiết phiếu nhập
                    .ThenInclude(ct => ct.MaNguyenLieuNavigation)
                  .Include(x => x.TbHoaDonBans) // Bao gồm danh sách hóa đơn bán của nhân viên
                    .ThenInclude(hd => hd.TbChiTietHoaDonBans)
                      .ThenInclude(sp => sp.MaSanPhamNavigation)
                .Include(x => x.TbPhieuNhapHangs) // Bao gồm lại phiếu nhập
                    .ThenInclude(ncc => ncc.MaNhaCungCapNavigation) // Bao gồm thông tin nhà cung cấp của phiếu nhập
                .FirstOrDefault(x => x.MaNhanVien == id);

            if (employee == null)
            {
                TempData["Message"] = "Không tìm thấy nhân viên.";
                return RedirectToAction("Index", "NhanViens");
            }

            return View(employee);
        }




        // GET: Admin/NhanViens/Create
        [Route("Create")]
        [Authentication]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new EmployeeAccountViewModel();
            // Các dropdown cho nhân viên
            // (Nếu cần dropdown cho MaQuan, bạn cũng đã có)
            ViewData["MaQuan"] = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan");
            ViewData["MaNhanVien"] = new SelectList(_context.TbNhanViens, "MaNhanVien", "HoTen");
            ViewData["MaNhaCungCap"] = new SelectList(_context.TbNhaCungCaps, "MaNhaCungCap", "TenNhaCungCap");
            ViewData["MaQuan"] = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan");


            // Đối với tài khoản: truyền danh sách quyền
            ViewBag.MaQuyen = new SelectList(_context.TbQuyens, "MaQuyen", "TenQuyen");

            // Nếu cần truyền dropdown cho giới tính, có thể thiết lập từ view luôn.
            return View(model);
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

        // POST: Admin/NhanViens/Create
        [Route("Create")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeAccountViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Băm mật khẩu của tài khoản (trường MatKhau ở Account)
                string hashPassword = HashPassword(vm.Account.MatKhau);
                vm.Account.MatKhau = hashPassword;

                // Thêm thông tin nhân viên vào cơ sở dữ liệu
                _context.TbNhanViens.Add(vm.Employee);
                await _context.SaveChangesAsync();

                // Sau khi tạo nhân viên, gán MaNhanVien (được tạo tự động) cho tài khoản
                vm.Account.MaNhanVien = vm.Employee.MaNhanVien;

                // (Tùy chọn) Bạn có thể gán mặc định quyền cho nhân viên nếu cần, ví dụ:
                // vm.Account.MaQuyen = 2;

                // Thêm thông tin tài khoản vào cơ sở dữ liệu
                _context.TbTaiKhoans.Add(vm.Account);
                int kq = await _context.SaveChangesAsync();

                TempData["Message"] = kq > 0 ? "Thêm nhân viên thành công" : "Không thêm được nhân viên";
                return RedirectToAction("Index");
            }
            return View(vm);
        }


        // GET: Admin/NhanViens/Edit/{id}
        [Route("Edit/{id}")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Lấy thông tin nhân viên theo MaNhanVien
            var employee = _context.TbNhanViens.FirstOrDefault(e => e.MaNhanVien == id);
            if (employee == null)
            {
                TempData["Message"] = "Không tìm thấy nhân viên cần sửa.";
                return RedirectToAction("Index", "NhanViens");
            }

            // Lấy tài khoản tương ứng của nhân viên (giả sử chỉ có 1 tài khoản cho mỗi nhân viên)
            var account = _context.TbTaiKhoans.FirstOrDefault(a => a.MaNhanVien == id);

            // Tạo view model tổng hợp
            var viewModel = new EmployeeAccountViewModel
            {
                Employee = employee,
                Account = account ?? new TbTaiKhoan() // Nếu chưa có tài khoản thì tạo mới
            };

            // Điền dữ liệu cho dropdown: Quán và Quyền
            ViewBag.MaQuan = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan", employee.MaQuan);
            ViewBag.MaQuyen = new SelectList(_context.TbQuyens, "MaQuyen", "TenQuyen", account?.MaQuyen);

            return View(viewModel);
        }

        // POST: Admin/NhanViens/Edit
        [Route("Edit/{id}")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EmployeeAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaQuan = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan", model.Employee.MaQuan);
                ViewBag.MaQuyen = new SelectList(_context.TbQuyens, "MaQuyen", "TenQuyen", model.Account.MaQuyen);
                return View(model);
            }

            var employeeFromDB = _context.TbNhanViens
                .Include(e => e.TbTaiKhoans)
                .FirstOrDefault(e => e.MaNhanVien == model.Employee.MaNhanVien);
            if (employeeFromDB == null)
            {
                TempData["Message"] = "Không tìm thấy nhân viên cần sửa.";
                return RedirectToAction("Index", "NhanViens");
            }

            // Cập nhật thông tin nhân viên
            employeeFromDB.HoTen = model.Employee.HoTen;
            employeeFromDB.DiaChi = model.Employee.DiaChi;
            employeeFromDB.Email = model.Employee.Email;
            employeeFromDB.NgaySinh = model.Employee.NgaySinh;
            employeeFromDB.GioiTinh = model.Employee.GioiTinh;
            employeeFromDB.ChucVu = model.Employee.ChucVu;
            employeeFromDB.Sdt = model.Employee.Sdt;
            employeeFromDB.SoCccd = model.Employee.SoCccd;
            employeeFromDB.LuongCoBan = model.Employee.LuongCoBan;
            employeeFromDB.HeSoLuong = model.Employee.HeSoLuong;
            employeeFromDB.MaQuan = model.Employee.MaQuan;

            _context.TbNhanViens.Update(employeeFromDB);

            // Cập nhật thông tin tài khoản
            var accountFromDB = _context.TbTaiKhoans.FirstOrDefault(a => a.MaNhanVien == model.Employee.MaNhanVien);
            if (accountFromDB != null)
            {
                accountFromDB.TenTaiKhoan = model.Account.TenTaiKhoan;
                if (!string.IsNullOrWhiteSpace(model.Account.MatKhau))
                {
                    accountFromDB.MatKhau = HashPassword(model.Account.MatKhau);
                }
                accountFromDB.MaQuyen = model.Account.MaQuyen;
                _context.TbTaiKhoans.Update(accountFromDB);
            }
            else
            {
                var newAccount = new TbTaiKhoan
                {
                    MaNhanVien = model.Employee.MaNhanVien,
                    TenTaiKhoan = model.Account.TenTaiKhoan,
                    MatKhau = string.IsNullOrWhiteSpace(model.Account.MatKhau) ? "" : HashPassword(model.Account.MatKhau),
                    MaQuyen = model.Account.MaQuyen
                };
                _context.TbTaiKhoans.Add(newAccount);
            }

            _context.SaveChanges();
            TempData["Message"] = "Cập nhật nhân viên thành công.";
            return RedirectToAction("Index", "NhanViens");
        }


        // Xoá nhân viên (với xác nhận và xoá cascade các thông tin liên quan: tài khoản, phiếu nhập hàng – bao gồm chi tiết phiếu nhập)
        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            TempData["Message"] = "";
            if (!int.TryParse(id, out int employeeId))
            {
                TempData["Message"] = "Sai định dạng mã nhân viên.";
                return RedirectToAction("Index", "NhanViens");
            }

            var employee = _context.TbNhanViens
                              .Include(x => x.TbTaiKhoans)
                              .Include(x => x.TbPhieuNhapHangs)
                                  .ThenInclude(ph => ph.TbPhieuNhapChiTiets)
                              .FirstOrDefault(x => x.MaNhanVien == employeeId);

            if (employee == null)
            {
                TempData["Message"] = "Không tìm thấy nhân viên cần xóa.";
                return RedirectToAction("Index", "NhanViens");
            }
            return View(employee);
        }

        [Route("Delete")]
        [Authentication]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _context.TbNhanViens
                              .Include(x => x.TbTaiKhoans)
                               .Include(x => x.TbHoaDonBans)
                                                              .ThenInclude(ph => ph.TbChiTietHoaDonBans)
                              .Include(x => x.TbPhieuNhapHangs)
                                                              .ThenInclude(ph => ph.TbPhieuNhapChiTiets)
                              .FirstOrDefault(x => x.MaNhanVien == id);

            if (employee == null)
            {
                TempData["Message"] = "Không tìm thấy nhân viên cần xóa.";
                return RedirectToAction("Index", "NhanViens");
            }

            // Xoá phiếu nhập hàng và chi tiết
            foreach (var hoaDonBan in employee.TbHoaDonBans.ToList())
            {
                foreach (var ct in hoaDonBan.TbChiTietHoaDonBans.ToList())
                {
                    _context.TbChiTietHoaDonBans.Remove(ct);
                }
                _context.TbHoaDonBans.Remove(hoaDonBan);
            }

            // Xoá tài khoản
            foreach (var account in employee.TbTaiKhoans.ToList())
            {
                _context.TbTaiKhoans.Remove(account);
            }

            // Xoá phiếu nhập hàng và chi tiết
            foreach (var phieuNhap in employee.TbPhieuNhapHangs.ToList())
            {
                foreach (var ct in phieuNhap.TbPhieuNhapChiTiets.ToList())
                {
                    _context.TbPhieuNhapChiTiets.Remove(ct);
                }
                _context.TbPhieuNhapHangs.Remove(phieuNhap);
            }
        

            // Cuối cùng xoá nhân viên
            _context.TbNhanViens.Remove(employee);
            _context.SaveChanges();

            TempData["Message"] = "Xóa nhân viên thành công.";
            return RedirectToAction("Index", "NhanViens");
        }
    }
}
