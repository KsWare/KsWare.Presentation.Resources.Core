using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;

namespace KsWare.Presentation.Core.Utils {

	/// <summary>
	/// Class ResourceHelper.
	/// </summary>
	public static class ResourceHelper {

		/// <summary>
		/// Finds the resource by value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="frameworkElement">The framework element.</param>
		/// <returns>ResourceItemInfo.</returns>
		public static ResourceItemInfo FindResourceByValue(object value, FrameworkElement frameworkElement = null) {
			var current = (DependencyObject) frameworkElement;
			while (true) {
				var hierarchy = new List<ResourceDictionary>();
				ResourceDictionary resources = null;
				if (current == null) resources = Application.Current.Resources;
				else if (current is FrameworkElement fe) resources = fe.Resources;
				if (resources != null) {
					if (TryFindResourceByValue(resources, value, out var foundKey, out var foundDictionary, hierarchy)) {
						return new ResourceItemInfo {
							Value = value,
							Key = foundKey,
							SourceDictionary = foundDictionary,
							Hierarchy = hierarchy,
							FrameworkElement = current as FrameworkElement

						};
					}
				}
				if (current == null) return null;
				current = LogicalTreeHelper.GetParent(current);
			}
		}

		/// <summary>
		/// Finds the resource by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="frameworkElement">The framework element.</param>
		/// <returns>ResourceItemInfo.</returns>
		public static ResourceItemInfo FindResource(object key, FrameworkElement frameworkElement = null) {
			var current = (DependencyObject) frameworkElement;
			while (true) {
				var hierarchy = new List<ResourceDictionary>();
				ResourceDictionary resources = null;
				if (current == null) resources = Application.Current.Resources;
				else if (current is FrameworkElement fe) resources = fe.Resources;
				// else if (current is Style st) st.Resources;
				// else if (current is ControlTemplate ct) ct.Resources;
				// else if (current is DataTemplate dt) dt.Resources
				// else if (current is IHaveResources ihr) ... ;
				if (resources != null) {
					var te = frameworkElement == current;
					object foundValue;
					ResourceDictionary foundDictionary;
					var found = TryFindResource(resources, key, out foundValue, out foundDictionary, hierarchy);
					if (!found && te) found = TryFindResource(resources, key, out foundValue, out foundDictionary, hierarchy);

					return new ResourceItemInfo {
						Value = foundValue,
						Key = key,
						SourceDictionary = foundDictionary,
						Hierarchy = hierarchy,
						FrameworkElement = current as FrameworkElement
					};

				}
				if (current == null) return null;
				current = LogicalTreeHelper.GetParent(current);
			}
		}

		private static bool TryFindResource(ResourceDictionary resources, object key, out object value, out ResourceDictionary resourceDictionary, List<ResourceDictionary> hierarchy) {
			var foundValue = (object) null;
			var foundResourceDictionary = (ResourceDictionary) null;
			
			foreach (var mergedDict in resources.MergedDictionaries) {
				var currentHierarchy = new List<ResourceDictionary>(hierarchy) { mergedDict };
				if(TryFindResource(mergedDict, key, out var v, out var d, currentHierarchy)) {
					foundValue = v;
					foundResourceDictionary = d;
					hierarchy.Clear();
					hierarchy.AddRange(currentHierarchy);
				}
			}

			if (resources.Contains(key)) {
				resourceDictionary = resources;
				value = resources[key];
				hierarchy.Add(resources);
				return true;
			}

			value = foundValue;
			resourceDictionary = foundResourceDictionary;
			return foundResourceDictionary!=null;
		}

		private static bool TryFindResourceByValue(ResourceDictionary resources, object value, out object key, out ResourceDictionary resourceDictionary, List<ResourceDictionary> hierarchy) {
			var foundKey = (object) null;
			var foundResourceDictionary = (ResourceDictionary) null;
			
			foreach (var mergedDict in resources.MergedDictionaries) {
				var currentHierarchy = new List<ResourceDictionary>(hierarchy) { mergedDict };
				if(TryFindResourceByValue(mergedDict, value, out var k, out var d, currentHierarchy)) {
					foundKey = k;
					foundResourceDictionary = d;
					hierarchy.Clear();
					hierarchy.AddRange(currentHierarchy);
				}
			}

			foreach (DictionaryEntry entry in resources) {
				if (Equals(entry.Value, value)) {
					key = entry.Key;
					resourceDictionary = resources;
					hierarchy.Add(resources);
					return true;
				}
			}

			key = foundKey;
			resourceDictionary = foundResourceDictionary;
			return foundResourceDictionary!=null;
		}

