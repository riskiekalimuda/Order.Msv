using AutoMapper;
using MassTransit;
using MessageMQCommon.MQ.Messages.OrderMsv;
using MessageMQCommon.MQ.Names;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Order.Msv.DTOs;
using Order.Msv.Models;

namespace Order.Msv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderMsvDbContext _context;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IMapper _mapper;
        public OrderController(OrderMsvDbContext context, ISendEndpointProvider sendEndpointProvider, IMapper mapper)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if(request == null)
            {
                return BadRequest("Request body is null.");
            }
            try
            {
                var order = _mapper.Map<TrxOrder>(request);
                order.CreatedAt = DateTime.Now;
                order.UpdatedAt = DateTime.Now;
                _context.TrxOrders.Add(order);
                await _context.SaveChangesAsync(); // Save the order to the database first to get the generated ID

                var orderMessage = _mapper.Map<OrderMessage>(order); 
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{QueueNames.OrderQueue.AddOrderQueue}"));
                await sendEndpoint.Send(orderMessage); 
                await _context.SaveChangesAsync();



                return Ok(new { OrderID = order.Id, Message = "Order created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }   
        }   
    }
}
