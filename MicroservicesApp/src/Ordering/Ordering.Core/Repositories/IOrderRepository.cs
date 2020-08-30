using Ordering.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.Core.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
    }
}
