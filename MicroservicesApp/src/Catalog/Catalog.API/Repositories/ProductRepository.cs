using Catalog.API.Data.Intarfaces;
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
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            try
            {
                _context = context ?? throw new ArgumentNullException(nameof(context)); ;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
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
        //issue
        public async Task<bool> Delete(string id)
        {
            FilterDefinition<Product> findProductFilter = Builders<Product>.Filter.Eq("Id", id);
            try
            {
                DeleteResult deleteResult = await _context.Products.DeleteOneAsync(findProductFilter);
                return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Product> GetProduct(string id)
        {
            //FilterDefinition<Product> findByIdFilter = Builders<Product>.Filter.ElemMatch(p => p.Id, id);
            try
            {
                //return await _context
                //            .Products
                //            .Find(findByIdFilter)
                //            .FirstOrDefaultAsync();
                return await _context
                          .Products
                          .Find(p=>p.Id==id)
                          .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
        {
            FilterDefinition<Product> filterByCategory = Builders<Product>.Filter.Eq("Category", categoryName);
            try
            {
                return await _context.Products.Find(filterByCategory).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            FilterDefinition<Product> filterByName = Builders<Product>.Filter.Eq("Name", name);
            try
            {
                return await _context.Products.Find(filterByName).ToListAsync();
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

        public async Task<bool> Update(Product product)
        {
            try
            {
                ReplaceOneResult updateResult = await _context.Products.ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);

                return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
