using AspnetRunBasics.ApiCollection.Infrastructure;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using AspnetRunBasics.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRunBasics.ApiCollection.Implementation
{
    public class BasketApi : BaseHttpClientWithFactory, IBasketApi
    {
        private readonly IApiSettings _settings;

        public BasketApi(IApiSettings settings, IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _settings = settings;
        }

        public async Task CheckoutBasket(BasketCheckoutModel model)
        {
            try
            {                              
                var message = new HttpRequestBuilder(_settings.BaseAddress)
                                .SetPath(_settings.BasketPath)
                                .AddToPath("Checkout")
                                .HttpMethod(HttpMethod.Post)
                                .GetHttpMessage();

                var json = JsonConvert.SerializeObject(model);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");

                await SendRequest<BasketCheckoutModel>(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BasketModel> GetBasket(string userName)
        {
            try
            {
                var message = new HttpRequestBuilder(_settings.BaseAddress)
                                               .SetPath(_settings.BasketPath)
                                               .AddToPath(userName)
                                               .HttpMethod(HttpMethod.Get)
                                               .GetHttpMessage();

                return await SendRequest<BasketModel>(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BasketModel> UpdateBasket(BasketModel model)
        {
            try
            {
                var message = new HttpRequestBuilder(_settings.BaseAddress)
                                .SetPath(_settings.BasketPath)
                                .HttpMethod(HttpMethod.Post)
                                .GetHttpMessage();

                var json = JsonConvert.SerializeObject(model);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");

                return await SendRequest<BasketModel>(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
