using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
	public static class ApplicationServicesExtension
	{

		public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
		{

			Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
			Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
			Services.AddScoped(typeof(IOrderService), typeof(OrderService));
			Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
			Services.AddAutoMapper(typeof(MappingProfiles));


			#region Error Handling
			Services.Configure<ApiBehaviorOptions>(Options =>
				{
					Options.InvalidModelStateResponseFactory = (actionContext) =>
					{
						//ModelState => Dic [KeyValuePair]

						// Key=> Name of Param

						// Value => Errors

						var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
						.SelectMany(P => P.Value.Errors)
						.Select(E => E.ErrorMessage)
						.ToArray();

						var ValidationErrorResponse = new ApiValidationErrorResponse()
						{

							Errors = errors

						};

						return new BadRequestObjectResult(ValidationErrorResponse);
					};

				}); 
			#endregion

			return Services;

		}
	}
}
