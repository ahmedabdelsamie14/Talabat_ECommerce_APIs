using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public interface ISpecifications<T> where T : BaseEntity
	{
		public Expression<Func<T, bool>> Criteria { get; set; }
		public List<Expression<Func<T, object>>> Includes { get; set; }	
		public Expression<Func<T,Object>> OrderBy { get; set; }
		public Expression<Func<T, Object>> OrderByDescending { get; set; }


		//Take(2)
		public int Take { get; set; }	

		//Skip(2)
		public int Skip { get; set; }

		public bool IsPaginationEnabled { get; set; }

	}
}
