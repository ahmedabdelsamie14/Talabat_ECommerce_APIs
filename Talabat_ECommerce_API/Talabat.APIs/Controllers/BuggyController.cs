using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
	
	public class BuggyController : APIBaseController
	{
		private readonly StoreContext _dbContext;

		public BuggyController(StoreContext dbContext)
        {
			_dbContext = dbContext;
		}

		[HttpGet("NotFound")]
		//BaseUrl/api/Buggy/NotFound
		public ActionResult GetNotFoundRequest()
		{
			var Product = _dbContext.Products.Find(100);
			if(Product is null)
			{
			return NotFound(new ApiResponse(404));
			}
			return Ok(Product);
		}


		[HttpGet("ServerError")]
		public ActionResult GetServerError()
		{

			var Product = _dbContext.Products.Find(100);
			var ProductToReturn= Product.ToString(); // Error
			return Ok(ProductToReturn);
		}

		[HttpGet("BadRequest")]
		public ActionResult GetbadRequest()
		{
			return BadRequest(); 
		}

		[HttpGet("BadRequest/{id}")]
		public ActionResult GetbadRequest(int id)
		{
			return Ok();
		}

	}
}
