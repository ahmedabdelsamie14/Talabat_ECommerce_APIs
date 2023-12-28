﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data
{
	public static class StoreContextSeed
	{
		//Seeding
		public static async Task SeedAsync(StoreContext dbContext)
		{

			if(!dbContext.ProductBrands.Any())//Check If THere Is Data In Table ProductBrands Don`t do the code at this if condition
			{
				var BrandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
				var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);

				if (Brands?.Count > 0)
				{
					foreach (var Brand in Brands)
					{
						await dbContext.Set<ProductBrand>().AddAsync(Brand);
					}
					await dbContext.SaveChangesAsync();
				}
			}


			if(!dbContext.ProductTypes.Any())
			{
				// Seeding Types
				var TypesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/types.json");
				var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);

				if (Types?.Count > 0)
				{
					foreach (var Type in Types)
					{
						await dbContext.Set<ProductType>().AddAsync(Type);
					}
					await dbContext.SaveChangesAsync();
				}
			}


			if(!dbContext.Products.Any())
			{
				// Seeding Products
				var ProductsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
				var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);

				if (Products?.Count > 0)
				{
					foreach (var Product in Products)
					{
						await dbContext.Set<Product>().AddAsync(Product);
					}
					await dbContext.SaveChangesAsync();
				}
			}

			if (!dbContext.DeliveryMethods.Any())
			{
				// Seeding Products
				var DeliveryMethodsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");
				var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);

				if (DeliveryMethods?.Count > 0)
				{
					foreach (var DeliveryMethod in DeliveryMethods)
					{
						await dbContext.Set<DeliveryMethod>().AddAsync(DeliveryMethod);
					}
					await dbContext.SaveChangesAsync();
				}
			}


		}
	}
}
