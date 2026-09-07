using AutoMapper;
using MassTransit;
using MessageMQCommon.MQ.Messages.OrderMsv;
using Order.Msv.Models;
using Order.Msv.Services;

namespace Order.Msv.Consumers
{
    public class OrderCreatedResultConsumer:IConsumer<OrderResultMessage>   
    {
        private readonly ILogger<OrderCreatedResultConsumer> _logger;
        private readonly OrderMsvDbContext _context;
        private readonly IMapper _mapper;
        private readonly OrderService _orderService;
        public OrderCreatedResultConsumer(ILogger<OrderCreatedResultConsumer> logger, OrderMsvDbContext context, IMapper mapper, OrderService orderService)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _orderService = orderService;
        }
        public async Task Consume(ConsumeContext<OrderResultMessage> context)
        {
            var message = context.Message;
            try
            {
                var updatedOrder = await _orderService.UpdateOrderAsync(message);

                _logger.LogInformation($"Received OrderCreatedResultMessage: OrderNumber={message.OrderNumber}, OrderResult={message.OrderResult}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OrderCreatedResultMessage");
            }   
        }
    }
}
