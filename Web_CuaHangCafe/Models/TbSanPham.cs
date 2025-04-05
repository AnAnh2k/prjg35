using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbSanPham")]
[Index("TenSanPham", Name = "UQ__tbSanPha__FCA804692C227E42", IsUnique = true)]
public partial class TbSanPham
{
    [Key]
    public int MaSanPham { get; set; }

    [StringLength(255)]
    public string TenSanPham { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal GiaBan { get; set; }

    public string? MoTa { get; set; }

    public string? HinhAnh { get; set; }

    public string? GhiChu { get; set; }

    [Column("MaNhomSP")]
    public int MaNhomSp { get; set; }

    [ForeignKey("MaNhomSp")]
    [InverseProperty("TbSanPhams")]
    public virtual TbNhomSanPham? MaNhomSpNavigation { get; set; } 

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<TbChiTietHoaDonBan> TbChiTietHoaDonBans { get; set; } = new List<TbChiTietHoaDonBan>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<TbGioHang> TbGioHangs { get; set; } = new List<TbGioHang>();
}
