using MediatR;
using Ordering.Application.Mapper;
using Ordering.Application.Queries;
using Ordering.Application.Responses;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Handlers
{
    public class GetOrderByUserNameHandler : IRequestHandler<GetOrderByUserNameQuery, IEnumerable<OrderResponse>>
    {
        private readonly IOrderRepository _oderRepository;

        public GetOrderByUserNameHandler(IOrderRepository orderRepository)
        {
            _oderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<IEnumerable<OrderResponse>> Handle(GetOrderByUserNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var orderList = await _oderRepository.GetOrdersByUserName(request.UserName);
                var orderResponseList = OrderMapper.Mapper.Map<IEnumerable<OrderResponse>>(orderList);
                return orderResponseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
