using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Clients")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách khách hàng với phân trang
        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.TbKhachHangs.AsNoTracking()
                                .OrderBy(x => x.SdtkhachHang)
                                .ToList();
            var pagedListItem = new PagedList<TbKhachHang>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // Tìm kiếm khách hàng theo tên
        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            if (string.IsNullOrEmpty(search))
            {
                return RedirectToAction("Index");
            }
            search = search.ToLower();
            ViewBag.search = search;

            var listItem = _context.TbKhachHangs.AsNoTracking()
                                .Where(x => x.TenKhachHang.ToLower().Contains(search))
                                .OrderBy(x => x.SdtkhachHang)
                                .ToList();
            var pagedListItem = new PagedList<TbKhachHang>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // Form tạo khách hàng mới
        [Route("Create")]
        [Authentication]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Create")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TbKhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                _context.TbKhachHangs.Add(khachHang);
                _context.SaveChanges();
                TempData["Message"] = "Thêm thành công";
                return RedirectToAction("Index", "Clients");
            }
            return View(khachHang);
        }

        // Form chỉnh sửa thông tin khách hàng
        // (Nếu không cần thiết sử dụng biến name thì có thể loại bỏ tham số này)
        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var khachHang = _context.TbKhachHangs.Find(id);
            if (khachHang == null)
            {
                TempData["Message"] = "Không tìm thấy khách hàng cần sửa.";
                return RedirectToAction("Index", "Clients");
            }
            ViewBag.name = name;
            return View(khachHang);
        }

        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TbKhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(khachHang).State = EntityState.Modified;
                _context.SaveChanges();
                TempData["Message"] = "Sửa thành công";
                return RedirectToAction("Index", "Clients");
            }
            return View(khachHang);
        }

        // Hiển thị chi tiết của khách hàng, bao gồm:
        // 1. Thông tin khách hàng (tbKhachHang)
        // 2. Tài khoản đăng nhập của khách hàng (tbTaiKhoanKH)
        // 3. Lịch sử giao dịch và chi tiết hóa đơn (tbHoaDonBan và tbChiTietHoaDonBan)
        // 4. Giỏ hàng (TbGioHangs) nếu cần
        [Route("Details/{id}")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var khachHang = _context.TbKhachHangs
                .Include(x => x.TbTaiKhoanKhs)
                  .Include(x => x.TbGioHangs)
                .Include(x => x.TbHoaDonBans)
               
                    .ThenInclude(hd => hd.TbChiTietHoaDonBans)
           .ThenInclude(ct => ct.MaSanPhamNavigation) // include thông tin sản phẩm
                .FirstOrDefault(x => x.MaKhachHang == id);

            if (khachHang == null)
            {
                TempData["Message"] = "Không tìm thấy chi tiết khách hàng.";
                return RedirectToAction("Index", "Clients");
            }
            return View(khachHang);
        }

        // GET: Admin/Clients/Delete?id=10
        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            TempData["Message"] = "";

            if (!int.TryParse(id, out int customerId))
            {
                TempData["Message"] = "Sai định dạng mã khách hàng.";
                return RedirectToAction("Index", "Clients");
            }

            // Load khách hàng kèm thông tin liên quan cần thiết
            var khachHang = _context.TbKhachHangs
                .Include(x => x.TbTaiKhoanKhs)
                .Include(x => x.TbHoaDonBans)
                    .ThenInclude(hd => hd.TbChiTietHoaDonBans)
                .FirstOrDefault(x => x.MaKhachHang == customerId);

            if (khachHang == null)
            {
                TempData["Message"] = "Không tìm thấy khách hàng cần xóa.";
                return RedirectToAction("Index", "Clients");
            }

            // Hiển thị view xác nhận xoá cho khách hàng này
            return View(khachHang);
        }

        // POST: Admin/Clients/Delete
        [Route("Delete")]
        [Authentication]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Load khách hàng kèm các collection liên quan, bao gồm tài khoản, hóa đơn (và chi tiết hóa đơn) và giỏ hàng
            var khachHang = _context.TbKhachHangs
                .Include(x => x.TbTaiKhoanKhs)
                .Include(x => x.TbHoaDonBans)
                    .ThenInclude(hd => hd.TbChiTietHoaDonBans)
                .Include(x => x.TbGioHangs)
                .FirstOrDefault(x => x.MaKhachHang == id);

            if (khachHang == null)
            {
                TempData["Message"] = "Không tìm thấy khách hàng cần xóa.";
                return RedirectToAction("Index", "Clients");
            }

            // Xoá các tài khoản truy cập của khách hàng
            foreach (var account in khachHang.TbTaiKhoanKhs.ToList())
            {
                _context.TbTaiKhoanKhs.Remove(account);
            }

            // Xoá các hóa đơn bán và chi tiết hóa đơn liên quan
            foreach (var invoice in khachHang.TbHoaDonBans.ToList())
            {
                foreach (var detail in invoice.TbChiTietHoaDonBans.ToList())
                {
                    _context.TbChiTietHoaDonBans.Remove(detail);
                }
                _context.TbHoaDonBans.Remove(invoice);
            }

            // Xoá giỏ hàng của khách hàng
            foreach (var gioHang in khachHang.TbGioHangs.ToList())
            {
                _context.TbGioHangs.Remove(gioHang);
            }

            // Cuối cùng xoá khách hàng
            _context.TbKhachHangs.Remove(khachHang);
            _context.SaveChanges();

            TempData["Message"] = "Xoá khách hàng thành công.";
            return RedirectToAction("Index", "Clients");
        }


    }
}
