﻿using AutoMapper;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder.CommandRequests;
using Ordering.Application.Features.Orders.Commands.DeleteOrder.CommandRequest;
using Ordering.Application.Features.Orders.Commands.UpdateOrder.CommandRequests;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;
using Ordering.Domain.Entities;

namespace Ordering.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrdersVm>().ReverseMap();
            CreateMap<Order, CheckoutOrderCommand>().ReverseMap();
            CreateMap<Order, UpdateOrderCommand>().ReverseMap();
            CreateMap<Order, DeleteOrderCommand>().ReverseMap();
        }
    }
}
