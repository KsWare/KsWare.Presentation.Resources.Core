using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace KsWare.Presentation.Resources.Core {

	public static class ThemeLoader {

		private static EventHandler? s_initializeFnc;

		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
			"Source", typeof(Uri), typeof(ThemeLoader), new FrameworkPropertyMetadata(default(Uri),OnSourceChanged));

		private static void OnSourceChanged2(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!(d is FrameworkElement element)) return;
			LookupResourceDictionary.s_lookupRoot=element;

			// ReSharper disable once ComplexConditionExpression
			s_initializeFnc = (s,_) => {
				if (s != null) element.Initialized -= s_initializeFnc;
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

			if(!element.IsInitialized) element.Initialized += s_initializeFnc;
			else s_initializeFnc(null, EventArgs.Empty);
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
