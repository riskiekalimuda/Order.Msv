using AutoMapper;
using MessageMQCommon.Respones;
using Order.Msv.DTOs;
using Order.Msv.Models;

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
