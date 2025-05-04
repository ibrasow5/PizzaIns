using Microsoft.EntityFrameworkCore;
using PizzaIns.Models;

namespace PizzaIns.Data
{
    public class PizzaDbContext : DbContext
    {
        public PizzaDbContext(DbContextOptions<PizzaDbContext> options)
            : base(options) { }

        public DbSet<Models.PizzaIns> Pizzas => Set<Models.PizzaIns>();
    }
}
