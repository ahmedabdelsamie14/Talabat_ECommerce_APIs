using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;
using OrderAddress = Talabat.Core.Entities.Order_Aggregate.Address;
using IdentityAddress = Talabat.Core.Entities.Identity.Address;

namespace Talabat.APIs.Helpers
{
	public class MappingProfiles:Profile
	{
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDTO>()
                .ForMember(d=>d.ProductType,o=>o.MapFrom(s=>s.ProductType.Name))
                .ForMember(d=>d.ProductBrand,o=>o.MapFrom(s=>s.ProductBrand.Name))
                .ForMember(d=>d.PictureUrl,o=>o.MapFrom<ProductPictureUrlResolver>());



            CreateMap<IdentityAddress, AddressDto>().ReverseMap();//Mapping For Address Identity

            CreateMap<AddressDto, OrderAddress>();//Mapping For Address Order Aggregate



			CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();

            CreateMap<Order,OrderToReturnDto>()
                .ForMember(d=>d.DeliveryMethod,O=>O.MapFrom(S=>S.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryMethodCost, O => O.MapFrom(S => S.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
                .ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
                .ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureResolver>());
			

		}
	}
}
