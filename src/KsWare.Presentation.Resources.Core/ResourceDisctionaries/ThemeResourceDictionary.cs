using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace KsWare.Presentation.Resources.Core {

	public class ThemeResourceDictionary : ResourceDictionaryEx {

		public static readonly DependencyProperty Source2Property = DependencyProperty.RegisterAttached(
			"Source2", typeof(Uri), typeof(ThemeResourceDictionary), new FrameworkPropertyMetadata(default(Uri),OnSourceChanged));

		public static void SetSource2(DependencyObject element, Uri value) {
			element.SetValue(Source2Property, value);
		}

		public static Uri GetSource2(DependencyObject element) {
			return (Uri) element.GetValue(Source2Property);
		}

		private static EventHandler? s_initializeFnc;

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
				var trd = new ThemeResourceDictionary();
				trd.BeginInit();
				trd.Source = newUri;
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

		private ResourceDictionary _tempMerged;

		internal static ThemeResourceDictionary Current;

		/// <summary>
		/// Gets or sets the theme name.
		/// </summary>
		/// <value>The theme name.</value>
		public string ThemeName { get; set; }

		public new Uri Source {
			get => base.Source;
			set {
				base.Source = value;
				if (ThemeName == null) {
					ThemeName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(value.OriginalString.Split(new []{'\\','/'}).Last()));
				}
			}
		}

		public override void EndInit() {
			base.EndInit();
		}

		protected override void OnGettingValue(object key, ref object value, out bool canCache) {
			base.OnGettingValue(key, ref value, out canCache);
		}

		internal void AddTemporary(ResourceDictionary resourceDictionary) {
			if (_tempMerged == null) _tempMerged = new ResourceDictionary();
			_tempMerged.MergedDictionaries.Add(resourceDictionary);
		}

		internal void RemoveTemporary(ResourceDictionary resourceDictionary) {
			if (_tempMerged == null) return;
			_tempMerged.MergedDictionaries.Remove(resourceDictionary);
		}

		public static ThemeResourceDictionary Load(string theme) 
			=> ThemeManager.Load(theme);

	}

}
