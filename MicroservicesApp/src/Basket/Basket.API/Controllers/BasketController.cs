using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Basket.API.Entities;
using Basket.API.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ILogger<BasketController> _logger;
        private IBasketRepository _repository;

        public BasketController(ILogger<BasketController> logger, IBasketRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpDelete("{userName}")]
        [ProducesResponseType(typeof(void),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            try
            {
                return Ok(await _repository.DeleteBasket(userName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{userName}")]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> GetBasket(string userName)
        {
            try
            {
                var basket = await _repository.GetBasket(userName);
                return Ok(basket ?? new BasketCart(userName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> UpdateBasket([FromBody]BasketCart basket)
        {
            try
            {
                return Ok(await _repository.UpdateBasket(basket));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}