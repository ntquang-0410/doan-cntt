using System;

namespace ConvenienceStoreApp.DTOs
{
    public class LoyaltyHistoryDto
    {
        public int LogId { get; set; }
        public int? DonHang { get; set; }
        public int SoDiem { get; set; }
        public string LoaiGiaoDich { get; set; }
        public string MoTa { get; set; }
        public DateTime ThoiGian { get; set; }
    }
}
