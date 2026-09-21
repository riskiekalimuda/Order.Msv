using MassTransit;
using MessageMQCommon.MQ.Messages.OrderMsv;
using Order.Msv.Services;

namespace Order.Msv.Consumers
{
    public class UpdateOrderResultConsumer:IConsumer<UpdateOrderResultMessage>
    {
        private readonly ILogger<UpdateOrderResultConsumer> _logger;
        private readonly OrderService _orderService;
        public UpdateOrderResultConsumer(ILogger<UpdateOrderResultConsumer> logger, OrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }
        public async Task Consume(ConsumeContext<UpdateOrderResultMessage>context)
        {
            var result = await _orderService.UpdateResultOrderAsync(context.Message);
            if(result.IsSuccess)
            {
                _logger.LogInformation($"Update order number = {context.Message.OrderNumber} was succesfully.");
            }
            else
            {
                _logger.LogError("Error update order");
            }
        }
    }
}
