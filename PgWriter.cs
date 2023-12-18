using exRate.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace exRate
{
    public class PgWriter : DbContext
    {
        public DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;

        private IConfiguration _configuration;

        public PgWriter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var user = _configuration["DATABASE_USERNAME"];
            var password = _configuration["DATABASE_PASSWORD"];
            var host = _configuration["DATABASE_HOST"];
            var port = _configuration["DATABASE_PORT"];
            optionsBuilder.UseNpgsql($"Host={host};Port={port};Database=softswiss;Username={user};Password={password}");            

        }
    }
}

