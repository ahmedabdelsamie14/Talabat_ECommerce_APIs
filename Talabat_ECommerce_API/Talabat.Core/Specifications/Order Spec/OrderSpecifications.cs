using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Spec
{
	public class OrderSpecifications:BaseSpecifications<Order>
	{
        public OrderSpecifications(string email):base(O=>O.BuyerEmail==email)
        {
            Includes.Add(O => O.DeliveryMethod);//Eager Loading For Navigational Property
            Includes.Add(O => O.Items);
            AddOrderByDescending(O => O.OrderDate);//order from New To Old By OrderDate
        }

        public OrderSpecifications(string email,int orderId)
			: base(O => O.BuyerEmail == email&&O.Id==orderId)
		{
			Includes.Add(O => O.DeliveryMethod);//Eager Loading For Navigational Property
			Includes.Add(O => O.Items);
		}
    }
}
