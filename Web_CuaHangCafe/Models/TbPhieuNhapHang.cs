using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbPhieuNhapHang")]
public partial class TbPhieuNhapHang
{
    [Key]
    public Guid MaPhieuNhap { get; set; }

    public int MaQuan { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayLap { get; set; }

    public int MaNhanVien { get; set; }

    public int MaNhaCungCap { get; set; }

    [StringLength(255)]
    public string? GhiChu { get; set; }

    [ForeignKey("MaNhaCungCap")]
    [InverseProperty("TbPhieuNhapHangs")]
    public virtual TbNhaCungCap MaNhaCungCapNavigation { get; set; } = null!;

    [ForeignKey("MaNhanVien")]
    [InverseProperty("TbPhieuNhapHangs")]
    public virtual TbNhanVien MaNhanVienNavigation { get; set; } = null!;

    [ForeignKey("MaQuan")]
    [InverseProperty("TbPhieuNhapHangs")]
    public virtual TbQuanCafe MaQuanNavigation { get; set; } = null!;

    [InverseProperty("MaPhieuNhapNavigation")]
    public virtual ICollection<TbPhieuNhapChiTiet> TbPhieuNhapChiTiets { get; set; } = new List<TbPhieuNhapChiTiet>();
}
