using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace KsWare.Presentation.Resources.Core {

	// init
	// ThemeLoader.RegisterTheme("Name", Uri);
	// ThemeLoader.RegisterTheme("Alias", "Name");
	// usage
	// csharp: ThemeLoader.Load(<theme-uri>|<theme-alias>)
	// xaml: <Window ThemeLoader.Source=<theme-uri>|<theme-alias>

	public static class ThemeLoader {

		private static StartupEventHandler? s_appStartupFnc;
		private static EventHandler? s_initializeFnc;
		private static Uri? s_applicationTheme;
		private static readonly Dictionary<string, Uri> s_themes = new Dictionary<string, Uri>(StringComparer.OrdinalIgnoreCase);

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

			if (newUri != null ) {
				var trd = Load(newUri);
				element.Resources.MergedDictionaries.Insert(1,trd);
			}
		}

		private static void InitializeResources(Application app, Uri newUri, Uri oldUri) {
			if (s_appStartupFnc != null) app.Startup -= s_appStartupFnc;
			var overrides = CollectOverridesAndRemove(app.Resources, oldUri);

			var o = new ResourceDictionary();
			o.BeginInit();
			foreach (var d in overrides) o.Add(d.Key, d.Value);
			o.EndInit();
			app.Resources.MergedDictionaries.Insert(0,o);

			if (newUri != null ) {
				//TODO styles will be sealed, disable deferred initialization
				var trd = Load(newUri);
				app.Resources.MergedDictionaries.Insert(1,trd);
			}
			s_applicationTheme = newUri;
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
		
		public static void SetSource(object obj, Uri value) {
			if (obj is FrameworkElement fe) fe.SetValue(SourceProperty, value);
			else if (obj is Application app) {
				var fnc = new StartupEventHandler((s, _) => InitializeResources((Application)s, value, s_applicationTheme));
				if (app.MainWindow == null) {
					s_appStartupFnc = fnc;
					app.Startup += s_appStartupFnc;
					return;
				}
				fnc(app, null);
			}
			else throw new InvalidOperationException($"ThemeLoader is not supported for objects of type '{obj.GetType().Name}'.");
		}

		public static Uri GetSource(object obj) {
			if (obj is FrameworkElement fe) return (Uri) fe.GetValue(SourceProperty);
			if (obj is Application app) return s_applicationTheme;
			throw new InvalidOperationException($"ThemeLoader is not supported for objects of type '{obj.GetType().Name}'.");
		}

		public static void RegisterTheme(string name, Uri uri) {
			if (!uri.OriginalString.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase)) {
				if (!s_themes.TryGetValue(uri.OriginalString, out uri))
					throw new ArgumentException("Alias not found.",nameof(uri));
			}
			s_themes[name] = uri;
		}

		public static void RegisterTheme(string name, string uri) 
			=> RegisterTheme(name, new Uri(uri, uri.StartsWith("pack:")?UriKind.Absolute : UriKind.Relative));

		public static ThemeResourceDictionary Load(string nameOrUri)
			=> Load(new Uri(nameOrUri, nameOrUri.StartsWith("pack:") ? UriKind.Absolute : UriKind.Relative));

		public static ThemeResourceDictionary Load(Uri uri) {
			if (!s_themes.TryGetValue(uri.OriginalString, out var sourceUri)) sourceUri = uri;

			var rd = new ThemeResourceDictionary();
			rd.BeginInit();
			rd.Source = sourceUri;
			rd.EndInit();
			return rd;
		}

		public static void UnregisterTheme(string name) {
			s_themes.Remove(name);
		}

		public static void ClearRegistrations() {
			s_themes.Clear();
		}

	}

}
