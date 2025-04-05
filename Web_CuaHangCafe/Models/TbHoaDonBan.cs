using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbHoaDonBan")]
public partial class TbHoaDonBan
{
    [Key]
    public Guid MaHoaDon { get; set; }

    public int MaQuan { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayLap { get; set; }

    public int? MaNhanVien { get; set; }

    public int MaKhachHang { get; set; }

    [StringLength(50)]
    public string HinhThucThanhToan { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TongTien { get; set; }

    [StringLength(50)]
    public string TrangThai { get; set; } = null!;


    [ForeignKey("MaKhachHang")]
    [InverseProperty("TbHoaDonBans")]
    public virtual TbKhachHang? MaKhachHangNavigation { get; set; }

    [ForeignKey("MaNhanVien")]
    [InverseProperty("TbHoaDonBans")]
    public virtual TbNhanVien MaNhanVienNavigation { get; set; } = null!;

    [ForeignKey("MaQuan")]
    [InverseProperty("TbHoaDonBans")]
    public virtual TbQuanCafe MaQuanNavigation { get; set; } = null!;

    [InverseProperty("MaHoaDonNavigation")]
    public virtual ICollection<TbChiTietHoaDonBan> TbChiTietHoaDonBans { get; set; } = new List<TbChiTietHoaDonBan>();
}
