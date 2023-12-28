using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPaymentService _paymentService;

		//private readonly IGenericRepository<Product> _productRepo;
		//private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
		//private readonly IGenericRepository<Order> _orderRepo;

		public OrderService(IBasketRepository basketRepository
			                //,IGenericRepository<Product> ProductRepo
			                //,IGenericRepository<DeliveryMethod> DeliveryMethodRepo
			                //,IGenericRepository<Order> OrderRepo
							,IUnitOfWork unitOfWork
			                ,IPaymentService paymentService)
        {
			_basketRepository = basketRepository;
			this._unitOfWork = unitOfWork;
			this._paymentService = paymentService;
			//_productRepo = ProductRepo;
			//_deliveryMethodRepo = DeliveryMethodRepo;
			//_orderRepo = OrderRepo;
		}
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethodId, Address ShippingAddress)
		{
			// 1.Get Basket From Basket Repo

			var Basket = await _basketRepository.GetBasketAsync(basketId);

			// 2.Get Selected Items at Basket From Product Repo

			var OrderItems = new List<OrderItem>();
			if (Basket?.Items.Count > 0)
			{
				foreach (var item in Basket.Items)
				{

					var Product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
					var ProductItemOrdered = new ProductItemOrderd(Product.Id, Product.Name, Product.PictureUrl); 
					var OrderItem = new OrderItem(ProductItemOrdered, Product.Price, item.Quantity);
                    OrderItems.Add(OrderItem);
				}
			}

			//3.Calculate SubTotal // Price Of Product Quantity

			var SubTotal = OrderItems.Sum(item => item.Price * item.Quantity);

			//4.Get Delivery Method From DeliveryMethod Repo

			var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);

			//5.Create Order
			var Spec = new orderWithPaymentIntentSpec(Basket.PaymentIntentId);
			var ExOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec); 
			if(ExOrder is not null)
			{
				_unitOfWork.Repository<Order>().Delete(ExOrder);
				await _paymentService.CreateOrUpdatePaymentIntent(basketId);
			}


			var Order = new Order(buyerEmail, ShippingAddress, DeliveryMethod, OrderItems, SubTotal,Basket.PaymentIntentId);

			//6.Add Order Locally

			await _unitOfWork.Repository<Order>().Add(Order);

			//7.Save Order To Database [ToDo]

			var Result= await _unitOfWork.CompleteAsync();//SaveChanges

			if (Result <= 0)
			{
				return null;
			}

			return Order;


		}

		public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
		{
			var Spec = new OrderSpecifications(buyerEmail,orderId);
			var Order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
			return Order;
		}

		public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
		{
			var Spec = new OrderSpecifications(buyerEmail);
			var Orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(Spec);
			return Orders;
		}
	}
}
