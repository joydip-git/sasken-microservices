using AspnetRunBasics.ApiCollection.Infrastructure;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using AspnetRunBasics.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspnetRunBasics.ApiCollection.Implementation
{
    public class OrderApi : BaseHttpClientWithFactory, IOrderApi
    {
        private readonly IApiSettings _settings;
        public OrderApi(IApiSettings settings, IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        public async Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName)
        {
            try
            {
                var message = new HttpRequestBuilder(_settings.BaseAddress).SetPath(_settings.OrderPath).AddQueryString("userName", userName).HttpMethod(HttpMethod.Get).GetHttpMessage();
                return await SendRequest<IEnumerable<OrderResponseModel>>(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
