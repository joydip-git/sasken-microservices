using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            try
            {
                _context = context ?? throw new ArgumentNullException("null context");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Create(Product product)
        {
            try
            {
                await _context.Products.InsertOneAsync(product);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Delete(string id)
        {
            FilterDefinition<Product> idFilter =
              Builders<Product>.Filter.Eq("Id", id);
            try
            {
                DeleteResult deleteResult = await _context.Products.DeleteOneAsync(idFilter);
                return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Product> GetProductById(string id)
        {
            try
            {
                return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            FilterDefinition<Product> nameFilter =
                Builders<Product>.Filter.Eq("Name", name);
            try
            {
                return await _context.Products.Find(nameFilter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            try
            {
                return await _context.Products.Find(p => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
        {
            FilterDefinition<Product> categoryNameFilter =
                 Builders<Product>.Filter.Eq("Category", categoryName);
            try
            {
                return await _context.Products.Find(categoryNameFilter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(Product product)
        {
            try
            {
                ReplaceOneResult updateResult = await _context.Products.ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);
                return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
