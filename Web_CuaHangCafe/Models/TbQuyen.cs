using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbQuyen")]
[Index("TenQuyen", Name = "UQ__tbQuyen__5637EE7996D565AA", IsUnique = true)]
public partial class TbQuyen
{
    [Key]
    public int MaQuyen { get; set; }

    [StringLength(255)]
    public string TenQuyen { get; set; } = null!;

    [InverseProperty("MaQuyenNavigation")]
    public virtual ICollection<TbTaiKhoanKh> TbTaiKhoanKhs { get; set; } = new List<TbTaiKhoanKh>();

    [InverseProperty("MaQuyenNavigation")]
    public virtual ICollection<TbTaiKhoan> TbTaiKhoans { get; set; } = new List<TbTaiKhoan>();
}
