using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_CuaHangCafe.ViewModels
{
    public class ThuChiItemViewModel
    {
        public int MaQuan { get; set; }
        public string TenQuan { get; set; }
        public decimal TotalInvoice { get; set; }     // Tổng tiền từ hóa đơn bán của quán
        public decimal TotalSalary { get; set; }        // Tiền lương nhân viên của quán
        public decimal TotalRevenue { get; set; }       // Tổng Thu = TotalInvoice + TotalSalary
        public decimal TotalImport { get; set; }        // Tổng tiền phiếu nhập của quán
        public decimal Profit { get; set; }             // Lãi = TotalRevenue - TotalImport
    }

    public class AllThuChiViewModel
    {
        // Thông tin lọc chung
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }

        // Danh sách dữ liệu thu chi cho từng quán
        public List<ThuChiItemViewModel> Items { get; set; } = new List<ThuChiItemViewModel>();
    }
}
