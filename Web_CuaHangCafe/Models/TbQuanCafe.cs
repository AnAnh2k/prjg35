using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbQuanCafe")]
[Index("Email", Name = "UQ__tbQuanCa__A9D10534DD412828", IsUnique = true)]
[Index("Sdt", Name = "UQ__tbQuanCa__CA1930A52AC5D9E9", IsUnique = true)]
public partial class TbQuanCafe
{
    [Key]
    public int MaQuan { get; set; }

    [StringLength(255)]
    public string TenQuan { get; set; } = null!;

    [StringLength(255)]
    public string DiaChi { get; set; } = null!;

    [Column("SDT")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sdt { get; set; } = null!;

    [StringLength(255)]
    public string? Email { get; set; }

    [InverseProperty("MaQuanNavigation")]
    public virtual ICollection<TbHoaDonBan> TbHoaDonBans { get; set; } = new List<TbHoaDonBan>();

    [InverseProperty("MaQuanNavigation")]
    public virtual ICollection<TbNhanVien> TbNhanViens { get; set; } = new List<TbNhanVien>();

    [InverseProperty("MaQuanNavigation")]
    public virtual ICollection<TbPhieuNhapHang> TbPhieuNhapHangs { get; set; } = new List<TbPhieuNhapHang>();
}
