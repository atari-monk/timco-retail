using System.Configuration;

namespace TRMDesktopUI.Library.Helpers
{
	public class ConfigHelper : IConfigHelper
	{
		public decimal GetTaxRate()
		{
			string rateText = ConfigurationManager.AppSettings["taxRate"];

			var isValidTaxRate = decimal.TryParse(rateText, out decimal output);

			if (isValidTaxRate == false)
			{
				throw new ConfigurationErrorsException("The tax rate is not set properly");
			}

			return output;
		}
	}
}