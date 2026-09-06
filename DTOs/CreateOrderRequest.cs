using System;
using System.Collections.Generic;

namespace Order.Msv.DTOs
{
    public class CreateOrderRequest
    {
        public string OrderNumber { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<CreateOrderDetailRequest> TrxOrdersDetails { get; set; } = new List<CreateOrderDetailRequest>();
    }

    public class CreateOrderDetailRequest
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal PricePerUnit { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
