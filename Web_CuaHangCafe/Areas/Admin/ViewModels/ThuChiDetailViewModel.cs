using System;
using System.Collections.Generic;
using Web_CuaHangCafe.Models;

namespace Web_CuaHangCafe.ViewModels
{
    public class ThuChiDetailViewModel
    {
        // Thông tin lọc/chung
        public int CafeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Danh sách chi tiết các bảng liên quan
        public IEnumerable<TbNhanVien> Employees { get; set; }
        public IEnumerable<TbHoaDonBan> Invoices { get; set; }
        public IEnumerable<TbPhieuNhapHang> PhieuNhapHangs { get; set; }
    }
}
