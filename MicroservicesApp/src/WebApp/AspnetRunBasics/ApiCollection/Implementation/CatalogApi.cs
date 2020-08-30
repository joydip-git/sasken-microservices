using AspnetRunBasics.ApiCollection.Infrastructure;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using AspnetRunBasics.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRunBasics.ApiCollection.Implementation
{
    public class CatalogApi : BaseHttpClientWithFactory, ICatalogApi
    {
        private readonly IApiSettings _settings;

        public CatalogApi(IApiSettings settings, IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _settings = settings;
        }

        public async Task<CatalogModel> CreateCatalog(CatalogModel model)
        {
            try
            {
                var message = new HttpRequestBuilder(_settings.BaseAddress)
                                       .SetPath(_settings.CatalogPath)
                                       .HttpMethod(HttpMethod.Post)
                                       .GetHttpMessage();

                var json = JsonConvert.SerializeObject(model);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");

                return await SendRequest<CatalogModel>(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            try
            {
                var message = new HttpRequestBuilder(_settings.BaseAddress)
                                      .SetPath(_settings.CatalogPath)
                                      .HttpMethod(HttpMethod.Get)
                                      .GetHttpMessage();

                return await SendRequest<IEnumerable<CatalogModel>>(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CatalogModel> GetCatalog(string id)
        {
            try
            {
                var message = new HttpRequestBuilder(_settings.BaseAddress)
                                          .SetPath(_settings.CatalogPath)
                                          .AddToPath(id)
                                          .HttpMethod(HttpMethod.Get)
                                          .GetHttpMessage();

                return await SendRequest<CatalogModel>(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CatalogModel>> GetProductByCategory(string categoryName)
        {
            try
            {
                var message = new HttpRequestBuilder(_settings.BaseAddress)
                                        .SetPath(_settings.CatalogPath)
                                        .AddToPath("GetProductByCategory")
                                        .AddToPath(categoryName)
                                        .HttpMethod(HttpMethod.Get)
                                        .GetHttpMessage();

                return await SendRequest<IEnumerable<CatalogModel>>(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
