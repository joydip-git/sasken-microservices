using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBusRabbitMQ.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EventBusRabbitMQ.Producer;
using EventBusRabbitMQ.Common;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ILogger<BasketController> _logger;
        private readonly IBasketRepository _repository;
        private readonly IMapper _mapper;
        private readonly EventBusRabbitMQProducer _rabbitMQProducer;

        public BasketController(ILogger<BasketController> logger, IBasketRepository repository, IMapper mapper, EventBusRabbitMQProducer rabbitMQProducer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _rabbitMQProducer = rabbitMQProducer ?? throw new ArgumentNullException(nameof(rabbitMQProducer));
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

        [HttpDelete("{userName}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
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

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody]BasketCheckout basketCheckout)
        {
            try
            {
                //get the basket total price
                //remove the basket
                //send event to rabbitmq
                var basket = await _repository.GetBasket(basketCheckout.UserName);
                if (basket == null)
                    return BadRequest();

                var removlaStatus = await _repository.DeleteBasket(basket.UserName);
                if (!removlaStatus)
                    return BadRequest();

                var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
                eventMessage.RequestId = Guid.NewGuid();
                eventMessage.TotalPrice = basket.TotalPrice;

                _rabbitMQProducer.PublishBasketCheckout(EventBusConstants.BASKET_CHECKOUT_QUEUE, eventMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Accepted();
        }

    }
}