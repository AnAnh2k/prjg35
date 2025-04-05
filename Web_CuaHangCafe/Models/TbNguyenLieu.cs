using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbNguyenLieu")]
public partial class TbNguyenLieu
{
    [Key]
    public int MaNguyenLieu { get; set; }

    [StringLength(255)]
    public string TenNguyenLieu { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal SoLuong { get; set; }

    [StringLength(50)]
    public string DonViTinh { get; set; } = null!;

    public DateOnly? HanSuDung { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal DonGia { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal SoLuongToiThieu { get; set; }

    [InverseProperty("MaNguyenLieuNavigation")]
    public virtual ICollection<TbPhieuNhapChiTiet> TbPhieuNhapChiTiets { get; set; } = new List<TbPhieuNhapChiTiet>();
}
