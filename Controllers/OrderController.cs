using AutoMapper;
using MassTransit;
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

            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdStr) || string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized("Unauthorized: User info not found in headers.");
            }

            if (!Guid.TryParse(userIdStr.ToString(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            var result = await _orderService.CreateOrderAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { OrderID = result.Data.Id, Message = "Order created successfully." });
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest updateOrderRequest)
        {
            if(updateOrderRequest == null)
            {
                return BadRequest("Request body is null");
            }
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdStr) || string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized("Unauthorized: User info not found in headers.");
            }

            if (!Guid.TryParse(userIdStr.ToString(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }
            var result = await _orderService.UpdateOrderAsync(updateOrderRequest);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { OrderID = result.Data.Id, Message = "Order created successfully." });
        }
    }
}


         


