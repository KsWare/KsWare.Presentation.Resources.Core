using System;
using System.Windows;

namespace KsWare.Presentation.Resources.Core {

	public class LookupResourceDictionary : ResourceDictionaryEx {

		internal static FrameworkElement? s_lookupRoot;

		public static readonly DependencyProperty IsLookupRootProperty =
			DependencyProperty.RegisterAttached("IsLookupRoot", typeof(bool), typeof(LookupResourceDictionary),
				new PropertyMetadata(false, OnIsLookupRootChanged));

		public static bool GetIsLookupRoot(DependencyObject obj) {
			return (bool) obj.GetValue(IsLookupRootProperty);
		}

		public static void SetIsLookupRoot(DependencyObject obj, bool value) {
			obj.SetValue(IsLookupRootProperty, value);
		}

		private static void OnIsLookupRootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is FrameworkElement frameworkElement && (bool) e.NewValue) {
				s_lookupRoot = frameworkElement;
			}
		}

		private string? _resourceKey;

		public string? ResourceKey {
			get => _resourceKey;
			set {
				_resourceKey = value;
				Load();
			}
		}

		private void Load() {
			if (string.IsNullOrEmpty(ResourceKey)) return;
			var value = s_lookupRoot?.TryFindResource(ResourceKey) ?? Application.Current.TryFindResource(ResourceKey);
			if (value is Uri uri) MergeResourceDictionary(new ResourceDictionary{Source = uri});
			else if (value is ResourceDictionary rd) MergeResourceDictionary(rd);
		}

		private void MergeResourceDictionary(ResourceDictionary resourceDictionary) {
			foreach (var key in resourceDictionary.Keys) {
				this[key] = resourceDictionary[key];
			}
		}

	}

}