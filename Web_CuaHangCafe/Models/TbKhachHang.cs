using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbKhachHang")]
[Index("SdtkhachHang", Name = "UQ__tbKhachH__C87280B09C8E9586", IsUnique = true)]
public partial class TbKhachHang
{
    [Key]
    public int MaKhachHang { get; set; }

    [StringLength(255)]
    public string TenKhachHang { get; set; } = null!;

    [Column("SDTKhachHang")]
    [StringLength(10)]
    [Unicode(false)]
    public string SdtkhachHang { get; set; } = null!;

    [StringLength(255)]
    public string DiaChi { get; set; } = null!;

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<TbGioHang> TbGioHangs { get; set; } = new List<TbGioHang>();

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<TbHoaDonBan> TbHoaDonBans { get; set; } = new List<TbHoaDonBan>();

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<TbTaiKhoanKh> TbTaiKhoanKhs { get; set; } = new List<TbTaiKhoanKh>();
}
