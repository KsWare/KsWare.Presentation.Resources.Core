using System.Collections.Generic;
using System.Windows;

namespace KsWare.Presentation.Themes.Core {

	public static class ResourceInfo {

		public static readonly DependencyProperty StyleLocationProperty = DependencyProperty.RegisterAttached(
			"StyleLocation", typeof(string), typeof(ResourceInfo), new PropertyMetadata(default(string),OnStyleLocationChanged));

		private static readonly DependencyPropertyKey StyleLocationInheritancePropertyKey = DependencyProperty.RegisterAttachedReadOnly(
			"StyleLocationInheritance", typeof(List<string>), typeof(ResourceInfo), new PropertyMetadata(default(List<string>)));

		public static readonly DependencyProperty StyleLocationInheritanceProperty = StyleLocationInheritancePropertyKey.DependencyProperty;

		public static List<string> GetStyleLocationInheritance(DependencyObject obj) {
			return (List<string>)obj.GetValue(StyleLocationInheritanceProperty);
		}

		private static void SetStyleLocationInheritance(DependencyObject obj, List<string> value) {
			obj.SetValue(StyleLocationInheritancePropertyKey, value);
		}

		public static void SetStyleLocation(DependencyObject element, string value) {
			element.SetValue(StyleLocationProperty, value);
		}

		public static string GetStyleLocation(DependencyObject element) {
			return (string) element.GetValue(StyleLocationProperty);
		}

		private static void OnStyleLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var list = GetStyleLocationInheritance(d);
			if(list==null) SetStyleLocationInheritance(d, new List<string> {(string) e.NewValue});
			else list.Add((string) e.NewValue);
		}
	}

}
