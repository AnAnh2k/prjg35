using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbTaiKhoan")]
[Index("TenTaiKhoan", Name = "UQ__tbTaiKho__B106EAF8373AC22F", IsUnique = true)]
public partial class TbTaiKhoan
{
    [Key]
    public int MaTaiKhoan { get; set; }

    [StringLength(255)]
    public string TenTaiKhoan { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;

    public int MaNhanVien { get; set; }

    public int MaQuyen { get; set; }

    [ForeignKey("MaNhanVien")]
    [InverseProperty("TbTaiKhoans")]
    public virtual TbNhanVien? MaNhanVienNavigation { get; set; }

    [ForeignKey("MaQuyen")]
    [InverseProperty("TbTaiKhoans")]
    public virtual TbQuyen? MaQuyenNavigation { get; set; } 
}
