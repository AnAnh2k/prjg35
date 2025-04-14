using Azure;
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
    [Route("Admin/NewsManage")]
    public class NewsManageController : Controller
    {
        private readonly Data.ApplicationDbContext _context;

        public NewsManageController(Data.ApplicationDbContext context)
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
            var listItem = _context.TbTinTucs.AsNoTracking().OrderBy(x => x.MaTinTuc).ToList();
            PagedList<TbTinTuc> pagedListItem = new PagedList<TbTinTuc>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            search = search.ToLower();
            ViewBag.search = search;

            var listItem = _context.TbTinTucs.AsNoTracking().Where(x => x.TieuDe.ToLower().Contains(search)).OrderBy(x => x.MaTinTuc).ToList();
            PagedList<TbTinTuc> pagedListItem = new PagedList<TbTinTuc>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        //[Route("Create")]
        //[Authentication]
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    ViewBag.NguoiDang = new SelectList(_context.TbQuanTriViens.ToList(), "TenNguoiDung", "TenNguoiDung");

        //    return View();
        //}

        [Route("Create")]
        [Authentication]
        [HttpGet]
        public IActionResult Create()
        {
            //ViewBag.NguoiDang = new SelectList(_context.TbQuanTriViens.ToList(), "TenNguoiDung", "TenNguoiDung");
            // Nếu request là AJAX thì trả PartialView, không cần layout
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ViewBag.FormAction = "Create";
                return PartialView("_CreateEditPartial", new TbTinTuc());
            }
            // Nếu không phải AJAX, bạn có thể trả về full view (có layout) như cũ
            return View();
        }

        //[Route("Create")]
        //[Authentication]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(TbTinTuc tinTuc)
        //{
        //    _context.TbTinTucs.Add(tinTuc);
        //    _context.SaveChanges();

        //    TempData["Message"] = "Thêm thành công";

        //    return RedirectToAction("Index", "NewsManage");
        //}


        [Route("Create")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TbTinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                _context.TbTinTucs.Add(tinTuc);
            _context.SaveChanges();
                // Nếu là AJAX trả về JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Thêm thành công" });
                }
                TempData["Message"] = "Thêm thành công";

            return RedirectToAction("Index", "NewsManage");
            }
            // Nếu dữ liệu không hợp lệ
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            return View(tinTuc);

        }

        [Route("Details")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id, string name)
        {
            var tinTuc = _context.TbTinTucs.SingleOrDefault(x => x.MaTinTuc == id);
            ViewBag.name = name;

            // Nếu là AJAX thì trả về PartialView không có layout
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DetailsPartial", tinTuc);
            }

            return View(tinTuc);
        }

        //[Route("Edit")]
        //[Authentication]
        //[HttpGet]
        //public IActionResult Edit(int id, string name)
        //{
        //    var tinTuc = _context.TbTinTucs.Find(id);

        //    ViewBag.NguoiDang = new SelectList(_context.TbQuanTriViens.ToList(), "TenNguoiDung", "TenNguoiDung");
        //    ViewBag.name = name;

        //    return View(tinTuc);
        //}

        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var tinTuc = _context.TbTinTucs.Find(id);

            if (tinTuc == null)
            {
                return NotFound();
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                ViewBag.FormAction = "Edit";
                return PartialView("_CreateEditPartial", tinTuc);
            }
            return View(tinTuc);
            //ViewBag.NguoiDang = new SelectList(_context.TbQuanTriViens.ToList(), "TenNguoiDung", "TenNguoiDung");
            //ViewBag.name = name;


        }

        //[Route("Edit")]
        //[Authentication]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(TbTinTuc tinTuc)
        //{
        //    _context.Entry(tinTuc).State = EntityState.Modified;
        //    _context.SaveChanges();

        //    TempData["Message"] = "Sửa thành công";

        //    return RedirectToAction("Index", "NewsManage");
        //}

        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TbTinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(tinTuc).State = EntityState.Modified;
                _context.SaveChanges();


                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Sửa thành công" });
                }
                TempData["Message"] = "Sửa thành công";

                return RedirectToAction("Index", "NewsManage");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            return View(tinTuc);

        }

        //[Route("Delete")]
        //[Authentication]
        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    TempData["Message"] = "";

        //    _context.Remove(_context.TbTinTucs.Find(id));
        //    _context.SaveChanges();

        //    TempData["Message"] = "Xoá thành công";

        //    return RedirectToAction("Index", "NewsManage");
        //}

        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TempData["Message"] = "";

            var tintuc = _context.TbTinTucs.Find(id);
            if (tintuc != null)
            {
                _context.TbTinTucs.Remove(tintuc);
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

            return RedirectToAction("Index", "NewsManage");
        }
    }
}
