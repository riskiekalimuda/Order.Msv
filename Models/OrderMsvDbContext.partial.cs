using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Order.Msv.Models
{
    public partial class OrderMsvDbContext : DbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.AddTransactionalOutboxEntities();
        }   
    }
}
