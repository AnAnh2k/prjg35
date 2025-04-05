using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[PrimaryKey("MaHoaDon", "MaSanPham")]
[Table("tbChiTietHoaDonBan")]
public partial class TbChiTietHoaDonBan
{
    [Key]
    public Guid MaHoaDon { get; set; }

    [Key]
    public int MaSanPham { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal DonGia { get; set; }

    [Column(TypeName = "decimal(21, 2)")]
    public decimal? ThanhTien { get; set; }

    [ForeignKey("MaHoaDon")]
    [InverseProperty("TbChiTietHoaDonBans")]
    public virtual TbHoaDonBan MaHoaDonNavigation { get; set; } = null!;

    [ForeignKey("MaSanPham")]
    [InverseProperty("TbChiTietHoaDonBans")]
    public virtual TbSanPham MaSanPhamNavigation { get; set; } = null!;
}
