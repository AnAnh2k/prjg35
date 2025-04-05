using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbNhomSanPham")]
public partial class TbNhomSanPham
{
    [Key]
    [Column("MaNhomSP")]
    public int MaNhomSp { get; set; }

    [Column("TenNhomSP")]
    [StringLength(255)]
    public string TenNhomSp { get; set; } = null!;

    [InverseProperty("MaNhomSpNavigation")]
    public virtual ICollection<TbSanPham> TbSanPhams { get; set; } = new List<TbSanPham>();
}
