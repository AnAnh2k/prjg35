using System.Collections.Generic;
using System.Linq;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;

public class TinTucRepository : ITinTucRepository
{
    private readonly ApplicationDbContext _context;

    public TinTucRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<TbTinTuc> GetAll()
    {
        return _context.TbTinTucs.ToList();
    }

    public TbTinTuc GetById(int id)
    {
        return _context.TbTinTucs.FirstOrDefault(t => t.MaTinTuc == id);
    }

    public void Create(TbTinTuc tinTuc)
    {
        tinTuc.NgayDang = DateOnly.FromDateTime(DateTime.Now); // Thiết lập ngày đăng
        _context.TbTinTucs.Add(tinTuc);
        _context.SaveChanges();
    }

    public void Update(TbTinTuc tinTuc)
    {
        var existing = GetById(tinTuc.MaTinTuc);
        if (existing != null)
        {
            existing.TieuDe = tinTuc.TieuDe;
            existing.NoiDung = tinTuc.NoiDung;
            existing.HinhAnh = tinTuc.HinhAnh;
            existing.NgayDang = DateOnly.FromDateTime(DateTime.Now); // Cập nhật ngày
            _context.TbTinTucs.Update(existing);
            _context.SaveChanges();
        }
    }

    public void Delete(int id)
    {
        var tinTuc = GetById(id);
        if (tinTuc != null)
        {
            _context.TbTinTucs.Remove(tinTuc);
            _context.SaveChanges();
        }
    }
}
