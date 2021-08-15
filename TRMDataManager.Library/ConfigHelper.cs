using System.Configuration;

namespace TRMDataManager.Library
{
	public class ConfigHelper
	{
		public static decimal GetTaxRate()
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