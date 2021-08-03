using Caliburn.Micro;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private readonly IProductEndpoint productEndpoint;
		private readonly IConfigHelper configHelper;
		private BindingList<ProductModel> products;
		private ProductModel selectedProduct;
		private BindingList<CartItemModel> cart = new BindingList<CartItemModel>();
		private int itemQuantity = 1;

		public BindingList<ProductModel> Products
		{
			get { return products; }
			set 
			{ 
				products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		public ProductModel SelectedProduct
		{
			get { return selectedProduct; }
			set 
			{ 
				selectedProduct = value;
				NotifyOfPropertyChange(() => SelectedProduct);
			}
		}

		public BindingList<CartItemModel> Cart
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

			foreach (CartItemModel item in Cart)
			{
				subTotal += item.Product.RetailPrice * item.QuantityInCart;
			}

			return subTotal;
		}
		
		private decimal CalculateTax()
		{
			decimal taxAmount = 0;
			decimal taxRate = configHelper.GetTaxRate()/100;

			foreach (CartItemModel item in Cart)
			{
				if(item.Product.IsTaxable)
				{
					taxAmount +=
					item.Product.RetailPrice
					* item.QuantityInCart
					* taxRate;
				}
			}

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
			, IConfigHelper configHelper)
		{
			this.productEndpoint = productEndpoint;
			this.configHelper = configHelper;
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			await LoadProducts();
		}

		private async Task LoadProducts()
		{
			var products = await productEndpoint.GetAll();
			Products = new BindingList<ProductModel>(products);
		}

		public void AddToCart()
		{
			var existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
			if(existingItem != null)
			{
				existingItem.QuantityInCart += ItemQuantity;
				//HACK - there should be a better way of refreshing the cart display
				Cart.Remove(existingItem);
				Cart.Add(existingItem);
			}
			else
			{
				var item = new CartItemModel
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
		}

		public bool CanRemoveFromCart
		{
			get
			{
				bool output = false;

				return output;
			}
		}

		public void RemoveFromCart()
		{
			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
		}

		public bool CanCheckOut
		{
			get
			{
				bool output = false;

				return output;
			}
		}

		public void CheckOut()
		{

		}
	}
}