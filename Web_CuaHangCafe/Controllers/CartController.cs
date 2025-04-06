using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web_CuaHangCafe.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: /Cart/Index
        public async Task<IActionResult> Index()
        {
            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangStr))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập.
                return RedirectToAction("Login1", "Access1");
            }
            int maKhachHang = int.Parse(maKhachHangStr);

            var cartItems = await _context.TbGioHangs
                .Include(g => g.MaSanPhamNavigation)
                .Where(g => g.MaKhachHang == maKhachHang)
                .ToListAsync();

            decimal tongTien = cartItems.Sum(item => item.SoLuong * item.MaSanPhamNavigation.GiaBan);
            ViewData["total"] = tongTien.ToString("n0");

            return View(cartItems);
        }

        // Thêm sản phẩm vào giỏ hàng
        //public async Task<IActionResult> Add(int id, int quantity)
        //{
        //    string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
        //    if (string.IsNullOrEmpty(maKhachHangStr))
        //    {
        //        return RedirectToAction("Login1", "Access1");
        //    }
        //    int maKhachHang = int.Parse(maKhachHangStr);

        //    // Kiểm tra xem sản phẩm có tồn tại không
        //    var product = await _context.TbSanPhams.FirstOrDefaultAsync(p => p.MaSanPham == id);
        //    if (product == null)
        //    {
        //        return NotFound("Sản phẩm không tồn tại.");
        //    }

        //    // Tìm xem đã có mục giỏ hàng cho sản phẩm của khách hàng chưa
        //    var cartItem = await _context.TbGioHangs
        //        .FirstOrDefaultAsync(g => g.MaKhachHang == maKhachHang && g.MaSanPham == id);

        //    if (cartItem == null)
        //    {
        //        // Thêm mới một mục giỏ hàng
        //        cartItem = new TbGioHang
        //        {
        //            MaKhachHang = maKhachHang,
        //            MaSanPham = id,
        //            SoLuong = quantity
        //        };
        //        _context.TbGioHangs.Add(cartItem);
        //    }
        //    else
        //    {
        //        // Nếu đã có, cập nhật số lượng
        //        cartItem.SoLuong += quantity;
        //        _context.TbGioHangs.Update(cartItem);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        // Tạo file DTO (AddItemRequest.cs) nếu chưa có
        public class AddItemRequest
        {
            public int id { get; set; }
            public int quantity { get; set; }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromBody] AddItemRequest request)
        {
            // Kiểm tra dữ liệu đầu vào
            if (request == null || request.id <= 0 || request.quantity <= 0)
            {
                return Json(new { success = false, message = "Thông tin không hợp lệ." });
            }
            // Debug log - bạn có thể ghi ra Console hoặc dùng logger của ASP.NET Core
            Console.WriteLine("Received AddItemRequest: id={0}, quantity={1}", request.id, request.quantity);
            // Kiểm tra đăng nhập qua session
            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangStr))
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });
            }
            int maKhachHang = int.Parse(maKhachHangStr);

            // Kiểm tra xem sản phẩm có tồn tại không
            var product = await _context.TbSanPhams.FirstOrDefaultAsync(p => p.MaSanPham == request.id);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            // Tìm xem đã có mục giỏ hàng của sản phẩm cho khách hàng chưa
            var cartItem = await _context.TbGioHangs
                .FirstOrDefaultAsync(g => g.MaKhachHang == maKhachHang && g.MaSanPham == request.id);

            if (cartItem == null)
            {
                // Nếu chưa có, thêm mới mục giỏ hàng
                cartItem = new TbGioHang
                {
                    MaKhachHang = maKhachHang,
                    MaSanPham = request.id,
                    SoLuong = request.quantity
                };
                _context.TbGioHangs.Add(cartItem);
            }
            else
            {
                // Nếu đã có, cập nhật số lượng
                cartItem.SoLuong += request.quantity;
                _context.TbGioHangs.Update(cartItem);
            }

            await _context.SaveChangesAsync();

            // Tính tổng số lượng sản phẩm trong giỏ hàng
            int cartCount = await _context.TbGioHangs
                .Where(g => g.MaKhachHang == maKhachHang)
                .SumAsync(g => g.SoLuong);

            return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng.", cartCount = cartCount });
        }



        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangStr))
            {
                return Json(0);
            }
            int maKhachHang = int.Parse(maKhachHangStr);

            var cartItems = await _context.TbGioHangs
                .Where(g => g.MaKhachHang == maKhachHang)
                .ToListAsync();

            int cartItemCount = cartItems.Sum(item => (int)item.SoLuong);
            return Json(cartItemCount);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] List<UpdateQuantityRequest> updates)
        {
            if (updates == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }
            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangStr))
            {
                return BadRequest("Không đăng nhập.");
            }
            int maKhachHang = int.Parse(maKhachHangStr);

            try
            {
                foreach (var update in updates)
                {
                    // Kiểm tra số lượng hợp lệ
                    if (update.Quantity < 1)
                        return BadRequest("Số lượng phải lớn hơn 0.");

                    var cartItem = await _context.TbGioHangs
                        .FirstOrDefaultAsync(g => g.MaKhachHang == maKhachHang && g.MaSanPham == update.ProductId);
                    if (cartItem == null)
                    {
                        return NotFound($"Không tìm thấy sản phẩm {update.ProductId} trong giỏ hàng.");
                    }

                    cartItem.SoLuong = update.Quantity;
                    _context.TbGioHangs.Update(cartItem);
                }

                await _context.SaveChangesAsync();

                // Lấy lại các mục giỏ hàng để tính tổng (không gửi toàn bộ đối tượng phức tạp về client)
                var updatedCartItems = await _context.TbGioHangs
                    .Include(g => g.MaSanPhamNavigation)
                    .Where(g => g.MaKhachHang == maKhachHang)
                    .ToListAsync();

                decimal totalAmount = updatedCartItems.Sum(item => item.SoLuong * item.MaSanPhamNavigation.GiaBan);
                int totalItems = updatedCartItems.Sum(item => (int)item.SoLuong);

                // Trả về các dữ liệu đơn giản thay vì cả đối tượng cartItems
                return Json(new
                {
                    success = true,
                    message = "Số lượng đã được cập nhật.",
                    totalAmount = totalAmount,
                    totalItems = totalItems
                });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần: ex.Message, ex.StackTrace
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public class UpdateQuantityRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }


        public class RemoveItemRequest
        {
            public int maSp { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove([FromBody] RemoveItemRequest request)
        {
            // Kiểm tra mã khách hàng từ session
            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangStr))
            {
                return Json(new { success = false, message = "Không đăng nhập." });
            }
            int maKhachHang = int.Parse(maKhachHangStr);

            int maSp = request.maSp;
            if (maSp <= 0)
            {
                return Json(new { success = false, message = "Mã sản phẩm không hợp lệ." });
            }

            // Tìm sản phẩm trong giỏ hàng
            var cartItem = await _context.TbGioHangs
                .FirstOrDefaultAsync(c => c.MaKhachHang == maKhachHang && c.MaSanPham == maSp);

            if (cartItem != null)
            {
                // Xóa sản phẩm
                _context.TbGioHangs.Remove(cartItem);
                await _context.SaveChangesAsync();

                // Tính tổng tiền và tổng sản phẩm
                var totalAmount = await _context.TbGioHangs
                    .Where(c => c.MaKhachHang == maKhachHang)
                    .SumAsync(item => item.SoLuong * item.MaSanPhamNavigation.GiaBan);

                var totalItems = await _context.TbGioHangs
                    .Where(c => c.MaKhachHang == maKhachHang)
                    .SumAsync(item => (int)item.SoLuong);

                return Json(new
                {
                    success = true,
                    message = "Đã xoá sản phẩm.",
                    totalAmount = totalAmount,
                    totalItems = totalItems
                });
            }
            else
            {
                return Json(new { success = false, message = "Không có sản phẩm." });
            }
        }

        public async Task<IActionResult> Checkout()
        {
            ViewBag.Quan = new SelectList(_context.TbQuanCafes, "MaQuan", "TenQuan");

            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangStr))
            {
                return RedirectToAction("Login1", "Access1");
            }
            int maKhachHang = int.Parse(maKhachHangStr);

            // Lấy các mục trong giỏ hàng
            var cartItems = await _context.TbGioHangs
                .Include(g => g.MaSanPhamNavigation)
                
                .Where(c => c.MaKhachHang == maKhachHang)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // Tính tổng tiền
            decimal tongTien = cartItems.Sum(item => item.SoLuong * item.MaSanPhamNavigation.GiaBan);

            // Lấy thông tin khách hàng dựa trên MaKhachHang được lưu trong session
            var customer = await _context.TbKhachHangs.FindAsync(maKhachHang);

            var model = new Web_CuaHangCafe.ViewModels.CheckoutViewModel
            {
                CartItems = cartItems,
                Customer = customer,
                Total = tongTien.ToString("n0")
            };

            return View(model);
        }


        public async Task<IActionResult> Confirmation(string customerName, string phoneNumber, string address, string checkoutMethod,int MaQuan)
        {
            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(maKhachHangStr))
            {
                return RedirectToAction("Login1", "Access1");
            }
            int maKhachHang = int.Parse(maKhachHangStr);

            var cartItems = await _context.TbGioHangs
                .Include(g => g.MaSanPhamNavigation)
                .Where(c => c.MaKhachHang == maKhachHang)
                .ToListAsync();
            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // Sử dụng biến maKhachHang được khai báo ở đây
            var customer = await _context.TbKhachHangs.FindAsync(maKhachHang);
            if (customer == null)
            {
                // Xử lý khi không tìm thấy khách hàng hoặc tạo mới nếu cần
            }
            else
            {
                // Nếu cần, cập nhật thông tin khách hàng
                customer.TenKhachHang = customerName;
                // Nếu bạn không cho phép chỉnh sửa, thì bạn có thể bỏ qua cập nhật số điện thoại hay địa chỉ
                _context.TbKhachHangs.Update(customer);
                await _context.SaveChangesAsync();
            }

            // Tạo hóa đơn mới cho khách hàng
            var order = new TbHoaDonBan
            {
                MaHoaDon = Guid.NewGuid(),
                MaQuan = MaQuan,
                NgayLap = DateTime.Now,
                MaNhanVien = null,
                MaKhachHang = customer.MaKhachHang,
                HinhThucThanhToan = checkoutMethod,
                TongTien = cartItems.Sum(x => x.SoLuong * x.MaSanPhamNavigation.GiaBan),
                TrangThai = "Chưa hoàn thành",
             
            };
            _context.TbHoaDonBans.Add(order);
            await _context.SaveChangesAsync();

            // Thêm chi tiết hóa đơn cho từng sản phẩm trong giỏ
            foreach (var cartItem in cartItems)
            {
                var orderDetail = new TbChiTietHoaDonBan
                {
                    MaHoaDon = order.MaHoaDon,
                    MaSanPham = cartItem.MaSanPham,
                    DonGia = cartItem.MaSanPhamNavigation.GiaBan,
                    SoLuong = cartItem.SoLuong,
                };
                _context.TbChiTietHoaDonBans.Add(orderDetail);
            }
            await _context.SaveChangesAsync();

            // Xoá các mặt hàng giỏ hàng của khách hàng sau khi tạo hóa đơn
            var cartRecords = _context.TbGioHangs.Where(c => c.MaKhachHang == maKhachHang);
            _context.TbGioHangs.RemoveRange(cartRecords);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success");
        }


        public IActionResult Success()
        {
            return View();
        }
    }
}
