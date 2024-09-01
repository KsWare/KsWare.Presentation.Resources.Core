using System;
using System.Linq;
using System.Windows;

namespace KsWare.Presentation.Themes.Core {

	public static class ThemeLoader {

		private static EventHandler  _initializeFnc;

		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
			"Source", typeof(Uri), typeof(ThemeLoader), new FrameworkPropertyMetadata(default(Uri),OnSourceChanged));

		private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!(d is FrameworkElement element)) return;

			// ReSharper disable once ComplexConditionExpression
			_initializeFnc = (s,_) => {
				if (s != null) element.Initialized -= _initializeFnc;
				var insertPosition = 0;
				if (e.OldValue is Uri oldUri) {
					var oldDic = element.Resources.MergedDictionaries.FirstOrDefault(rd => rd.Source == oldUri);
					if (oldDic!=null) {
						insertPosition = element.Resources.MergedDictionaries.IndexOf(oldDic);
						element.Resources.MergedDictionaries.Remove(oldDic);
					}
				}
				if (e.NewValue is Uri newUri ) {
					var trd = new ThemeResourceDictionary();
					trd.BeginInit();
					trd.Source = newUri;
					trd.EndInit();
					element.Resources.MergedDictionaries.Insert(insertPosition,trd);
				}
			};

			if(!element.IsInitialized) element.Initialized += _initializeFnc;
			else _initializeFnc(null, EventArgs.Empty);
		}

		private static void lucky(object sender, EventArgs e) {
			throw new NotImplementedException();
		}

		public static void SetSource(FrameworkElement element, Uri value) {
			element.SetValue(SourceProperty, value);
		}

		public static Uri GetSource(FrameworkElement element) {
			return (Uri) element.GetValue(SourceProperty);
		}

	}

}
