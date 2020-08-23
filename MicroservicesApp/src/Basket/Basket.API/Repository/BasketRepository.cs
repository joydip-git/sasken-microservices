using Basket.API.Data.Interfaces;
using Basket.API.Entities;
using Basket.API.Repository.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Basket.API.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _context;

        public BasketRepository(IBasketContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> DeleteBasket(string userName)
        {
            try
            {
                return await _context.Redis.KeyDeleteAsync(userName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BasketCart> GetBasket(string userName)
        {
            try
            {
                var basket = await _context.Redis.StringGetAsync(userName);
                if (basket.IsNullOrEmpty)
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<BasketCart>(basket);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BasketCart> UpdateBasket(BasketCart basket)
        {
            try
            {
                var updated = await _context.Redis.StringSetAsync(basket.UserName, JsonConvert.SerializeObject(basket));
                return await GetBasket(basket.UserName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
