using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class ProductWithBrandAndTypeSpecifications:BaseSpecifications<Product>
	{
        //CTOR Is Used To Get All Products
        public ProductWithBrandAndTypeSpecifications(ProductSpecParams Params):base(P=>
        (!Params.BrandId.HasValue||P.ProductBrandId==Params.BrandId)&&(!Params.TypeId.HasValue || P.ProductTypeId == Params.TypeId))
        {
            Includes.Add(P => P.ProductType);
            Includes.Add(P => P.ProductBrand);

            if (!string.IsNullOrEmpty(Params.Sort))
            {
                switch(Params.Sort)
                {
                    case "PriceAsc":
                        AddOrderBy(P => P.Price);
                        break;
                    case "PriceDesc":
                        AddOrderByDescending(P => P.Price);
                        break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;
                }
            }



            //Products=100
            //PageSize=10
            //PageIndex=5


            //Skip => 40 => 4*10
            //Take =>10

            ApplyPagination(Params.PageSize*(Params.PageIndex-1),Params.PageSize);
        }

        //CTOR Is Used To Get Product BY Id 
        public ProductWithBrandAndTypeSpecifications(int id):base(P=>P.Id==id)
        {
			Includes.Add(P => P.ProductType);
			Includes.Add(P => P.ProductBrand);
		}
    }
}
