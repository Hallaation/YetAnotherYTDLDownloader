using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace YetAnotherYTDLDownloader.Converters
{
	/// <summary>
	/// Returns visible if the boolean is false
	/// and returns hidden if boolean is true
	/// </summary>
	internal class BoolToVisiblityInverted : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//value was true, collapse
			if (value is bool isboolValue && isboolValue)
			{
				return Visibility.Hidden;
			}
			//value was false, go visible
			else
			{
				return Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
