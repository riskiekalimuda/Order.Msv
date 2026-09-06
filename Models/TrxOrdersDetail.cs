using System;
using System.Collections.Generic;


namespace Order.Msv.Models
{

    public partial class TrxOrdersDetail
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal PricePerUnit { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual TrxOrder Order { get; set; } = null!;
    }
}