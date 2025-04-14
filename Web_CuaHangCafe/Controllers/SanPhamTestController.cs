using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Web_CuaHangCafe.Models.Authentication;

namespace Web_CuaHangCafe.Controllers
{
    public class SanPhamTestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanPhamTestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SanPhamTest
        [Authentication]
        public async Task<IActionResult> Index()
        {
            var list = await _context.TbSanPhams.ToListAsync();
            return View(list);
        }

        // GET: SanPhamTest/Details/5
        [Authentication]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var sanPham = await _context.TbSanPhams
                .FirstOrDefaultAsync(sp => sp.MaSanPham == id);
            if (sanPham == null)
                return NotFound();

            return View(sanPham);
        }

        [Authentication]
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        // GET: SanPhamTest/Create
        [Authentication]
        public IActionResult Create()
        {
            // Sử dụng toán tử an toàn để tránh null khi DbSet chưa được khởi tạo hoặc chưa có bản ghi.
            ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp"); return View();
        }

        // POST: SanPhamTest/Create
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TbSanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                // Băm mật khẩu trước khi lưu vào DB
                string hashPassword = HashPassword(sanPham.VerifyKey);
                sanPham.VerifyKey = hashPassword;
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, khởi tạo lại danh sách dropdown
            ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
            return View(sanPham);
        }

        // GET: SanPhamTest/Edit/5
        [Authentication]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var sanPham = await _context.TbSanPhams.FindAsync(id);
            if (sanPham == null)
                return NotFound();

            ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
            return View(sanPham);
        }

        // POST: SanPhamTest/Edit/5
        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TbSanPham sanPham)
        {
            if (id != sanPham.MaSanPham)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TbSanPhams.Any(e => e.MaSanPham == sanPham.MaSanPham))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, khởi tạo lại danh sách dropdown
            ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
            return View(sanPham);
        }

        // GET: SanPhamTest/Delete/5 (Tùy chọn, nếu bạn muốn hiển thị trang xác nhận xóa)
        [Authentication]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var sanPham = await _context.TbSanPhams.FirstOrDefaultAsync(sp => sp.MaSanPham == id);
            if (sanPham == null)
                return NotFound();

            return View(sanPham);
        }

        // POST: SanPhamTest/Delete/5
        [Authentication]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.TbSanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.TbSanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
