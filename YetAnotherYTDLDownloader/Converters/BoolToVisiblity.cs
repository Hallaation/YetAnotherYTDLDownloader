using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;

namespace YetAnotherYTDLDownloader.Converters
{
	internal class BoolToVisiblity : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//bool isboolvalue = (value is bool) && isboolvalue == true
			//assignment and evaluation in 1 line as opposed to nested ifs
			//value is true, we want to show
			if (value is bool isboolValue && isboolValue)
			{
				return Visibility.Visible;
			}
			//value was false, collapse
			else
			{
				return Visibility.Hidden;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
