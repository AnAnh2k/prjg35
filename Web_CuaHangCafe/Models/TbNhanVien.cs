using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbNhanVien")]
[Index("SoCccd", Name = "UQ__tbNhanVi__8A547D3A9CDEE2A7", IsUnique = true)]
[Index("Email", Name = "UQ__tbNhanVi__A9D105349831608B", IsUnique = true)]
[Index("Sdt", Name = "UQ__tbNhanVi__CA1930A5FFF0118D", IsUnique = true)]
public partial class TbNhanVien
{
    [Key]
    public int MaNhanVien { get; set; }

    public int MaQuan { get; set; }

    [StringLength(255)]
    public string HoTen { get; set; } = null!;

    [StringLength(255)]
    public string? DiaChi { get; set; }

    public DateOnly? NgaySinh { get; set; }

    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [StringLength(50)]
    public string ChucVu { get; set; } = null!;

    [Column("SDT")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sdt { get; set; } = null!;

    [Column("SoCCCD")]
    [StringLength(50)]
    [Unicode(false)]
    public string SoCccd { get; set; } = null!;

    [StringLength(255)]
    public string? Email { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal LuongCoBan { get; set; }

    [Column(TypeName = "decimal(4, 2)")]
    public decimal HeSoLuong { get; set; }

    [ForeignKey("MaQuan")]
    [InverseProperty("TbNhanViens")]
    public virtual TbQuanCafe? MaQuanNavigation { get; set; }

    [InverseProperty("MaNhanVienNavigation")]
    public virtual ICollection<TbHoaDonBan> TbHoaDonBans { get; set; } = new List<TbHoaDonBan>();

    [InverseProperty("MaNhanVienNavigation")]
    public virtual ICollection<TbPhieuNhapHang> TbPhieuNhapHangs { get; set; } = new List<TbPhieuNhapHang>();

    [InverseProperty("MaNhanVienNavigation")]
    public virtual ICollection<TbTaiKhoan> TbTaiKhoans { get; set; } = new List<TbTaiKhoan>();
}
