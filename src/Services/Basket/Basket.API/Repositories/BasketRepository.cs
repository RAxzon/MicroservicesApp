using System;
using System.Threading.Tasks;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {

        private readonly IDistributedCache _redis;

        public BasketRepository(IDistributedCache redis)
        {
            _redis = redis ?? throw new ArgumentException(nameof(redis));
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await _redis.GetStringAsync(userName);
            return string.IsNullOrEmpty(basket) ? null : JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redis.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
            return await GetBasket(basket.UserName);
        }

        public async Task DeleteBasket(string UserName)
        {
            await _redis.RemoveAsync(UserName);
        }

    }
}
