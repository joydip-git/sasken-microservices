using Basket.API.Data.Interfaces;
using StackExchange.Redis;
using System;

namespace Basket.API.Data
{
    public class BasketContext : IBasketContext
    {
        private readonly ConnectionMultiplexer _redisConnection;

        public BasketContext(ConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
            Redis = _redisConnection.GetDatabase();
        }

        public IDatabase Redis { get; }
    }
}
