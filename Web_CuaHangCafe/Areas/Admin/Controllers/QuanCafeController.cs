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
    [Route("Admin/QuanCafe")]
    public class QuanCafeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuanCafeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/QuanCafe or /Admin/QuanCafe/Index
        [Route("")]
        [Route("Index")]
        [Authentication]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;
            var listItem = await _context.TbQuanCafes
                                   .OrderBy(q => q.TenQuan)
                                   .ToListAsync();

            // Sử dụng PagedList nếu cần, nếu không chỉ trả về listItem
            IPagedList<TbQuanCafe> pagedList = listItem.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        // GET: /Admin/QuanCafe/Details/{id}
        [Route("Details/{id}")]
        [Authentication]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var quanCafe = await _context.TbQuanCafes
                   .Include(nv => nv.TbNhanViens)
                     .Include(pn => pn.TbPhieuNhapHangs)
                      .ThenInclude(hd => hd.TbPhieuNhapChiTiets)
                 .Include(q => q.TbHoaDonBans) // Nạp danh sách hóa đơn bán của quán
                     .ThenInclude(hd => hd.TbChiTietHoaDonBans) // Nạp chi tiết của mỗi hóa đơn
                         .ThenInclude(ct => ct.MaSanPhamNavigation) // Nạp thông tin sản phẩm của chi tiết hóa đơn
                 .FirstOrDefaultAsync(q => q.MaQuan == id);

            if (quanCafe == null)
            {
                TempData["Message"] = "Không tìm thấy chi tiết quán café.";
                return RedirectToAction("Index", "QuanCafe");
            }
            return View(quanCafe);
        }



        // GET: /Admin/QuanCafe/Create
        [Route("Create")]
        [HttpGet]
        [Authentication]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/QuanCafe/Create
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TbQuanCafe quanCafe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quanCafe);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Thêm quán cafe thành công";
                return RedirectToAction(nameof(Index));
            }
            return View(quanCafe);
        }

        // GET: /Admin/QuanCafe/Edit/{id}
        [Route("Edit/{id:int}")]
        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var quanCafe = await _context.TbQuanCafes.FindAsync(id);
            if (quanCafe == null)
                return NotFound();

            return View(quanCafe);
        }

        // POST: /Admin/QuanCafe/Edit/{id}
        [Route("Edit/{id:int}")]
        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TbQuanCafe quanCafe)
        {
            if (id != quanCafe.MaQuan)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quanCafe);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Cập nhật thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuanCafeExists(quanCafe.MaQuan))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(quanCafe);
        }

        // GET: /Admin/QuanCafe/Delete/{id}
        [Route("Delete/{id:int}")]
        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var quanCafe = await _context.TbQuanCafes
                .FirstOrDefaultAsync(q => q.MaQuan == id);
            if (quanCafe == null)
                return NotFound();

            return View(quanCafe);
        }

        // POST: /Admin/QuanCafe/Delete/{id}
        [Route("Delete/{id:int}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authentication]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quanCafe = await _context.TbQuanCafes.FindAsync(id);
            if (quanCafe != null)
            {
                _context.TbQuanCafes.Remove(quanCafe);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Xoá thành công";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool QuanCafeExists(int id)
        {
            return _context.TbQuanCafes.Any(e => e.MaQuan == id);
        }
    }
}
