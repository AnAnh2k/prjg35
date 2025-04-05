using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Areas.Admin.ViewModels;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using Web_CuaHangCafe.ViewModels;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/PhieuNhapHang")]
    public class PhieuNhapHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhieuNhapHangController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Admin/PhieuNhapHang
        [Route("")]
        [Route("Index")]
        [Authentication]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;

            var listItem = await _context.TbPhieuNhapHangs
                 .Include(p => p.MaQuanNavigation)
                 .Include(p => p.MaNhanVienNavigation)
                 .Include(p => p.MaNhaCungCapNavigation)
                 .Include(p => p.TbPhieuNhapChiTiets) // Bao gồm chi tiết phiếu nhập
                 .AsNoTracking()
                 .OrderByDescending(p => p.NgayLap)
                 .ToListAsync();

        

      

            var pagedList = new PagedList<TbPhieuNhapHang>(listItem, pageNumber, pageSize);

            return View(pagedList);
        }


        // GET: Admin/PhieuNhapHang/Search?search=...
        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;

            // Nếu search có thể chuyển thành ngày, tìm theo ngày lập
            List<TbPhieuNhapHang> listItem;
            if (!string.IsNullOrEmpty(search) && DateTime.TryParse(search, out DateTime searchDate))
            {
                listItem = _context.TbPhieuNhapHangs
                    .AsNoTracking()
                    .Where(p => p.NgayLap.Date == searchDate.Date)
                    .OrderByDescending(p => p.NgayLap)
                    .ToList();
            }
            else
            {
                // Nếu không có giá trị tìm kiếm hợp lệ, trả về tất cả
                listItem = _context.TbPhieuNhapHangs
                    .AsNoTracking()
                    .OrderByDescending(p => p.NgayLap)
                    .ToList();
            }

            var pagedList = new PagedList<TbPhieuNhapHang>(listItem, pageNumber, pageSize);
            ViewBag.search = search;
            return View(pagedList);
        }

        // GET: Admin/PhieuNhapHang/Details/{id}
        [Route("Details/{id}")]
        [Authentication]
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id, int? page)
        {
            if (id == null)
            {
                TempData["Message"] = "Mã phiếu nhập không hợp lệ.";
                return RedirectToAction("Index");
            }
            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;

            // Include các Chi tiết và thông tin navigation (Nguyên liệu,…)
            var phieuNhap = await _context.TbPhieuNhapHangs
                .Include(p => p.MaQuanNavigation)
                .Include(p => p.MaNhanVienNavigation)
                .Include(p => p.MaNhaCungCapNavigation)
                .Include(p => p.TbPhieuNhapChiTiets)
                    .ThenInclude(ct => ct.MaNguyenLieuNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.MaPhieuNhap == id);

            if (phieuNhap == null)
            {
                TempData["Message"] = "Không tìm thấy phiếu nhập.";
                return RedirectToAction("Index");
            }
            return View(phieuNhap);
        }

        //[Route("Nhap")]
        //[HttpGet]
        //public IActionResult Nhap()
        //{
        //    var dsNguyenLieu = _context.TbNguyenLieus
        //                            .AsNoTracking()
        //                            .OrderBy(nl => nl.TenNguyenLieu)
        //                            .ToList();
                
        //    var vm = new Web_CuaHangCafe.ViewModels.PhieuNhapViewModel
        //    {
        //        NgayNhap = DateTime.Now,
        //        GhiChu = string.Empty,
        //        ChiTietNhap = dsNguyenLieu.Select(nl => new Web_CuaHangCafe.ViewModels.PhieuNhapChiTietViewModel
        //        {
        //            MaNguyenLieu = nl.MaNguyenLieu,
        //            TenNguyenLieu = nl.TenNguyenLieu,
        //            SoLuongNhap = 0,
        //            DonGiaNhap = nl.DonGia
        //        }).ToList()
        //    };

        //    // Lấy mã nhân viên từ session và truy vấn tên
        //    var maNhanVienStr = HttpContext.Session.GetString("MaNhanVien");
        //    int maNhanVien = 0;
        //    string tenNhanVien = string.Empty;
        //    if (!string.IsNullOrEmpty(maNhanVienStr))
        //    {
        //        maNhanVien = int.Parse(maNhanVienStr);
        //        var nhanVien = _context.TbNhanViens.FirstOrDefault(x => x.MaNhanVien == maNhanVien);
        //        if (nhanVien != null)
        //        {
        //            tenNhanVien = nhanVien.HoTen;
        //        }
        //    }

        //    // Thay vì tạo dropdown, ta chuyển sang hiển thị thông tin cố định
         

        //    //ViewBag.MaNhanVien = new SelectList(_context.TbNhanViens, "MaNhanVien", "HoTen");
        //    ViewBag.MaQuan = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan");
        //    ViewBag.MaNhaCungCap = new SelectList(_context.TbNhaCungCaps, "MaNhaCungCap", "TenNhaCungCap");
        //    ViewBag.MaNhanVien = maNhanVien;
        //    ViewBag.TenNhanVien = tenNhanVien;
        //    return View(vm);
        //}

        [Route("Nhap")]
        [HttpGet]
        public IActionResult Nhap()
        {
            // Lấy mã nhân viên từ session
            var maNhanVienStr = HttpContext.Session.GetString("MaNhanVien");
            int maNhanVien = 0;
            string tenNhanVien = string.Empty;
            if (!string.IsNullOrEmpty(maNhanVienStr))
            {
                maNhanVien = int.Parse(maNhanVienStr);
                // Truy vấn để lấy tên nhân viên
                var nhanVien = _context.TbNhanViens.FirstOrDefault(x => x.MaNhanVien == maNhanVien);
                if (nhanVien != null)
                {
                    tenNhanVien = nhanVien.HoTen;
                }
            }

            // Khởi tạo view model cho phiếu nhập
            var vm = new Web_CuaHangCafe.ViewModels.PhieuNhapViewModel
            {
                NgayNhap = DateTime.Now,
                GhiChu = string.Empty,
                ChiTietNhap = _context.TbNguyenLieus
                                       .AsNoTracking()
                                       .OrderBy(nl => nl.TenNguyenLieu)
                                       .Select(nl => new Web_CuaHangCafe.ViewModels.PhieuNhapChiTietViewModel
                                       {
                                           MaNguyenLieu = nl.MaNguyenLieu,
                                           TenNguyenLieu = nl.TenNguyenLieu,
                                           SoLuongNhap = 0,
                                           DonGiaNhap = nl.DonGia
                                       }).ToList()
            };

            // Lấy các SelectList cho dropdown chỉ đối với Quán và Nhà cung cấp
            ViewBag.MaQuan = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan");
            ViewBag.MaNhaCungCap = new SelectList(_context.TbNhaCungCaps, "MaNhaCungCap", "TenNhaCungCap");

            // Lưu thông tin nhân viên vào ViewBag
            ViewBag.MaNhanVien = maNhanVien;
            ViewBag.TenNhanVien = tenNhanVien;

            return View(vm);
        }






        [Route("Nhap")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Nhap(Web_CuaHangCafe.ViewModels.PhieuNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaQuan = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan");
                ViewBag.MaNhaCungCap = new SelectList(_context.TbNhaCungCaps, "MaNhaCungCap", "TenNhaCungCap");
                return View(model);
            }

            // Lấy mã nhân viên từ session
            var maNhanVienStr = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVienStr))
            {
                // Nếu không tìm thấy, báo lỗi phù hợp.
                ModelState.AddModelError("", "Không thể xác định được nhân viên đăng nhập.");
                ViewBag.MaQuan = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan");
                ViewBag.MaNhaCungCap = new SelectList(_context.TbNhaCungCaps, "MaNhaCungCap", "TenNhaCungCap");
            // Nếu muốn hiển thị mã nhân viên
               // Nếu muốn hiển thị tên nhân viên
                return View(model);
            }
            int maNhanVien = int.Parse(maNhanVienStr);

            // Tạo header phiếu nhập sử dụng thông tin từ view model và mã nhân viên từ session
            var phieuNhap = new TbPhieuNhapHang
            {

                MaPhieuNhap = Guid.NewGuid(),
                NgayLap = model.NgayNhap,
                GhiChu = model.GhiChu,
                MaNhanVien = maNhanVien,
                MaQuan = model.MaQuan,
                MaNhaCungCap = model.MaNhaCungCap

            };

            _context.TbPhieuNhapHangs.Add(phieuNhap);
            await _context.SaveChangesAsync();

            // Xử lý danh sách chi tiết phiếu nhập
            foreach (var ct in model.ChiTietNhap)
            {
                if (ct.SoLuongNhap > 0)
                {
                    var detail = new TbPhieuNhapChiTiet
                    {
                        MaPhieuNhap = phieuNhap.MaPhieuNhap,
                        MaNguyenLieu = ct.MaNguyenLieu,
                        SoLuong = ct.SoLuongNhap,
                        DonGia = ct.DonGiaNhap
                    };
                    _context.TbPhieuNhapChiTiets.Add(detail);

                    // Cập nhật số lượng tồn kho của nguyên liệu
                    var nguyenLieu = await _context.TbNguyenLieus.FindAsync(ct.MaNguyenLieu);
                    if (nguyenLieu != null)
                    {
                        nguyenLieu.SoLuong += ct.SoLuongNhap;
                        _context.TbNguyenLieus.Update(nguyenLieu);
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Phiếu nhập đã được lưu thành công.";
            return RedirectToAction("Index", "PhieuNhapHang");
        }







        //// GET: Admin/PhieuNhapHang/Create
        //[Route("Create")]
        //[Authentication]
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    var model = new TbPhieuNhapHang
        //    {
        //        NgayLap = DateTime.Now
        //    };

        //    ViewData["MaQuan"] = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan");
        //    ViewData["MaNhanVien"] = new SelectList(_context.TbNhanViens, "MaNhanVien", "HoTen");
        //    ViewData["MaNhaCungCap"] = new SelectList(_context.TbNhaCungCaps, "MaNhaCungCap", "TenNhaCungCap");

        //    return View(model);
        //}

        //// POST: Admin/PhieuNhapHang/Create
        //[Route("Create")]
        //[Authentication]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("MaQuan,NgayLap,MaNhanVien,MaNhaCungCap,GhiChu")] TbPhieuNhapHang phieuNhap)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        phieuNhap.MaPhieuNhap = Guid.NewGuid();
        //        _context.TbPhieuNhapHangs.Add(phieuNhap);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MaQuan"] = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan", phieuNhap.MaQuan);
        //    ViewData["MaNhanVien"] = new SelectList(_context.TbNhanViens, "MaNhanVien", "HoTen", phieuNhap.MaNhanVien);
        //    ViewData["MaNhaCungCap"] = new SelectList(_context.TbNhaCungCaps, "MaNhaCungCap", "TenNhaCungCap", phieuNhap.MaNhaCungCap);
        //    return View(phieuNhap);
        //}

        // GET: Admin/PhieuNhapHang/Edit/{id}
        [Route("Edit/{id}")]
        [Authentication]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();
            var phieuNhap = await _context.TbPhieuNhapHangs.FindAsync(id);
            if (phieuNhap == null)
                return NotFound();
            ViewData["MaQuan"] = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan", phieuNhap.MaQuan);
            ViewData["MaNhanVien"] = new SelectList(_context.TbNhanViens, "MaNhanVien", "HoTen", phieuNhap.MaNhanVien);
            ViewData["MaNhaCungCap"] = new SelectList(_context.TbNhaCungCaps, "MaNhaCungCap", "TenNhaCungCap", phieuNhap.MaNhaCungCap);
            return View(phieuNhap);
        }

        // POST: Admin/PhieuNhapHang/Edit/{id}
        [Route("Edit/{id}")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MaPhieuNhap,MaQuan,NgayLap,MaNhanVien,MaNhaCungCap,GhiChu")] TbPhieuNhapHang phieuNhap)
        {
            if (id != phieuNhap.MaPhieuNhap)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phieuNhap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhieuNhapExists(phieuNhap.MaPhieuNhap))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaQuan"] = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan", phieuNhap.MaQuan);
            ViewData["MaNhanVien"] = new SelectList(_context.TbNhanViens, "MaNhanVien", "HoTen", phieuNhap.MaNhanVien);
            ViewData["MaNhaCungCap"] = new SelectList(_context.TbNhaCungCaps, "MaNhaCungCap", "TenNhaCungCap", phieuNhap.MaNhaCungCap);
            return View(phieuNhap);
        }

        // GET: Admin/PhieuNhapHang/Delete/{id}
        [Route("Delete/{id}")]
        [Authentication]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var phieuNhap = await _context.TbPhieuNhapHangs
                .Include(p => p.MaQuanNavigation)
                .Include(p => p.MaNhanVienNavigation)
                .Include(p => p.MaNhaCungCapNavigation)
                .FirstOrDefaultAsync(p => p.MaPhieuNhap == id);
            if (phieuNhap == null)
                return NotFound();
            return View(phieuNhap);
        }

        // POST: Admin/PhieuNhapHang/Delete/{id}
        [Route("Delete/{id}")]
        [Authentication]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var phieuNhap = await _context.TbPhieuNhapHangs.FindAsync(id);
            if (phieuNhap != null)
            {
                // Xoá các chi tiết liên quan trước
                var details = _context.TbPhieuNhapChiTiets.Where(ct => ct.MaPhieuNhap == id);
                _context.TbPhieuNhapChiTiets.RemoveRange(details);
                _context.TbPhieuNhapHangs.Remove(phieuNhap);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PhieuNhapExists(Guid id)
        {
            return _context.TbPhieuNhapHangs.Any(e => e.MaPhieuNhap == id);
        }
    }
}
