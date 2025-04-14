using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;

namespace Web_CuaHangCafe.Controllers
{
    [Route("[controller]")]
    public class NhomSPTestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhomSPTestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action trả về view danh sách nhóm sản phẩm
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var list = _context.TbNhomSanPhams
                               .AsNoTracking()
                               .OrderBy(x => x.MaNhomSp)
                               .ToList();
            return View(list);
        }

        // Partial view cho danh sách nhóm sản phẩm (dùng AJAX)
        [HttpGet("PartialList")]
        public IActionResult PartialList()
        {
            var list = _context.TbNhomSanPhams
                               .AsNoTracking()
                               .OrderBy(x => x.MaNhomSp)
                               .ToList();
            return PartialView("PartialList", list);
        }

        // Partial view cho form thêm nhóm sản phẩm
        [Authentication]
        [HttpGet("PartialCreate")]
        public IActionResult PartialCreate()
        {
            return PartialView("PartialCreate", new TbNhomSanPham());
        }

        // Partial view cho form sửa nhóm sản phẩm – cần truyền id
        [Authentication]
        [HttpGet("PartialEdit")]
        public IActionResult PartialEdit(int id)
        {
            var nhomSp = _context.TbNhomSanPhams.Find(id);
            if (nhomSp == null)
                return NotFound();
            return PartialView("PartialEdit", nhomSp);
        }

        // Action Create – trả về JSON (dùng cho AJAX)
        [Authentication]
        [HttpPost("Create")]
        public IActionResult Create([FromBody] TbNhomSanPham nhomSp)
        {
            if (ModelState.IsValid)
            {
                _context.TbNhomSanPhams.Add(nhomSp);
                _context.SaveChanges();
                return Json(new { success = true, message = "Thêm nhóm sản phẩm thành công" });
            }
            return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
        }

        // Action Edit – trả về JSON (dùng cho AJAX)
        [Authentication]
        [HttpPost("Edit")]
        public IActionResult Edit([FromBody] TbNhomSanPham nhomSp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            _context.Entry(nhomSp).State = EntityState.Modified;
            _context.SaveChanges();
            return Json(new { success = true, message = "Cập nhật nhóm sản phẩm thành công" });
        }

        // Action Delete – trả về JSON (dùng cho AJAX)
        [Authentication]
        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] int id)
        {
            var nhomSp = _context.TbNhomSanPhams.Find(id);
            if (nhomSp == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy nhóm sản phẩm" });
            }
            _context.TbNhomSanPhams.Remove(nhomSp);
            _context.SaveChanges();
            return Json(new { success = true, message = "Xóa nhóm sản phẩm thành công" });
        }
    }
}
