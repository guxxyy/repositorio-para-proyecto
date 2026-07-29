using Microsoft.EntityFrameworkCore;
using BILLETERACRIPTOMONEDASBACKEND.Models;

namespace BILLETERACRIPTOMONEDASBACKEND.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<Billetera> Billetera{get;set;}
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Transaccion>()
                .Property(t => t.CryptoAmount)
                .HasPrecision(18, 8); 

            modelBuilder.Entity<Transaccion>()
                .Property(t => t.MoneySpent)
                .HasPrecision(18, 2); 

            modelBuilder.Entity<Cliente>() 
                .HasOne(c=> c.Billetera) 
                .WithOne(b=>b.Cliente)
                .HasForeignKey<Billetera>(b=> b.IdCliente) 
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}