using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Models;
using X.PagedList;
using Web_CuaHangCafe.Data;
using System.Text.Json;

namespace Web_CuaHangCafe.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Data.ApplicationDbContext _context;
        // Biến tĩnh để lưu danh sách người dùng đã xem sản phẩm theo mã sản phẩm
        public static Dictionary<int, List<string>> ProductViewers = new Dictionary<int, List<string>>();
        public ProductsController(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            int pageSize = 18;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.TbSanPhams.AsNoTracking().OrderBy(x => x.MaSanPham).ToList();
            PagedList<TbSanPham> pagedListItem = new PagedList<TbSanPham>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        public IActionResult Type(int target, string targetName, int? page)
        {
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.TbSanPhams.AsNoTracking().Where(x => x.MaNhomSp == target).OrderBy(x => x.TenSanPham).ToList();
            PagedList<TbSanPham> pagedListItem = new PagedList<TbSanPham>(listItem, pageNumber, pageSize);

            ViewBag.target = target;
            ViewBag.targetName = targetName;

            return View(pagedListItem);
        }

        //public IActionResult Details(int id)
        //{
        //    var products = _context.TbSanPhams.SingleOrDefault(x => x.MaSanPham == id);
        //    var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
        //    {
        //        Expires = DateTime.Now.AddDays(7),
        //        HttpOnly = false
        //    };
        //    Response.Cookies.Append("lastAccess", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), cookieOptions);
        //    return View(products);
        //}

        public IActionResult Details(int id)
        {
            // Lấy sản phẩm hiện tại theo id
            var product = _context.TbSanPhams.SingleOrDefault(x => x.MaSanPham == id);
            if (product == null)
            {
                return NotFound();
            }

            // Cập nhật cookie (như cũ)
            var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = false
            };
            Response.Cookies.Append("lastAccess", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), cookieOptions);

            // Lấy danh sách "Recently Viewed" từ session (danh sách các ID sản phẩm)
            string recentJson = HttpContext.Session.GetString("RecentlyViewed");
            List<int> recentlyViewed = !string.IsNullOrEmpty(recentJson)
                                            ? JsonSerializer.Deserialize<List<int>>(recentJson)
                                            : new List<int>();

            // Nếu sản phẩm hiện tại chưa có trong danh sách thì thêm vào
            if (!recentlyViewed.Contains(id))
            {
                recentlyViewed.Add(id);
                // Nếu số lượng vượt quá 5, loại bỏ mục cũ nhất
                if (recentlyViewed.Count > 5)
                {
                    recentlyViewed.RemoveAt(0);
                }
            }
            // Lưu lại danh sách mới vào session
            HttpContext.Session.SetString("RecentlyViewed", JsonSerializer.Serialize(recentlyViewed));
                       // Lấy danh sách sản phẩm vừa xem dựa trên danh sách ID có trong session
            // (Nếu muốn sắp xếp theo thứ tự xem, có thể sắp xếp theo vị trí trong list)
            var recentProducts = _context.TbSanPhams
                                  .Where(p => recentlyViewed.Contains(p.MaSanPham))
                                  .ToList();

            // Lấy người dùng hiện tại từ session
            var currentUser = HttpContext.Session.GetString("TenTaiKhoan");

            // Cập nhật danh sách người dùng đã xem sản phẩm này
            if (!string.IsNullOrEmpty(currentUser))
            {
                // Dùng lock để đảm bảo thread-safe đối với biến tĩnh
                lock (ProductViewers)
                {
                    if (!ProductViewers.ContainsKey(id))
                    {
                        ProductViewers[id] = new List<string>();
                    }
                    // Nếu người dùng chưa được thêm và hiện có (tránh trùng lặp)
                    if (!ProductViewers[id].Contains(currentUser))
                    {
                        ProductViewers[id].Add(currentUser);
                    }
                }
            }

            // Lấy danh sách người xem sản phẩm hiện tại từ biến tĩnh
            List<string> viewers = new List<string>();
            if (ProductViewers.ContainsKey(id))
            {
                viewers = ProductViewers[id];
            }
            ViewBag.Viewers = viewers;
            // Truyền danh sách các sản phẩm vừa xem qua ViewBag
            ViewBag.RecentProducts = recentProducts;

            return View(product);
        }

    }
}
