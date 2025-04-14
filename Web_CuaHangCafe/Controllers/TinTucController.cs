using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;

namespace Web_CuaHangCafe.Controllers
{
    [Route("[controller]")]
    public class TinTucController : Controller
    {
        private readonly ITinTucRepository _repository;

        public TinTucController(ITinTucRepository repository)
        {
            _repository = repository;
        }

        // Action trả về view chính (Index) nếu cần, hoặc có thể giữ cho load AJAX từ sidebar.
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // Phục vụ view chứa partial container, nếu danh sách cũng load qua AJAX thì có thể trả về một View rỗng.
            var list = _repository.GetAll();
            return View(list);
        }

        // Partial view cho danh sách tin tức
        [HttpGet("PartialList")]
        public IActionResult PartialList()
        {
            var list = _repository.GetAll();
            return PartialView("PartialList", list);
        }

        // Partial view cho form thêm tin tức
        [Authentication]
        [HttpGet("PartialCreate")]
        public IActionResult PartialCreate()
        {
            return PartialView("PartialCreate", new TbTinTuc());
        }

        // Partial view cho form sửa tin tức – cần truyền id, ví dụ qua query string
        [Authentication]
        [HttpGet("PartialEdit")]
        public IActionResult PartialEdit(int id)
        {
            var tinTuc = _repository.GetById(id);
            if (tinTuc == null)
                return NotFound();
            return PartialView("PartialEdit", tinTuc);
        }

        // Action Create, Edit, Delete trả về JSON (dùng cho AJAX)
        [Authentication]
        [HttpPost("Create")]
        public IActionResult Create([FromBody] TbTinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                _repository.Create(tinTuc);
                return Json(new { success = true, message = "Tin tức đã được thêm thành công" });
            }
            return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
        }

        [Authentication]
        [HttpPost("Edit")]
        public IActionResult Edit([FromBody] TbTinTuc tinTuc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            _repository.Update(tinTuc);
            return Json(new { success = true, message = "Tin tức đã được cập nhật thành công" });
        }

        [Authentication]
        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] int id)
        {
            _repository.Delete(id);
            return Json(new { success = true, message = "Tin tức đã được xóa thành công" });
        }

        // API để lấy dữ liệu tin tức theo id (cho modal sửa trong partial list nếu cần)
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var tinTuc = _repository.GetById(id);
            if (tinTuc == null)
                return NotFound();
            return Json(new
            {
                maTinTuc = tinTuc.MaTinTuc,
                tieuDe = tinTuc.TieuDe,
                noiDung = tinTuc.NoiDung,
                ngayDang = tinTuc.NgayDang
            });
        }
    }
}
