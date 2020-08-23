﻿using Basket.API.Entities;
using System.Threading.Tasks;

namespace Basket.API.Repository.Interfaces
{
    public interface IBasketRepository
    {
        Task<BasketCart> GetBasket(string userName);
        Task<BasketCart> UpdateBasket(BasketCart basket);
        Task<bool> DeleteBasket(string userName);
    }
}
