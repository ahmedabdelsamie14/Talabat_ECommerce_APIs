using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	
	[Authorize]
	public class PaymentsController :APIBaseController 
	{
		private readonly IPaymentService _paymentService;
		private readonly IMapper _mapper;

		public PaymentsController(IPaymentService paymentService,IMapper mapper)
        {
		    _paymentService = paymentService;
			_mapper = mapper;
		}

		// Create Or Update EndPoint
		[ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpPost]
         public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
		 {
			var CustomerBasket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
			if (CustomerBasket is null) return BadRequest(new ApiResponse(400, "There is a Problem With Your Basket"));
		    var MappedBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(CustomerBasket); 
			return Ok(MappedBasket);
		 }



	}
}
