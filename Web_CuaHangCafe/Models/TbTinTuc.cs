using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbTinTuc")]
public partial class TbTinTuc
{
    [Key]
    public int MaTinTuc { get; set; }

    [StringLength(255)]
    public string TieuDe { get; set; } = null!;

    public DateOnly NgayDang { get; set; }

    public string NoiDung { get; set; } = null!;

    [StringLength(255)]
    public string? HinhAnh { get; set; }
}
