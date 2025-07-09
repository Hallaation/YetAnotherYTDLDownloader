using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace YetAnotherYTDLDownloader.DepdencyProperties
{
	public static class AutoScrollBehaviour
	{
		public static readonly DependencyProperty AutoScrollProperty =
	 DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollBehaviour), new PropertyMetadata(false, AutoScrollPropertyChanged));
		public static void AutoScrollPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var scrollViewer = obj as ScrollViewer;
			if (scrollViewer != null && (bool)args.NewValue)
			{
				scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
				scrollViewer.ScrollToEnd();
			}
			else
			{
				Debug.Assert(scrollViewer != null);
				if (scrollViewer != null)
				{
					scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
				}
			}
		}

		private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			// Only scroll to bottom when the extent changed. Otherwise you can't scroll up
			if (e.ExtentHeightChange != 0)
			{
				var scrollViewer = sender as ScrollViewer;
				scrollViewer?.ScrollToBottom();
			}
		}

		public static bool GetAutoScroll(DependencyObject obj)
		{
			return (bool)obj.GetValue(AutoScrollProperty);
		}

		public static void SetAutoScroll(DependencyObject obj, bool value)
		{
			obj.SetValue(AutoScrollProperty, value);
		}
	}
}
