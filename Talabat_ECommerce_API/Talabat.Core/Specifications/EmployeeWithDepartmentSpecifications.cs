using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class EmployeeWithDepartmentSpecifications:BaseSpecifications<Employee>
	{
        public EmployeeWithDepartmentSpecifications()
        {
            Includes.Add(E=>E.Department);
        }

        public EmployeeWithDepartmentSpecifications(int id):base(Employee=>Employee.Id==id)
        {
			Includes.Add(E => E.Department);
		}
    }
}
