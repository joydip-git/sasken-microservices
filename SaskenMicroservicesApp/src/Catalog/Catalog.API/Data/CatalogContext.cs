using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Settings;
using MongoDB.Driver;
using System;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(ICatalogDatabaseSettings setting)
        {
            try
            {
                var client = new MongoClient(setting.ConnectionString);
                var db = client.GetDatabase(setting.DatabaseName);
                Products = db.GetCollection<Product>(setting.CollectionName);
                CatalogContextSeedData.SeedData(Products);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IMongoCollection<Product> Products { get; }
    }
}