		public static string? KeyToString(object? key, PropertyInfo? pi = null) {
			switch (key) {
				case null: return null;
				case string s: return s;
				case Type t: return $"{{x:Type {t.Name}}}";
				case ComponentResourceKey crk: return componentResourceKey(crk);
				//case TemplateKey  tk: return extend(tk.ToString()); 
				case ResourceKey rk: return extend(rk.ToString()); // only the type name
				case var o: return o.ToString();
			}
			return null;
			string componentResourceKey(ComponentResourceKey crk) {
				return
					$"{{ComponentResourceKey TypeInTargetAssembly={{x:Type {crk.TypeInTargetAssembly.Name}}}, ResourceId={crk.ResourceId}}}";
			}
			string extend(string s) {
				if (pi != null) return $"{{x:Static {pi.DeclaringType!.Name}.{pi.Name}}}";
				return s;
			}
		}

		public static string? KeyToXaml(object? key, PropertyInfo? pi = null) {
			switch (key) {
				case null: return null;
				case string s: return s;
				case Type t: return $"{{x:Type {t.Name}}}";
				case ComponentResourceKey crk: return extend(crk.ToString());
				case TemplateKey  tk: return extend(tk.ToString());
				case ResourceKey rk: return extend(rk.ToString()); // only the type name
				case var o: return o.ToString();
			}

			string extend(string s) {
				if (pi != null) return $"{{x:Static {pi.DeclaringType.Name}.{pi.Name}}}";
				return s;
			}
		}

		public static int CountUniqueKeys(ResourceDictionary resources) {
			var keys = new Dictionary<object, int>();
			
			void Enumerate(ResourceDictionary resourceDictionary, int level = 0) {
				foreach (var mergedDict in resourceDictionary.MergedDictionaries) {
					Enumerate(mergedDict, level+1);
				}
				foreach (var key in resourceDictionary.Keys) {
					if (!keys.TryGetValue(key, out _)) keys[key] = 1;
					else keys[key] += 1;
				}
			}
			Enumerate(resources);
			return keys.Count;
		}

	}

	/// <summary>
	/// Class ResourceItemInfo.
	/// </summary>
	public class ResourceItemInfo : IXamlLineInfo {

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		public object Key { get; set; }

		/// <summary>
		/// Gets a string representation of the <see cref="Key"/>.
		/// </summary>
		/// <value>The string representation of the key.</value>
		public string KeyString => ResourceHelper.KeyToString(Key);

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value { get; set; }

		/// <summary>
		/// Gets or sets the source dictionary.
		/// </summary>
		/// <value>The source dictionary.</value>
		public ResourceDictionary SourceDictionary { get; set; }

		/// <summary>
		/// Gets or sets the source.
		/// </summary>
		/// <value>The source.</value>
		public string Source { get; set; }

		/// <summary>
		/// Gets or sets the hierarchy.
		/// </summary>
		/// <value>The hierarchy.</value>
		public List<ResourceDictionary> Hierarchy { get; set; } = new List<ResourceDictionary>();

		/// <summary>
		/// Gets or sets the framework element.
		/// </summary>
		/// <value>The framework element.</value>
		public FrameworkElement FrameworkElement { get; set; }

		/// <summary>
		/// Gets a value that specifies whether line information is available.
		/// </summary>
		/// <value><c>true</c> if this instance has line information; otherwise, <c>false</c>.</value>
		public bool HasLineInfo { get; set; }

		/// <summary>
		/// Gets the line number to report.
		/// </summary>
		/// <value>The line number.</value>
		public int LineNumber { get; set; }

		/// <summary>
		/// Gets the line position to report.
		/// </summary>
		/// <value>The line position.</value>
		public int LinePosition { get; set; }
	}

}