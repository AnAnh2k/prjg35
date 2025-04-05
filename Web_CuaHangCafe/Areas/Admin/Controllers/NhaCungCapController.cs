using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using X.PagedList;
using System.Linq;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/NhaCungCap")]
    public class NhaCungCapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhaCungCapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách nhà cung cấp với phân trang
        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 1) ? 1 : page.Value;
            var listItem = _context.TbNhaCungCaps.AsNoTracking()
                                 .OrderBy(x => x.TenNhaCungCap)
                                 .ToList();
            var pagedListItem = new PagedList<TbNhaCungCap>(listItem, pageNumber, pageSize);
            return View(pagedListItem);
        }

        // Tìm kiếm nhà cung cấp theo tên
        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 1) ? 1 : page.Value;

            if (string.IsNullOrEmpty(search))
            {
                return RedirectToAction("Index");
            }
            search = search.ToLower();
            ViewBag.search = search;

            var listItem = _context.TbNhaCungCaps.AsNoTracking()
                                .Where(x => x.TenNhaCungCap.ToLower().Contains(search))
                                .OrderBy(x => x.TenNhaCungCap)
                                .ToList();
            var pagedListItem = new PagedList<TbNhaCungCap>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // GET: Admin/NhaCungCap/Create
        [Route("Create")]
        [Authentication]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhaCungCap/Create
        [Route("Create")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TbNhaCungCap supplier)
        {
            if (ModelState.IsValid)
            {
                _context.TbNhaCungCaps.Add(supplier);
                _context.SaveChanges();
                TempData["Message"] = "Thêm Nhà Cung Cấp thành công.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Admin/NhaCungCap/Edit
        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var supplier = _context.TbNhaCungCaps.Find(id);
            if (supplier == null)
            {
                TempData["Message"] = "Không tìm thấy Nhà Cung Cấp cần sửa.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // POST: Admin/NhaCungCap/Edit
        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TbNhaCungCap supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(supplier).State = EntityState.Modified;
                _context.SaveChanges();
                TempData["Message"] = "Sửa Nhà Cung Cấp thành công.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Admin/NhaCungCap/Details/{id}
        [Route("Details/{id}")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var supplier = _context.TbNhaCungCaps.AsNoTracking()
                                .FirstOrDefault(x => x.MaNhaCungCap == id);
            if (supplier == null)
            {
                TempData["Message"] = "Không tìm thấy Nhà Cung Cấp.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Admin/NhaCungCap/Delete?id=...
        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            TempData["Message"] = "";

            if (!int.TryParse(id, out int supplierId))
            {
                TempData["Message"] = "Sai định dạng mã Nhà Cung Cấp.";
                return RedirectToAction("Index");
            }

            var supplier = _context.TbNhaCungCaps.Find(supplierId);
            if (supplier == null)
            {
                TempData["Message"] = "Không tìm thấy Nhà Cung Cấp cần xóa.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // POST: Admin/NhaCungCap/Delete
        [Route("Delete")]
        [Authentication]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var supplier = _context.TbNhaCungCaps.Find(id);
            if (supplier == null)
            {
                TempData["Message"] = "Không tìm thấy Nhà Cung Cấp cần xóa.";
                return RedirectToAction("Index");
            }

            // Nếu Nhà Cung Cấp có các dữ liệu phụ thuộc (ví dụ: Phiếu nhập hàng) thì có thể cần xử lý
            // hoặc để Cascade Delete nếu cấu hình ràng buộc trong DbContext/CSDL.

            _context.TbNhaCungCaps.Remove(supplier);
            _context.SaveChanges();

            TempData["Message"] = "Xóa Nhà Cung Cấp thành công.";
            return RedirectToAction("Index");
        }
    }
}
