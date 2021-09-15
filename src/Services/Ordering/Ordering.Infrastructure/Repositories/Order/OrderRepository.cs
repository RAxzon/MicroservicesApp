using Ordering.Application.Contracts.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories.Order
{
    public class OrderRepository : RepositoryBase<Domain.Entities.Order>, IOrderRepository
    {

        public OrderRepository(OrderContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Domain.Entities.Order>> GetHighOrders(int price)
        {
            return await _context.Orders.Where(o => o.TotalPrice > price).ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Order>> GetOrderByUserName(string userName)
        {
            return await _context.Orders.Where(o => o.UserName == userName).ToListAsync();
        }

        
    }
}
