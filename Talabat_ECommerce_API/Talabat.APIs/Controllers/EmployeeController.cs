using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmployeeController : ControllerBase
	{
		private readonly IGenericRepository<Employee> _employeeRepo;

		public EmployeeController(IGenericRepository<Employee> EmployeeRepo)
        {
			_employeeRepo = EmployeeRepo;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
		{
			var spec = new EmployeeWithDepartmentSpecifications();
			var Employees=await _employeeRepo.GetAllWithSpecAsync(spec);
			return Ok(Employees);

		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Employee>> GetEmployeeById(int id)
		{
			var Spec = new EmployeeWithDepartmentSpecifications(id);
			var Employee = await _employeeRepo.GetEntityWithSpecAsync(Spec);
			return Ok(Employee);
		}
	}
}
