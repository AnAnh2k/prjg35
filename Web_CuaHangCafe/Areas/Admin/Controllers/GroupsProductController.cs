using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/GroupsProduct")]
    public class GroupsProductController : Controller
    {
        private readonly Data.ApplicationDbContext _context;

        public GroupsProductController(Data.ApplicationDbContext context)
        {
            _context = context;
        }

       

        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.TbNhomSanPhams.AsNoTracking().OrderBy(x => x.MaNhomSp).ToList();
            PagedList<TbNhomSanPham> pagedListItem = new PagedList<TbNhomSanPham>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        [Route("Search")]
        [HttpGet]
        public IActionResult Search(string search)
        {
            // Ví dụ: Lấy danh sách nhóm sản phẩm có tên chứa từ khóa search
            // Nếu search rỗng, trả về toàn bộ danh sách hoặc danh sách trống theo logic bạn mong muốn.
            var listItem = _context.TbNhomSanPhams
                .AsNoTracking()
                .Where(x => string.IsNullOrEmpty(search) || x.TenNhomSp.ToLower().Contains(search.ToLower()))
                .OrderBy(x => x.MaNhomSp)
                .ToList();

            // Nếu là yêu cầu AJAX, trả về PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_SearchResults", listItem);
            }
            // Nếu không, trả về View chính (ví dụ: "Search.cshtml")
            return View(listItem);
        }


        //[Route("Create")]
        //[Authentication]
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[Route("Create")]
        //[Authentication]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(TbNhomSanPham nhomSp)
        //{
        //    _context.TbNhomSanPhams.Add(nhomSp);
        //    _context.SaveChanges();

        //    TempData["Message"] = "Thêm thành công";

        //    return RedirectToAction("Index", "GroupsProduct");
        //}

        [Route("Create")]
        [Authentication]
        [HttpGet]
        public IActionResult Create()
        {
            // Nếu request là AJAX thì trả PartialView, không cần layout
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ViewBag.FormAction = "Create";
                return PartialView("_CreateEditPartial", new TbNhomSanPham());
            }
            // Nếu không phải AJAX, bạn có thể trả về full view (có layout) như cũ
            return View();
        }

        [Route("Create")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TbNhomSanPham nhomSp)
        {
            if (ModelState.IsValid)
            {
                _context.TbNhomSanPhams.Add(nhomSp);
                _context.SaveChanges();

                // Nếu là AJAX trả về JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Thêm thành công" });
                }

                TempData["Message"] = "Thêm thành công";
                return RedirectToAction("Index", "GroupsProduct");
            }
            // Nếu dữ liệu không hợp lệ
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            return View(nhomSp);
        }


        // [Route("Edit")]
        //[Authentication]
        // [HttpGet]
        // public IActionResult Edit(int id, string name)
        // {
        //     var nhomSp = _context.TbNhomSanPhams.Find(id);
        //     ViewBag.name = name;

        //     return View(nhomSp);
        // }

        // [Route("Edit")]
        // [Authentication]
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult Edit(TbNhomSanPham nhomSp)
        // {
        //     _context.Entry(nhomSp).State = EntityState.Modified;
        //     _context.SaveChanges();

        //     TempData["Message"] = "Sửa thành công";

        //     return RedirectToAction("Index", "GroupsProduct");
        // }

        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var nhomSp = _context.TbNhomSanPhams.Find(id);
            if (nhomSp == null)
            {
                return NotFound();
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ViewBag.FormAction = "Edit";
                return PartialView("_CreateEditPartial", nhomSp);
            }
            return View(nhomSp);
        }

        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TbNhomSanPham nhomSp)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(nhomSp).State = EntityState.Modified;
                _context.SaveChanges();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Sửa thành công" });
                }

                TempData["Message"] = "Sửa thành công";
                return RedirectToAction("Index", "GroupsProduct");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            return View(nhomSp);
        }


        //[Route("Delete")]
        //[Authentication]
        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    TempData["Message"] = "";

        //    var sanPham = _context.TbSanPhams.Where(x => x.MaNhomSp == id).ToList();

        //    if (sanPham.Count() > 0)
        //    {
        //        TempData["Message"] = "Xoá không thành công";
        //        return RedirectToAction("Index", "GroupsProduct");
        //    }

        //    _context.Remove(_context.TbNhomSanPhams.Find(id));
        //    _context.SaveChanges();

        //    TempData["Message"] = "Xoá thành công";

        //    return RedirectToAction("Index", "GroupsProduct");
        //}
        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TempData["Message"] = "";

            var sanPham = _context.TbSanPhams.Where(x => x.MaNhomSp == id).ToList();

            if (sanPham.Count() > 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Xóa không thành công vì có sản phẩm" });
                }
                TempData["Message"] = "Xóa không thành công vì có sản phẩm";
                return RedirectToAction("Index", "GroupsProduct");
            }

            var nhomSp = _context.TbNhomSanPhams.Find(id);
            if (nhomSp != null)
            {
                _context.TbNhomSanPhams.Remove(nhomSp);
                _context.SaveChanges();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Xóa thành công", id = id });
                }
                TempData["Message"] = "Xóa thành công";
            }
            else
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Không tìm thấy nhóm sản phẩm" });
                }
                TempData["Message"] = "Không tìm thấy nhóm sản phẩm";
            }

            return RedirectToAction("Index", "GroupsProduct");
        }

    }
}
