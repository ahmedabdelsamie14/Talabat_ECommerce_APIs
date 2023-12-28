using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;
using Talabat.Service;
using Order = Talabat.Core.Entities.Order_Aggregate.Order;

namespace Talabat.APIs.Controllers
{
	
	public class OrdersController :APIBaseController 
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public OrdersController(IOrderService orderService,IMapper mapper,IUnitOfWork unitOfWork)
        {
			_orderService = orderService;
		    _mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		//Create Order
		[ProducesResponseType(typeof(OrderToReturnDto),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [HttpPost]//Post=>baseurl/api/Orders
		[Authorize]
		public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
		{
			var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var MappedAddress = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);
			var Order=await _orderService.CreateOrderAsync(BuyerEmail, orderDto.BasketId,orderDto.DeliveryMethodId, MappedAddress);
			if(Order is null) 
			{
				return BadRequest(new ApiResponse(400, "There Is a Problem With Your Order"));
			}
			var MappedOrder=_mapper.Map<Order,OrderToReturnDto>(Order);
			return Ok(MappedOrder);
		}

		[ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[HttpGet]//Get=>baseurl/api/Orders
		[Authorize]
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
		{
			var BuyerEmail=User.FindFirstValue(ClaimTypes.Email);
			var Orders=await _orderService.GetOrdersForSpecificUserAsync(BuyerEmail);
			if(Orders is null)
			{
				return NotFound(new ApiResponse(404,"There Is No Orders For This User"));
			}
			var MappedOrder=_mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderToReturnDto>>(Orders);
			return Ok(MappedOrder);
		}

		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[Authorize]
		[HttpGet("{id}")]//Get=>baseurl/api/Orders
		public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
		{
			var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var Order=await _orderService.GetOrderByIdForSpecificUserAsync(BuyerEmail, id);
			if (Order is null)
			{
				return NotFound(new ApiResponse(404, $"There Is No Order With Id={id} for This User "));
			}
			var MappedOrder = _mapper.Map<Order, OrderToReturnDto>(Order);
			return Ok(MappedOrder);
		}

		[HttpGet("DeliveryMethods")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
		{
			var DeliveryMethods= await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
			return Ok(DeliveryMethods);
		}


	}
}
