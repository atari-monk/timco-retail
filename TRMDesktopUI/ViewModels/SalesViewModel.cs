using Caliburn.Micro;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private readonly IProductEndpoint productEndpoint;
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
				decimal subTotal = 0;

				foreach (var item in Cart)
				{
					subTotal += (item.Product.RetailPrice * item.QuantityInCart);
				}

				return subTotal.ToString("C");
			}
		}

		public string Tax
		{
			get
			{
				return "$0.00";
			}
		}

		public string Total
		{
			get
			{
				return "$0.00";
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
			IProductEndpoint productEndpoint)
		{
			this.productEndpoint = productEndpoint;
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