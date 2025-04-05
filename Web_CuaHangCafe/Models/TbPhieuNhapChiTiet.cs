using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[PrimaryKey("MaPhieuNhap", "MaNguyenLieu")]
[Table("tbPhieuNhapChiTiet")]
public partial class TbPhieuNhapChiTiet
{
    [Key]
    public Guid MaPhieuNhap { get; set; }

    [Key]
    public int MaNguyenLieu { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal SoLuong { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal DonGia { get; set; }

    [Column(TypeName = "decimal(21, 4)")]
    public decimal? ThanhTien { get; set; }

    [ForeignKey("MaNguyenLieu")]
    [InverseProperty("TbPhieuNhapChiTiets")]
    public virtual TbNguyenLieu MaNguyenLieuNavigation { get; set; } = null!;

    [ForeignKey("MaPhieuNhap")]
    [InverseProperty("TbPhieuNhapChiTiets")]
    public virtual TbPhieuNhapHang MaPhieuNhapNavigation { get; set; } = null!;
}
