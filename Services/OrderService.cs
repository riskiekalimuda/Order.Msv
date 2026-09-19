using AutoMapper;
using MassTransit;
using MassTransit.Transports;
using MessageMQCommon.MQ.Messages.OrderMsv;
using MessageMQCommon.MQ.Names;
using MessageMQCommon.Respones;
using Microsoft.EntityFrameworkCore;
using Order.Msv.DTOs;
using Order.Msv.Models;
using static MassTransit.ValidationResultExtensions;

namespace Order.Msv.Services
{
    public class OrderService
    {
        private readonly OrderMsvDbContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly IMapper _mapper;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        public OrderService(OrderMsvDbContext context, ILogger<OrderService> logger
            , IMapper mapper, ISendEndpointProvider sendEndpointProvider)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<ServiceResult<TrxOrder>> UpdateOrderAsync(OrderResultMessage orderResultMessage)
        {
            try
            {
                var order = await _context.TrxOrders.FirstOrDefaultAsync(o => o.OrderNumber == orderResultMessage.OrderNumber);
                if (order == null)
                {
                    return new ServiceResult<TrxOrder>(false) { IsSuccess = false, ErrorMessage = "Order not found", ErrorCode = "ORDER_NOT_FOUND" };
                }
                order.Status = orderResultMessage.OrderResult;
                order.UpdatedAt = DateTime.Now;
                _context.TrxOrders.Update(order);
                await _context.SaveChangesAsync();
                return new ServiceResult<TrxOrder>(true) { IsSuccess = true, Data = order };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
                return new ServiceResult<TrxOrder>(false) { IsSuccess = false, ErrorMessage = "Error updating order", ErrorCode = "DATABASE_ERROR" };
            }   
        }

        public async Task<ServiceResult<TrxOrder>> CreateOrderAsync(CreateOrderRequest orderRequest)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = _mapper.Map<TrxOrder>(orderRequest);
                order.CreatedAt = DateTime.Now;
                order.UpdatedAt = DateTime.Now;
                _context.TrxOrders.Add(order);
                await _context.SaveChangesAsync();

                var orderMessage = _mapper.Map<OrderMessage>(order);
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{QueueNames.OrderQueue.AddOrderQueue}"));
                await sendEndpoint.Send(orderMessage);

                await transaction.CommitAsync();

                return new ServiceResult<TrxOrder>(true) { IsSuccess = true, Data = order };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order");
                return new ServiceResult<TrxOrder>(false) { IsSuccess = false, ErrorMessage = "Error creating order", ErrorCode = "DATABASE_ERROR" };
            }
        }   
    }
}
