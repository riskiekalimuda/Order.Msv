using AutoMapper;
using MassTransit;
using MessageMQCommon.MQ.Messages.OrderMsv;
using MessageMQCommon.MQ.Names;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Order.Msv.DTOs;
using Order.Msv.Models;
using Order.Msv.Services;

namespace Order.Msv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderMsvDbContext _context;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IMapper _mapper;
        private readonly OrderService _orderService;
        public OrderController(OrderMsvDbContext context, ISendEndpointProvider sendEndpointProvider, IMapper mapper, OrderService orderService)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
            _mapper = mapper;
            _orderService = orderService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if(request == null)
            {
                return BadRequest("Request body is null.");
            }

            var result = await _orderService.CreateOrderAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            try
            {
                var orderMessage = _mapper.Map<OrderMessage>(result.Data);
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{QueueNames.OrderQueue.AddOrderQueue}"));
                await sendEndpoint.Send(orderMessage);
                await _context.SaveChangesAsync();

                return Ok(new { OrderID = Guid.NewGuid(), Message = "Order created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while processing the request: {ex.Message}");
            }
        }
    }
}


         


