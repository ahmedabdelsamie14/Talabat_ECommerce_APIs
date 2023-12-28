using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
	//[Route("api/[controller]")]
	//[ApiController]
	public class ProductsController :APIBaseController
	{
		
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		

		public ProductsController(IMapper mapper ,IUnitOfWork unitOfWork)
        {
			_mapper = mapper;
		    _unitOfWork = unitOfWork;
		}

		//Get All Products
		//BaseURL/api/Product -> Get
		[Authorize]
		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts([FromQuery]ProductSpecParams Params)
		{
			var Spec = new ProductWithBrandAndTypeSpecifications(Params);
			var Products= await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);			
			var MappedProducts = _mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDTO>>(Products);
			var CountSpec = new ProductWithFilterationForCountAsync(Params);
			var Count =await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);			
			return Ok(new Pagination<ProductToReturnDTO>(Params.PageIndex,Params.PageSize,MappedProducts,Count));
		}

		//Get Product By Id
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(ProductToReturnDTO),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]

		public async Task<ActionResult<Product>> GetProductById(int id)
		{
			var Spec = new ProductWithBrandAndTypeSpecifications(id);
			var Product=await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(Spec);
			if(Product is null)
			{
				return NotFound(new ApiResponse(404));
			}
			var MappedProduct=_mapper.Map<Product,ProductToReturnDTO>(Product);
			return Ok(MappedProduct);
		}

		//Get All Types
		//BaseUrl/api/Products/Types
		[HttpGet("Types")]
		public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
		{
			var Types=await _unitOfWork.Repository<ProductType>().GetAllAsync();
			return Ok(Types);
		}

		//Get All Brands
		//BaseUrl/api/Products/Brands
		[HttpGet("Brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var Brands =await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
			return Ok(Brands);
		}

	}
}
