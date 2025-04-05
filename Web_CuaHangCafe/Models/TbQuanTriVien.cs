using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_CuaHangCafe.Models;

[Table("tbQuanTriVien")]
public partial class TbQuanTriVien
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string TenNguoiDung { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;
}
