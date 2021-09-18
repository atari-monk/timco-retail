using AutoMapper;
using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private readonly IProductEndpoint productEndpoint;
		private readonly ISaleEndpoint saleEndpoint;
		private readonly IConfigHelper configHelper;
		private readonly IMapper mapper;
		private BindingList<ProductDisplayModel> products;
		private ProductDisplayModel selectedProduct;
		private CartItemDisplayModel selectedCartItem;
		private BindingList<CartItemDisplayModel> cart = new BindingList<CartItemDisplayModel>();
		private int itemQuantity = 1;

		public BindingList<ProductDisplayModel> Products
		{
			get { return products; }
			set 
			{ 
				products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		public ProductDisplayModel SelectedProduct
		{
			get { return selectedProduct; }
			set 
			{ 
				selectedProduct = value;
				NotifyOfPropertyChange(() => SelectedProduct);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		public CartItemDisplayModel SelectedCartItem
		{
			get { return selectedCartItem; }
			set
			{
				selectedCartItem = value;
				NotifyOfPropertyChange(() => selectedCartItem);
				NotifyOfPropertyChange(() => CanRemoveFromCart);
			}
		}

		public BindingList<CartItemDisplayModel> Cart
		{
			get { return cart; }
			set 
			{ 
				cart = value;
				NotifyOfPropertyChange(() => Cart);
			}
		}

		public int ItemQuantity
		{
			get { return itemQuantity; }
			set 
			{ 
				itemQuantity = value;
				NotifyOfPropertyChange(() => ItemQuantity);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		public string SubTotal 
		{ 
			get
			{
				return CalculateSubTotal().ToString("C");
			}
		}

		private decimal CalculateSubTotal()
		{
			decimal subTotal = 0;

			foreach (CartItemDisplayModel item in Cart)
			{
				subTotal += item.Product.RetailPrice * item.QuantityInCart;
			}

			return subTotal;
		}
		
		private decimal CalculateTax()
		{
			decimal taxAmount = 0;
			decimal taxRate = configHelper.GetTaxRate()/100;

			taxAmount = Cart
				.Where(x => x.Product.IsTaxable)
				.Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

			return taxAmount;
		}

		public string Tax
		{
			get
			{
				return CalculateTax().ToString("C");
			}
		}

		public string Total
		{
			get
			{
				decimal total = CalculateSubTotal() + CalculateTax();
				return total.ToString("C"); ;
			}
		}

		public bool CanAddToCart
		{
			get
			{
				bool output = false;

				if(ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
				{
					output = true;
				}

				return output;
			}
		}

		public SalesViewModel(
			IProductEndpoint productEndpoint
			, ISaleEndpoint saleEndpoint
			, IConfigHelper configHelper
			, IMapper mapper)
		{
			this.productEndpoint = productEndpoint;
			this.saleEndpoint = saleEndpoint;
			this.configHelper = configHelper;
			this.mapper = mapper;
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			await LoadProducts();
		}

		private async Task LoadProducts()
		{
			var productsList = await productEndpoint.GetAll();
			var products = mapper.Map<List<ProductDisplayModel>>(productsList);
			Products = new BindingList<ProductDisplayModel>(products);
		}

		public void AddToCart()
		{
			var existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
			if(existingItem != null)
			{
				existingItem.QuantityInCart += ItemQuantity;
			}
			else
			{
				var item = new CartItemDisplayModel
				{
					Product = SelectedProduct
					, QuantityInCart = ItemQuantity
				};
				Cart.Add(item);
			}

			SelectedProduct.QuantityInStock -= ItemQuantity;
			ItemQuantity = 1;
			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
			NotifyOfPropertyChange(() => CanCheckOut);
		}

		public bool CanRemoveFromCart
		{
			get
			{
				bool output = false;

				if(SelectedCartItem != null && SelectedCartItem?.Product.QuantityInStock > 0)
				{
					output = true;
				}

				return output;
			}
		}

		public void RemoveFromCart()
		{
			SelectedCartItem.Product.QuantityInStock += 1;

			if (SelectedCartItem.QuantityInCart > 1)
			{
				SelectedCartItem.QuantityInCart -= 1;
			}
			else
			{
				Cart.Remove(SelectedCartItem);
			}

			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
			NotifyOfPropertyChange(() => CanCheckOut);
		}

		public bool CanCheckOut
		{
			get
			{
				bool output = false;
				
				if(Cart.Count > 0)
				{
					output = true;
				}

				return output;
			}
		}

		public async Task CheckOut()
		{
			var sale = new SaleModel();

			foreach (var item in Cart)
			{
				sale.SaleDetails.Add(new SaleDetailModel
				{
					ProductId = item.Product.Id
					, Quantity = item.QuantityInCart
				});
			}

			await saleEndpoint.PostSale(sale);
		}
	}
}