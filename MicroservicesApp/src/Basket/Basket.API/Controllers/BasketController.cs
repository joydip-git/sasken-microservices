﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repository.Interfaces;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producer;
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
        private readonly EventBusRabbitMQProducer _rabbitMQProducer;
        private readonly IMapper _mapper;

        public BasketController(ILogger<BasketController> logger, IBasketRepository repository, EventBusRabbitMQProducer rabbitMQProducer, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _rabbitMQProducer = rabbitMQProducer ?? throw new ArgumentNullException(nameof(rabbitMQProducer));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        [HttpPost]
        [Route("[action]")]            
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Checkout([FromBody]BasketCheckout basketCheckout)
        {

            try
            {
                var basket = await _repository.GetBasket(basketCheckout.UserName);
                if (basket == null)
                    return BadRequest();

                var status = await _repository.DeleteBasket(basket.UserName);
                if (!status)
                    return BadRequest();

                var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
                eventMessage.RequestId = new Guid();
                eventMessage.TotalPrice = basket.TotalPrice;

                _rabbitMQProducer.PublishBasketCheckout(EventBusConstants.BASKET_CHECKOUT_QUEUE, eventMessage);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return Accepted();
        }
    }
}