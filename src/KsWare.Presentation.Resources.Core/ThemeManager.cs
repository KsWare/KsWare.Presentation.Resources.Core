using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Windows;

namespace KsWare.Presentation.Resources.Core {

	// init
	// ThemeManager.RegisterTheme("Name", Uri, isDarkMode);
	// ThemeManager.RegisterTheme("Alias", "Name");
	// usage
	// csharp: ThemeManager.Load(<theme-uri>|<theme-alias>)
	// xaml: <Window ThemeManager.Source=<theme-uri>|<theme-alias>
	// usage w/o theme:
	// <Window ThemeManager.IsDarkMode="True"

	public static class ThemeManager {

		private static StartupEventHandler? s_appStartupFnc;
		private static EventHandler? s_initializeFnc;
		private static Uri? s_applicationTheme;
		private static readonly Dictionary<string, ThemeInfo> s_themes = new Dictionary<string, ThemeInfo>(StringComparer.OrdinalIgnoreCase);

		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
			"Source", typeof(Uri), typeof(ThemeManager), new FrameworkPropertyMetadata(default(Uri),OnSourceChanged));
		
		public static readonly DependencyProperty IsDarkThemeProperty = DependencyProperty.RegisterAttached(
			"IsDarkTheme", typeof(bool), typeof(ThemeManager),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnIsDarkThemeChanged));

		internal static readonly DependencyPropertyProxy<bool> IsAppDarkThemeProxy = new DependencyPropertyProxy<bool>(false);

		public static void SetSource(object obj, Uri value) {
			if (obj is FrameworkElement fe) fe.SetValue(SourceProperty, value);
			else if (obj is Application app) {
				var fnc = new StartupEventHandler((s,_) => InitializeResources((Application)s, value, s_applicationTheme));
				if (app.MainWindow == null) {
					s_appStartupFnc = fnc;
					app.Startup += s_appStartupFnc;
					return;
				}
				fnc(app, null);
			}
			else throw new InvalidOperationException($"Source is not supported for objects of type '{obj.GetType().Name}'.");
		}

		public static Uri? GetSource(object obj) {
			if (obj is FrameworkElement fe) return (Uri) fe.GetValue(SourceProperty);
			if (obj is Application app) return s_applicationTheme;
			throw new InvalidOperationException($"Source is not supported for objects of type '{obj.GetType().Name}'.");
		}

		public static bool GetIsDarkTheme(object obj) {
			if (obj is FrameworkElement fe) {
				var value = fe.ReadLocalValue(IsDarkThemeProperty);
				return value == DependencyProperty.UnsetValue ? IsAppDarkThemeProxy.Value : (bool) value;
			}
			if (obj is Application) return IsAppDarkThemeProxy.Value;
			throw new ArgumentException($"IsDarkMode is not supported for objects of type '{obj.GetType().Name}'.");
		}

		/// <summary>
		/// Set a value indicating current theme is a dark theme.
		/// </summary>
		/// <param name="obj">FrameworkElement or Application</param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <remarks>
		/// If <see cref="ThemeManager.RegisterTheme(string,Uri,bool?)"/> is used, <c>IsDarkMode</c> is attached amd updated automatically. 
		/// </remarks>
		public static void SetIsDarkTheme(object obj, bool value) {
			if(obj is FrameworkElement fe) fe.SetValue(IsDarkThemeProperty, value);
			else if (obj is Application) IsAppDarkThemeProxy.Value = value;
			else throw new ArgumentException($"IsDarkMode is not supported for objects of type '{obj.GetType().Name}'.");
		}

		private static void OnIsDarkThemeChanged(object obj, DependencyPropertyChangedEventArgs e) {
			if (obj is FrameworkElement element) element.InvalidateVisual();
		}

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

				var isDarkMode = s_themes.Any(de => de.Value.Uri == newUri && de.Value.IsDark == true);
				SetIsDarkTheme(element,isDarkMode);
			}
		}

		private static void InitializeResources(Application app, Uri? newUri, Uri? oldUri) {
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

			var isDarkTheme = s_themes.Any(de => (de.Key==newUri?.OriginalString || de.Value.Uri == newUri) && de.Value.IsDark == true);
			SetIsDarkTheme(app,isDarkTheme);
		}

		private static List<DictionaryEntry> CollectOverridesAndRemove(ResourceDictionary resourceDictionary, Uri? oldThemeUri) {
			var overridesList = new List<DictionaryEntry>();
			collectAndRemove(resourceDictionary);
			return overridesList;

			void collectAndRemove(ResourceDictionary rd) {
				rd.Cast<DictionaryEntry>()
					.Where(entry => $"{entry.Key}".EndsWith(".Overrides")).ToList()
					.ForEach(entry => {
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

		public static void RegisterTheme(string name, Uri uri, bool? isDark = null) {
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (uri == null) throw new ArgumentNullException(nameof(uri));

			if (uri.OriginalString.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase)) {
				var themeInfo = s_themes.FirstOrDefault(de => de.Value.Uri == uri).Value ?? 
				                new ThemeInfo(uri,isDark);
				s_themes[name] = themeInfo;
			}
			else {
				// register alias
				if (!s_themes.TryGetValue(uri.OriginalString, out var themeInfo))
					throw new ArgumentException("Alias not found.", nameof(uri));
				s_themes[name] = themeInfo;
			}
		}

		public static void RegisterTheme(string name, string uri, bool? isDark = null) 
			=> RegisterTheme(name, new Uri(uri, uri.StartsWith("pack:")?UriKind.Absolute : UriKind.Relative), isDark);

		public static ThemeResourceDictionary Load(string nameOrUri)
			=> Load(new Uri(nameOrUri, nameOrUri.StartsWith("pack:") ? UriKind.Absolute : UriKind.Relative));

		public static ThemeResourceDictionary Load(Uri uri) {
			if (s_themes.TryGetValue(uri.OriginalString, out var themeInfo)) uri = themeInfo.Uri;

			var rd = new ThemeResourceDictionary();
			rd.BeginInit();
			rd.Source = uri;
			rd.EndInit();
			return rd;
		}

		public static void UnregisterTheme(string name) {
			s_themes.Remove(name);
			var uri = new Uri(name, name.StartsWith("pack:") ? UriKind.Absolute : UriKind.Relative);
			s_themes.Where(de=>de.Value.Uri==uri).Select(de=>de.Key).ToList().ForEach(key=>s_themes.Remove(key));
		}

		public static void ClearRegistrations() {
			s_themes.Clear();
		}

		internal class DependencyPropertyProxy<T> : DependencyObject {

			public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
				nameof(Value), typeof(T), typeof(DependencyPropertyProxy<T>), new PropertyMetadata(default(T)));

			public DependencyPropertyProxy(T value) {
				Value = value;
			}

			public T Value {
				get => (T) GetValue(ValueProperty);
				set => SetValue(ValueProperty, value);
			}

		}
	}

	public class ThemeInfo {

		public ThemeInfo(Uri uri, bool? isDark) {
			Uri = uri;
			IsDark = isDark;
		}

		public Uri Uri { get; }
		public bool? IsDark { get; }

	}

}
