using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KsWare.Presentation.Resources.Core {

	// <Image
	//    ThemeImageSource.Light="resource-uri"
	//    ThemeImageSource.Dark="resource-uri"
	// />

	// alternatives
	// theme:Dark.Source = ImageSource        class Dark { public ImageSource Source {get;set;}
	// theme:Dark.Background = Brush
	// :-( we would need a class with ALL possible property names!
	// :-( we would only be able to have one type per name


	public static class ThemeImageSource {

		public static readonly DependencyProperty LightProperty = DependencyProperty.RegisterAttached(
			"Light", typeof(ImageSource), typeof(ThemeImageSource),
			new FrameworkPropertyMetadata(default(ImageSource), OnPropertyChanged));

		public static readonly DependencyProperty DarkProperty = DependencyProperty.RegisterAttached(
			"Dark", typeof(ImageSource), typeof(ThemeImageSource),
			new FrameworkPropertyMetadata(default(ImageSource), OnPropertyChanged));

		public static void SetLight(DependencyObject element, ImageSource value) {
			element.SetValue(LightProperty, value);
		}

		public static ImageSource GetLight(DependencyObject element) {
			return (ImageSource) element.GetValue(LightProperty);
		}

		public static void SetDark(DependencyObject element, ImageSource value) {
			element.SetValue(DarkProperty, value);
		}

		public static ImageSource GetDark(DependencyObject element) {
			return (ImageSource) element.GetValue(DarkProperty);
		}

		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is Image image) {
				BindingOperations.ClearBinding(image, Image.SourceProperty);
				BindingOperations.SetBinding(image, Image.SourceProperty, new MultiBinding {
					Bindings = {
						new Binding {
							Path = new PropertyPath(ThemeManager.IsDarkThemeProperty),
							Source = image,
							Mode = BindingMode.OneWay
						},
						new Binding {
							Path = new PropertyPath(ThemeManager.DependencyPropertyProxy<bool>.ValueProperty),
							Source = ThemeManager.IsAppDarkThemeProxy,
							Mode = BindingMode.OneWay
						},
						new Binding {
							Source = image,
							Mode = BindingMode.OneWay
						},
					},
					Converter = new IsDarkModeToImageSourceConverter(GetLight(d), GetDark(d))
				});
			}
		}

		[SuppressMessage("ReSharper", "TooManyArguments", Justification = "by design")]
		private class IsDarkModeToImageSourceConverter :IValueConverter, IMultiValueConverter {

			private readonly ImageSource? _light;
			private readonly ImageSource? _dark;

			public IsDarkModeToImageSourceConverter(ImageSource light, ImageSource dark) {
				_light = light;
				_dark = dark;
			}

			public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
				return ((bool?) value == true) ? _dark??_light : _light;
			}

			public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture) {
				// values[0]: IsDarkMode von FrameworkElement
				// values[1]: IsDarkMode von Application über den Proxy
				// values[2]: FrameworkElement

				var isUnsetValue = ((Image) values[2]).ReadLocalValue(ThemeManager.IsDarkThemeProperty) ==
				                   DependencyProperty.UnsetValue;

				if (isUnsetValue) return values[1] is true ? _dark??_light : _light;
				return values[0] is true ? _dark ?? _light : _light;
			}

			public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
				throw new NotImplementedException();
			}

			public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture) {
				throw new NotImplementedException();
			}

		}

	}

}