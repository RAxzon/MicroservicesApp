using System;
using Discount.API.Entities;
using Discount.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {

        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection _connection;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }

        public async Task<IEnumerable<Coupon>> GetDiscounts()
        {
            var coupons = await _connection.QueryAsync<Coupon>("SELECT * FROM Coupon");
            return coupons;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var coupon = await _connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", productName);
            return coupon;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var insert = await _connection.ExecuteAsync(
                "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                new {coupon.ProductName, coupon.Description, coupon.Amount
        });

            return insert != 0;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var updated = await _connection.ExecuteAsync(
                "UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE ProductName = @productName", 
                new {coupon.ProductName, coupon.Description, coupon.Amount});

            return updated != 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var deleted = await _connection.ExecuteAsync("DELETE FROM Coupon WHERE productName = @ProductName", productName);

            return deleted != 0;
        }

    }
}
