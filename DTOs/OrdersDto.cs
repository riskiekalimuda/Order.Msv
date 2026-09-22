using Order.Msv.Models;

namespace Order.Msv.DTOs
{
    public class OrdersDto
    {
        public Guid Id { get; set; }

        public string OrderNumber { get; set; } = null!;

        public Guid CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<OrderDetailDto> OrderDetailsDto { get; set; } = new List<OrderDetailDto>();
    }

    public class OrderDetailDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal PricePerUnit { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
