using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace Order.Msv.Models
{

    public partial class OrderMsvDbContext : DbContext
    {
        public OrderMsvDbContext()
        {
        }

        public OrderMsvDbContext(DbContextOptions<OrderMsvDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TrxOrder> TrxOrders { get; set; }    

        public virtual DbSet<TrxOrdersDetail> TrxOrdersDetails { get; set; }    

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Name=ConnectionStrings:OrderMsvDBConnection");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<TrxOrder>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("orders_pkey");

                entity.ToTable("trx_orders");

                entity.HasIndex(e => e.OrderNumber, "orders_order_number_key").IsUnique();

                entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .HasColumnName("order_number");
                entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'PENDING'::character varying")
                .HasColumnName("status");
                entity.Property(e => e.TotalAmount)
                .HasPrecision(12, 2)
                .HasColumnName("total_amount");
                entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            });

            modelBuilder.Entity<TrxOrdersDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("order_details_pkey");

                entity.ToTable("trx_orders_details");

                entity.HasIndex(e => e.ProductId, "idx_order_details_product");

                entity.HasIndex(e => new { e.OrderId, e.ProductId }, "unique_order_product").IsUnique();

                entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.PricePerUnit)
                .HasPrecision(12, 2)
                .HasColumnName("price_per_unit");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Order).WithMany(p => p.TrxOrdersDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_details_order_id_fkey");
            });

            this.OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
