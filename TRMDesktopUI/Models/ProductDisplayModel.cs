using System.ComponentModel;

namespace TRMDesktopUI.Models
{
	public class ProductDisplayModel : INotifyPropertyChanged
	{
		private int quantityInStock;

		public int Id
		{
			get; set;
		}
		public string ProductName
		{
			get; set;
		}
		public string Description
		{
			get; set;
		}
		public decimal RetailPrice
		{
			get; set;
		}

		public int QuantityInStock
		{
			get {
				return quantityInStock;
			}
			set {
				quantityInStock = value;
				CallPropertyChanged(nameof(QuantityInStock));
			}
		}

		public bool IsTaxable
		{
			get; set;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void CallPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}