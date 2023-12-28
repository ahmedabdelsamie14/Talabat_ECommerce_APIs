using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data.Configurations
{
	public class OrderConfigurations : IEntityTypeConfiguration<Order>//To Use Function Configure Implement Interface IEntityTypeConfiguration
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.Property(o=>o.Status)
				    .HasConversion(OStatus=>OStatus.ToString(),OStatus=>(OrderStatus) Enum.Parse(typeof(OrderStatus),OStatus));

			builder.Property(O => O.SubTotal)
				.HasColumnType("decimal(18,2)");

			builder.OwnsOne(O => O.ShippingAddress, SA => SA.WithOwner());//Meaning Order Has One Shipping Address And Willl Mapping The Shipping Address With Owner 

			builder.HasOne(O => O.DeliveryMethod)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);

		}
	}
}
