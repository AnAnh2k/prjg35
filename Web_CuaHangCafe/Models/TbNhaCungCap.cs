using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbNhaCungCap")]
[Index("Sdtncc", Name = "UQ__tbNhaCun__D7706A67674D1C9B", IsUnique = true)]
public partial class TbNhaCungCap
{
    [Key]
    public int MaNhaCungCap { get; set; }

    [StringLength(255)]
    public string TenNhaCungCap { get; set; } = null!;

    [StringLength(255)]
    public string? DiaChi { get; set; }

    [Column("SDTNCC")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sdtncc { get; set; } = null!;

    [Column("STK")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Stk { get; set; }

    [InverseProperty("MaNhaCungCapNavigation")]
    public virtual ICollection<TbPhieuNhapHang> TbPhieuNhapHangs { get; set; } = new List<TbPhieuNhapHang>();
}
