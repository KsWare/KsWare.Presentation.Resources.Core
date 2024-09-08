using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace KsWare.Presentation.Resources.Core {

	public static class ThemeLoader {

		private static Dictionary<string, Uri> s_themes = new Dictionary<string, Uri>();

		private static EventHandler? s_initializeFnc;

		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
			"Source", typeof(Uri), typeof(ThemeLoader), new FrameworkPropertyMetadata(default(Uri),OnSourceChanged));

		private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!(d is FrameworkElement element)) return;
			LookupResourceDictionary.s_lookupRoot=element;
			if (element.IsInitialized)
				InitializeResources(element,(Uri)e.NewValue, (Uri)e.OldValue);
			else {
				s_initializeFnc = (s, _) => InitializeResources((FrameworkElement)s, (Uri)e.NewValue, (Uri)e.OldValue);
				element.Initialized += s_initializeFnc;
			}
		}

		private static void InitializeResources(FrameworkElement element, Uri newUri, Uri oldUri) {
			if (s_initializeFnc != null) element.Initialized -= s_initializeFnc;
			var overrides = CollectOverridesAndRemove(element.Resources, oldUri);

			var o = new ResourceDictionary();
			o.BeginInit();
			foreach (var d in overrides) o.Add(d.Key, d.Value);
			o.EndInit();
			element.Resources.MergedDictionaries.Insert(0,o);

			if (newUri !=null ) {
				if (!s_themes.TryGetValue(newUri.OriginalString, out var uri)) uri = newUri;
				var trd = new ThemeResourceDictionary();
				trd.BeginInit();
				trd.Source = uri;
				trd.EndInit();
				element.Resources.MergedDictionaries.Insert(1,trd);
			}
		}

		private static List<DictionaryEntry> CollectOverridesAndRemove(ResourceDictionary resourceDictionary, Uri oldThemeUri) {
			var overridesList = new List<DictionaryEntry>();
			collectAndRemove(resourceDictionary);
			return overridesList;

			void collectAndRemove(ResourceDictionary rd) {
				rd.Cast<DictionaryEntry>()
					.Where(entry => $"{entry.Key}".EndsWith(".Overrides")).ToList().ForEach(entry => {
						overridesList.Add(entry); 
						rd.Remove(entry.Key);
					});
				
				foreach (var d in rd.MergedDictionaries.ToArray()) {
					if (d.Source != null) {
						if (d.Source.Equals(oldThemeUri)) rd.MergedDictionaries.Remove(d);
						continue;
					}
					collectAndRemove(d);
					if (!d.MergedDictionaries.Any() && !d.Keys.Cast<object>().Any())
						rd.MergedDictionaries.Remove(d);
				}
			}
		}
		
		public static void SetSource(FrameworkElement element, Uri value) {
			element.SetValue(SourceProperty, value);
		}

		public static Uri GetSource(FrameworkElement element) {
			return (Uri) element.GetValue(SourceProperty);
		}

		public static void RegisterTheme(string name, Uri uri) {
			s_themes[name] = uri;
		}
		public static void RegisterTheme(string name, string uri) {
			s_themes[name] = new Uri(uri, uri.StartsWith("pack:")?UriKind.Absolute : UriKind.Relative);
		}

	}

}
