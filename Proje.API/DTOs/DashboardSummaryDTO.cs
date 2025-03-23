using System.Collections.Generic;

namespace Proje.API.DTOs
{
    public class DashboardSummaryDTO
    {
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<OrderDTO> RecentOrders { get; set; }
        public List<ChartDataDTO> MonthlyRevenue { get; set; }
        public List<TopProductDTO> TopSellingProducts { get; set; }
    }

    public class ChartDataDTO
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
    }

    public class TopProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalSales { get; set; }
        public decimal Revenue { get; set; }
    }
}