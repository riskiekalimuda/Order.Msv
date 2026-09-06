using System;
using System.Collections.Generic;


namespace Order.Msv.Models
{

    public partial class TrxOrder
    {
        public Guid Id { get; set; }

        public string OrderNumber { get; set; } = null!;

        public Guid CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<TrxOrdersDetail> TrxOrdersDetails { get; set; } = new List<TrxOrdersDetail>();
    }
}