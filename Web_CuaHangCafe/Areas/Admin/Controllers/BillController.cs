using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Bill")]
    public class BillController : Controller
    {
        private readonly Data.ApplicationDbContext _context;

        public BillController(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // Action Confirm để cập nhật trạng thái hóa đơn
        [HttpGet]
        [Route("Confirm/{id}")]
        [Authentication]
        public async Task<IActionResult> Confirm(Guid id)
        {
            // Lấy mã nhân viên đang đăng nhập từ Session hoặc từ User Claims
            string maNhanVienStr = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVienStr))
            {
                TempData["Message"] = "Bạn cần đăng nhập để xác nhận hóa đơn.";
                return RedirectToAction("Login1", "Access1");
            }
            int maNhanVien = int.Parse(maNhanVienStr);

            // Tìm hóa đơn theo mã hóa đơn id
            var order = await _context.TbHoaDonBans.FindAsync(id);
            if (order == null)
            {
                TempData["Message"] = "Không tìm thấy hóa đơn.";
                return RedirectToAction("Index");
            }

            // Nếu hóa đơn đã được xác nhận, không cho xác nhận lại
            if (order.TrangThai != "Chưa hoàn thành")
            {
                TempData["Message"] = "Hóa đơn đã được xác nhận.";
                return RedirectToAction("Index");
            }

            // Cập nhật hóa đơn: gán MaNhanVien và chuyển trạng thái
            order.MaNhanVien = maNhanVien;
            order.TrangThai = "Hoàn thành";

            _context.TbHoaDonBans.Update(order);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Xác nhận hóa đơn thành công.";
            return RedirectToAction("Index");
        }

        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.TbHoaDonBans
                                   .Include(x => x.MaKhachHangNavigation)
                                        .Include(x => x.MaNhanVienNavigation)
                                        .Include(x => x.MaQuanNavigation)// Load thông tin khách hàng
                                   .AsNoTracking()
                                   .OrderByDescending(x => x.NgayLap)
                                   .ToList();
            PagedList<TbHoaDonBan> pagedListItem = new PagedList<TbHoaDonBan>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }


        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            ViewBag.search = search;

            // Nếu giá trị search không rỗng và có thể chuyển sang DateTime
            List<TbHoaDonBan> listItem;
            if (!string.IsNullOrEmpty(search) && DateTime.TryParse(search, out DateTime searchDate))
            {
                // So sánh ngày bán (chỉ lấy phần Date) với ngày tìm kiếm
                listItem = _context.TbHoaDonBans
                    .AsNoTracking()
                    .Where(x => x.NgayLap.Date == searchDate.Date)
                    .OrderBy(x => x.MaHoaDon)
                    .ToList();
            }
            else
            {
                // Nếu không có giá trị tìm kiếm hoặc search không hợp lệ, trả về danh sách tất cả
                listItem = _context.TbHoaDonBans
                    .AsNoTracking()
                    .OrderBy(x => x.MaHoaDon)
                    .ToList();
            }

            PagedList<TbHoaDonBan> pagedListItem = new PagedList<TbHoaDonBan>(listItem, pageNumber, pageSize);
            return View(pagedListItem);
        }

        [Route("Details")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int? page, string id, string name)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Message"] = "Mã hóa đơn không hợp lệ.";
                return RedirectToAction("Index");
            }
            if (!Guid.TryParse(id, out Guid billGuid))
            {
                TempData["Message"] = "Sai định dạng mã hóa đơn.";
                return RedirectToAction("Index");
            }

            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;

            // Đảm bảo Include navigation property của hóa đơn và sản phẩm
            var listItem = _context.TbChiTietHoaDonBans
                .Include(ct => ct.MaHoaDonNavigation)
                .Include(ct => ct.MaSanPhamNavigation)
                .AsNoTracking()
                .Where(x => x.MaHoaDon == billGuid)
                .OrderBy(x => x.MaHoaDon)
                .ToList();

            if (!listItem.Any())
            {
                TempData["Message"] = "Không tìm thấy chi tiết của hóa đơn.";
                return RedirectToAction("Index");
            }

            var pagedListItem = new X.PagedList.PagedList<TbChiTietHoaDonBan>(listItem, pageNumber, pageSize);

            ViewBag.Name = name;

            return View(pagedListItem);
        }


    }
}
