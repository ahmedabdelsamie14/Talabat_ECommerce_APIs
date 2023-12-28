﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.Repository
{
	public class BasketRepository : IBasketRepository
	{
		private readonly IDatabase _database;

		public BasketRepository(IConnectionMultiplexer redis)//Ask CLR to inject for me object from class implement interface IConnectionMultiplexer
		{
			_database = redis.GetDatabase();
		}
        public async Task<bool> DeleteBasketAsync(string BasketId)
		{
			return await _database.KeyDeleteAsync(BasketId);
		}


		public async Task<CustomerBasket?> GetBasketAsync(string BasketId)
		{
			var Basket=await _database.StringGetAsync(BasketId);
			//if(Basket.IsNull) { return null; }
			//else
			//{
			//	return JsonSerializer.Deserialize<CustomerBasket>(Basket);//To Convert From JSON To Object(CustomerBasket) =>Deserialize
			//}

			return Basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(Basket);

		}

		public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket Basket)
		{
			var JsonBasket=JsonSerializer.Serialize(Basket);
			var CreatedOrUpdated= await _database.StringSetAsync(Basket.Id, JsonBasket, TimeSpan.FromDays(1));
			if(!CreatedOrUpdated)return null;

			return await GetBasketAsync(Basket.Id);
		}
	}
}
