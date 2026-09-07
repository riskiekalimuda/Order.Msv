using AutoMapper;
using MessageMQCommon.MQ.Messages.OrderMsv;
using MessageMQCommon.Respones;
using Order.Msv.DTOs;
using Order.Msv.Models;
using Microsoft.EntityFrameworkCore;    

namespace Order.Msv.Services
{
    public class OrderService
    {
        private readonly OrderMsvDbContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly IMapper _mapper;
        public OrderService(OrderMsvDbContext context, ILogger<OrderService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
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
            try
            {
                var order = _mapper.Map<TrxOrder>(orderRequest);
                order.CreatedAt = DateTime.Now;
                order.UpdatedAt = DateTime.Now;
                _context.TrxOrders.Add(order);
                await _context.SaveChangesAsync();
                return new ServiceResult<TrxOrder>(true) { IsSuccess = true, Data = order };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return new ServiceResult<TrxOrder>(false) { IsSuccess = false, ErrorMessage = "Error creating order", ErrorCode = "DATABASE_ERROR" };
            }
        }   
    }
}
