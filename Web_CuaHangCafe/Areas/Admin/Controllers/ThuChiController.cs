using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.ViewModels;

namespace Web_CuaHangCafe.Controllers
{
    [Area("Admin")]
    [Route("Admin/ThuChi")]
    public class ThuChiController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ThuChiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ThuChi/All
        [HttpGet("All")]
        public IActionResult All()
        {
            // Giá trị mặc định: từ ngày hôm qua đến hôm nay
            DateTime start = DateTime.Today.AddDays(-30);
            DateTime end = DateTime.Today; // Vì NgayLap đã là kiểu Date (hoặc có thời gian 00:00:00)
            var model = new AllThuChiViewModel
            {
                StartDate = start,
                EndDate = end,
                Items = GetAllThuChi(start, end)
            };

            return View(model);
        }

        // POST: Admin/ThuChi/All
        [HttpPost("All")]
        public IActionResult All(AllThuChiViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Dùng .Date để đảm bảo chỉ so sánh phần Date (không có giờ)
                DateTime dateStart = model.StartDate.Date;
                DateTime dateEnd = model.EndDate.Date;

                // Tính toán thu – chi theo khoảng thời gian lọc
                model.Items = GetAllThuChi(dateStart, dateEnd);
            }
            return View(model);
        }

        // Phương thức nội bộ để tính toán thu – chi của tất cả các quán
        private System.Collections.Generic.List<ThuChiItemViewModel> GetAllThuChi(DateTime startDate, DateTime endDate)
        {
            // Chuyển đổi các giá trị nhận vào sang .Date (loại phần giờ)
            DateTime dateStart = startDate.Date;
            DateTime dateEnd = endDate.Date;

            var quanCafes = _context.TbQuanCafes.ToList();
            var result = new System.Collections.Generic.List<ThuChiItemViewModel>();

            foreach (var quan in quanCafes)
            {
                var hoadon = _context.TbHoaDonBans
                    .Where(hd => hd.MaQuan == quan.MaQuan &&
                                 hd.NgayLap.Date >= dateStart &&
                                 hd.NgayLap.Date <= dateEnd)
                    .Sum(hd => (decimal?)hd.TongTien) ?? 0;

                var luong = _context.TbNhanViens
                    .Where(nv => nv.MaQuan == quan.MaQuan)
                    .Sum(nv => (decimal?)(nv.LuongCoBan * nv.HeSoLuong)) ?? 0;

                var nhap = (from ph in _context.TbPhieuNhapHangs
                            join ct in _context.TbPhieuNhapChiTiets on ph.MaPhieuNhap equals ct.MaPhieuNhap
                            where ph.MaQuan == quan.MaQuan &&
                                  ph.NgayLap.Date >= dateStart &&
                                  ph.NgayLap.Date <= dateEnd
                            select (decimal?)ct.ThanhTien).Sum() ?? 0;

                var thu = hoadon;
                var chi = nhap + luong;
                var profit = thu - chi;

                result.Add(new ThuChiItemViewModel
                {
                    MaQuan = quan.MaQuan,
                    TenQuan = quan.TenQuan,
                    TotalInvoice = hoadon,
                    TotalSalary = luong,
                    TotalRevenue = thu,
                    TotalImport = chi,
                    Profit = profit
                });
            }

            return result;
        }

        // Action chuyển sang trang Chi tiết Thu – Chi của một quán
        [HttpGet("All/Detail")]
        public IActionResult AllDetail(int maQuan, DateTime startDate, DateTime endDate)
        {
            return RedirectToAction("ThuChiChiTiet", new { cafeId = maQuan, startDate, endDate });
        }

        // Action Chi tiết Thu – Chi của một quán
        [HttpGet("ThuChiChiTiet")]
        public IActionResult ThuChiChiTiet(int cafeId, DateTime startDate, DateTime endDate)
        {
            DateTime dateStart = startDate.Date;
            DateTime dateEnd = endDate.Date;

            var detailModel = new ThuChiDetailViewModel
            {
                CafeId = cafeId,
                StartDate = startDate,
                EndDate = endDate,
                Employees = _context.TbNhanViens.Where(nv => nv.MaQuan == cafeId).ToList(),
                Invoices = _context.TbHoaDonBans
                    .Where(hd => hd.MaQuan == cafeId &&
                                 hd.NgayLap.Date >= dateStart &&
                                 hd.NgayLap.Date <= dateEnd)
                    .Include(hd => hd.TbChiTietHoaDonBans)
                    .ToList(),
                PhieuNhapHangs = _context.TbPhieuNhapHangs
                    .Where(pn => pn.MaQuan == cafeId &&
                                 pn.NgayLap.Date >= dateStart &&
                                 pn.NgayLap.Date <= dateEnd)
                    .Include(pn => pn.TbPhieuNhapChiTiets)
                    .Include(pn => pn.MaNhaCungCapNavigation)
                    .ToList()
            };

            return View(detailModel);
        }
    }
}
