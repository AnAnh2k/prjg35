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
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listItem = (from product in _context.TbSanPhams
                            join type in _context.TbNhomSanPhams on product.MaNhomSp equals type.MaNhomSp
                            orderby product.MaSanPham
                            select new ProductViewModel
                            {
                                MaSanPham = product.MaSanPham,
                                TenSanPham = product.TenSanPham,
                                GiaBan = product.GiaBan,
                                MoTa = product.MoTa,
                                HinhAnh = product.HinhAnh,
                                GhiChu = product.GhiChu,
                                LoaiSanPham = type.TenNhomSp
                            }).ToList();

            PagedList<ProductViewModel> pagedListItem = new PagedList<ProductViewModel>(listItem, pageNumber, pageSize);

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

        // POST: /Access1/Create
        //[Route("Create")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(TbSanPham sanPham, IFormFile imageFile)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // Nếu không hợp lệ, nạp lại dropdown
        //        ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
        //        return View(sanPham);
        //    }

        //    // Nếu có file hình ảnh được upload
        //    if (imageFile != null && imageFile.Length > 0)
        //    {
        //        // Lấy đường dẫn upload: wwwroot/img/products
        //        string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "img", "products");
        //        if (!Directory.Exists(uploadFolder))
        //        {
        //            Directory.CreateDirectory(uploadFolder);
        //        }

        //        // Tạo tên file duy nhất
        //        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
        //        string filePath = Path.Combine(uploadFolder, uniqueFileName);

        //        // Lưu file vào đường dẫn trên
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            imageFile.CopyTo(stream);
        //        }

        //        // Gán tên file (hoặc đường dẫn tương đối) cho thuộc tính HinhAnh của sản phẩm
        //        sanPham.HinhAnh = uniqueFileName;
        //    }

        //    // Thêm sản phẩm vào cơ sở dữ liệu
        //    _context.TbSanPhams.Add(sanPham);
        //    int kq = _context.SaveChanges();
        //    if (kq > 0)
        //    {
        //        TempData["Message"] = "Thêm sản phẩm thành công";
        //    }
        //    else
        //    {
        //        TempData["Message"] = "Không thêm được sản phẩm";
        //    }

        //    // Chuyển hướng đến trang Index của HomeAdmin
        //    return RedirectToAction("Index", "HomeAdmin");
        //}
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SanPhamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
                return View(model);
            }

            // Nếu tên sản phẩm đã tồn tại, thêm timestamp để tạo tên riêng biệt
            string tenSanPham = model.TenSanPham;
            if (_context.TbSanPhams.Any(p => p.TenSanPham.ToLower() == tenSanPham.ToLower()))
            {
                tenSanPham = $"{tenSanPham}_{DateTime.Now:yyyyMMddHHmmssfff}";
            }


            TbSanPham sanPham = new TbSanPham
            {
                TenSanPham = tenSanPham,
                GiaBan = model.GiaBan,
                MoTa = model.MoTa,
                GhiChu = model.GhiChu,
                MaNhomSp = model.MaNhomSp
                // Hình ảnh sẽ được gán sau khi xử lý file upload
            };

            // Xử lý file upload (nếu có)
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "img", "products");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }

                sanPham.HinhAnh = uniqueFileName;
            }

            _context.TbSanPhams.Add(sanPham);
            int kq = _context.SaveChanges();

            TempData["Message"] = kq > 0 ? "Thêm sản phẩm thành công" : "Không thêm được sản phẩm";
            return RedirectToAction("Index", "HomeAdmin");
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

        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var sanPham = _context.TbSanPhams.Find(id);

            ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
            ViewBag.name = name;

            return View(sanPham);
        }

        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateProductViewModel createProduct)
        {
            string fileName = "";

            if (createProduct.HinhAnh != null)
            {
                string uploadFolder = Path.Combine(Path.Combine(_hostEnvironment.WebRootPath, "img"), "products");
                fileName = createProduct.HinhAnh.FileName;
                string filePath = Path.Combine(uploadFolder, fileName);
                createProduct.HinhAnh.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            var product = new TbSanPham
            {
                MaSanPham = createProduct.MaSanPham,
                TenSanPham = createProduct.TenSanPham,
                GiaBan = (decimal)createProduct.GiaBan,
                MoTa = createProduct.MoTa,
                HinhAnh = fileName,
                GhiChu = createProduct.GhiChu,
                MaNhomSp = createProduct.MaLoaiSanPham
            };

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            TempData["Message"] = "Sửa sản phẩm thành công";
            return RedirectToAction("Index", "HomeAdmin");
        }

        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TempData["Message"] = "";
            var chiTietHoaDon = _context.TbChiTietHoaDonBans.Where(x => x.MaSanPham == id).ToList();

            if (chiTietHoaDon.Count() > 0)
            {
                TempData["Message"] = "Không xoá được sản phẩm";

                return RedirectToAction("Index", "HomeAdmin");
            }

            _context.Remove(_context.TbSanPhams.Find(id));
            _context.SaveChanges();

            TempData["Message"] = "Sản phẩm đã được xoá";

            return RedirectToAction("Index", "HomeAdmin");
        }
    }
}
