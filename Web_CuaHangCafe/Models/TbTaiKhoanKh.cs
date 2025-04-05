using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbTaiKhoanKH")]
[Index("TenTaiKhoan", Name = "UQ__tbTaiKho__B106EAF862EE1876", IsUnique = true)]
public partial class TbTaiKhoanKh
{
    [Key]
    public int MaTaiKhoan { get; set; }

    [StringLength(255)]
    public string TenTaiKhoan { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;

    public int MaKhachHang { get; set; }

    public int MaQuyen { get; set; }

    [ForeignKey("MaKhachHang")]
    [InverseProperty("TbTaiKhoanKhs")]
    public virtual TbKhachHang MaKhachHangNavigation { get; set; } = null!;

    [ForeignKey("MaQuyen")]
    [InverseProperty("TbTaiKhoanKhs")]
    public virtual TbQuyen MaQuyenNavigation { get; set; } = null!;
}
