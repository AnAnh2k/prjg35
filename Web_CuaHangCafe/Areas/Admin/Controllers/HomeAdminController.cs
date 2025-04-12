using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using Web_CuaHangCafe.ViewModels;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("HomeAdmin")]
    public class HomeAdminController : Controller
    {
        private readonly Data.ApplicationDbContext _context;
        IWebHostEnvironment _hostEnvironment;

        public HomeAdminController(Data.ApplicationDbContext context, IWebHostEnvironment hc)
        {
            _context = context;
            _hostEnvironment = hc;
        }

        [Route("")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            ViewBag.ProductGroups = _context.TbNhomSanPhams.ToList();

            int pageSize = 8;
            int pageNumber = page ?? 1;

            var query = _context.TbSanPhams
                .Include(p => p.MaNhomSpNavigation)
                .OrderBy(p => p.MaSanPham)
                .Select(p => new ProductViewModel
                {
                    MaSanPham = p.MaSanPham,
                    TenSanPham = p.TenSanPham,
                    GiaBan = p.GiaBan,
                    MoTa = p.MoTa,
                    HinhAnh = p.HinhAnh,
                    GhiChu = p.GhiChu,
                    LoaiSanPham = p.MaNhomSpNavigation.TenNhomSp
                });

            return View(query.ToPagedList(pageNumber, pageSize));
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

            var listItem = _context.TbSanPhams.AsNoTracking().Where(x => x.TenSanPham.ToLower().Contains(search)).OrderBy(x => x.MaSanPham).ToList();
            PagedList<TbSanPham> pagedListItem = new PagedList<TbSanPham>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // GET: /Access1/Create
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            // Nạp danh sách nhóm sản phẩm vào ViewBag để tạo dropdown
            ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
            return View();
        }


        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = errors
                    });
                }

                // Xử lý upload ảnh và thêm sản phẩm
                string fileName = "default.jpg";
                if (request.ImageFile != null && request.ImageFile.Length > 0)
                {
                    fileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.ImageFile.FileName)}";
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, "img/products", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.ImageFile.CopyToAsync(stream);
                    }
                }

                var product = new TbSanPham
                {
                    TenSanPham = request.TenSanPham,
                    GiaBan = request.GiaBan,
                    MaNhomSp = request.MaNhomSp,
                    MoTa = request.MoTa,
                    GhiChu = request.GhiChu,
                    HinhAnh = fileName
                };

                _context.TbSanPhams.Add(product);
                await _context.SaveChangesAsync();

                var productGroup = await _context.TbNhomSanPhams
                    .FirstOrDefaultAsync(x => x.MaNhomSp == request.MaNhomSp);

                return Json(new
                {
                    success = true,
                    product = new
                    {
                        MaSanPham = product.MaSanPham,
                        TenSanPham = product.TenSanPham,
                        HinhAnh = product.HinhAnh,
                        LoaiSanPham = productGroup?.TenNhomSp ?? "Không xác định",
                        GiaBan = product.GiaBan.ToString("N0"),
                        GhiChu = product.GhiChu ?? ""
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi server: {ex.Message}"
                });
            }
        }



        [Route("Details")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id, string name)
        {
            var productItem = (from product in _context.TbSanPhams
                               join type in _context.TbNhomSanPhams on product.MaNhomSp equals type.MaNhomSp
                               where product.MaSanPham == id
                               select new ProductViewModel
                               {
                                   MaSanPham = product.MaSanPham,
                                   TenSanPham = product.TenSanPham,
                                   GiaBan = product.GiaBan,
                                   MoTa = product.MoTa,
                                   HinhAnh = product.HinhAnh,
                                   GhiChu = product.GhiChu,
                                   LoaiSanPham = type.TenNhomSp
                               }).SingleOrDefault();

            ViewBag.name = name;

            return View(productItem);
        }

        // Trong HomeAdminController
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.TbSanPhams
                .Include(p => p.MaNhomSpNavigation)
                .FirstOrDefaultAsync(p => p.MaSanPham == id);

            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            return Json(new
            {
                success = true,
                maSanPham = product.MaSanPham,
                tenSanPham = product.TenSanPham,
                giaBan = product.GiaBan,
                maNhomSp = product.MaNhomSp,
                moTa = product.MoTa,
                ghiChu = product.GhiChu,
                hinhAnh = product.HinhAnh,
                loaiSanPham = product.MaNhomSpNavigation?.TenNhomSp
            });
        }

        [HttpPost]
        [Route("EditProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct([FromForm] ProductEditRequest request)
        {
            try
            {
                // if (!ModelState.IsValid)
                // {
                //     var errors = ModelState.Values
                //         .SelectMany(v => v.Errors)
                //         .Select(e => e.ErrorMessage)
                //         .ToList();

                //     return Json(new
                //     {
                //         success = false,
                //         message = "Dữ liệu không hợp lệ",
                //         errors = errors
                //     });
                // }
                var product = await _context.TbSanPhams.FindAsync(request.MaSanPham);
                if (product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                // Xử lý ảnh
                if (request.ImageFile != null && request.ImageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.ImageFile.FileName)}";
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, "img/products", fileName);

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(product.HinhAnh) && product.HinhAnh != "default.jpg")
                    {
                        var oldPath = Path.Combine(_hostEnvironment.WebRootPath, "img/products", product.HinhAnh);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.ImageFile.CopyToAsync(stream);
                    }
                    product.HinhAnh = "fileName";
                }

                // Cập nhật thông tin
                product.TenSanPham = request.TenSanPham;
                product.GiaBan = request.GiaBan;
                product.MaNhomSp = request.MaNhomSp;
                product.MoTa = request.MoTa;
                product.GhiChu = request.GhiChu;




                _context.Update(product);
                await _context.SaveChangesAsync();

                var productGroup = await _context.TbNhomSanPhams
                    .FirstOrDefaultAsync(x => x.MaNhomSp == request.MaNhomSp);

                return Json(new
                {
                    success = true,
                    product = new
                    {
                        MaSanPham = product.MaSanPham,
                        TenSanPham = product.TenSanPham,
                        HinhAnh = product.HinhAnh,
                        LoaiSanPham = productGroup?.TenNhomSp ?? "Không xác định",
                        GiaBan = product.GiaBan.ToString("N0"),
                        GhiChu = product.GhiChu ?? ""
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.TbSanPhams.FindAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                // Kiểm tra ràng buộc
                var hasOrders = await _context.TbChiTietHoaDonBans.AnyAsync(x => x.MaSanPham == id);
                if (hasOrders)
                {
                    return Json(new { success = false, message = "Không thể xóa sản phẩm đã có đơn hàng" });
                }

                _context.TbSanPhams.Remove(product);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Xóa thành công",
                    productId = id
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }
    }
}
