using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/NguyenLieus")]
    public class NguyenLieusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NguyenLieusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách khách hàng với phân trang
        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.TbNguyenLieus.AsNoTracking()
                                .OrderBy(x => x.MaNguyenLieu)
                                .ToList();
            var pagedListItem = new PagedList<TbNguyenLieu>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // Tìm kiếm khách hàng theo tên
        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            if (string.IsNullOrEmpty(search))
            {
                return RedirectToAction("Index");
            }
            search = search.ToLower();
            ViewBag.search = search;

            var listItem = _context.TbNguyenLieus.AsNoTracking()
                                .Where(x => x.TenNguyenLieu.ToLower().Contains(search))
                                .OrderBy(x => x.MaNguyenLieu)
                                .ToList();
            var pagedListItem = new PagedList<TbNguyenLieu>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }
        // GET: Admin/NguyenLieus/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var nguyenLieu = await _context.TbNguyenLieus
                .FirstOrDefaultAsync(nl => nl.MaNguyenLieu == id);
            if (nguyenLieu == null)
                return NotFound();

            return View(nguyenLieu);
        }

        // GET: Admin/NguyenLieus/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NguyenLieus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("TenNguyenLieu,SoLuong,DonViTinh,HanSuDung,DonGia,SoLuongToiThieu")] TbNguyenLieu nguyenLieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguyenLieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguyenLieu);
        }

        // GET: Admin/NguyenLieus/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var nguyenLieu = await _context.TbNguyenLieus.FindAsync(id);
            if (nguyenLieu == null)
                return NotFound();

            return View(nguyenLieu);
        }

        // POST: Admin/NguyenLieus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("MaNguyenLieu,TenNguyenLieu,SoLuong,DonViTinh,HanSuDung,DonGia,SoLuongToiThieu")] TbNguyenLieu nguyenLieu)
        {
            if (id != nguyenLieu.MaNguyenLieu)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguyenLieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguyenLieuExists(nguyenLieu.MaNguyenLieu))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nguyenLieu);
        }

        // GET: Admin/NguyenLieus/Delete/5
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var nguyenLieu = await _context.TbNguyenLieus
                .FirstOrDefaultAsync(nl => nl.MaNguyenLieu == id);
            if (nguyenLieu == null)
                return NotFound();

            return View(nguyenLieu);
        }

        // POST: Admin/NguyenLieus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguyenLieu = await _context.TbNguyenLieus.FindAsync(id);
            if (nguyenLieu != null)
            {
                _context.TbNguyenLieus.Remove(nguyenLieu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NguyenLieuExists(int id)
        {
            return _context.TbNguyenLieus.Any(nl => nl.MaNguyenLieu == id);
        }
    }
}
